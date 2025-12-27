import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments';
import { catchError, Observable, of, tap } from 'rxjs';

interface IJwtPayload {
    exp?: number;
    [key: string]: unknown;
};

@Injectable({ providedIn: 'root' })
export class AuthService {
    private readonly _http: HttpClient = inject(HttpClient);
    private readonly _apiUrl: string = environment.adminApiUrl;

    /** fallback-TTL токена (если не пришел в JWT) */
    private readonly _tokenTimeToLive: number = environment.authTokenTimeToLive;
    private readonly _tokenKey: string = 'AUTH_TOKEN';
    private readonly _expiresAtKey: string = 'AUTH_EXPIRES_AT';
    /** id активного таймера авто-разлогина */
    private _logoutTimerId: number | null = null;

    /**
     * Войти в аккаунт
     * @param email
     * @param password
     */
    public login(email: string, password: string): Observable<string> {
        return this._http.post(
            `${this._apiUrl}/auth/login`,
            { email, password },
            { responseType: 'text' },
        )
            .pipe(tap(jwt => this.setSession(jwt)));
    }

    /** Выйти из аккаунта */
    public logout(): Observable<void> {
        this.stopAutoLogoutTimer();

        return this._http.post<void>(`${this._apiUrl}/auth/logout`, null)
            .pipe(
                tap(() => this.clearSession()),
                catchError(() => {
                    this.clearSession();

                    return of(void 0);
                })
            );
    }

    /** Инициализация таймера авторазлогина */
    public initAuthTimer(): void {
        const token: string | null = this.getToken();
        const expiresAt: number = this.getExpiresAt();

        if (!token || !Number.isFinite(expiresAt) || this.isExpired()) {
            this.clearSession();

            return;
        }

        this.startAutoLogoutTimer(expiresAt);
    }

    /** Проверка авторизации */
    public checkAuth(): boolean {
        return !!this.getToken() && !this.isExpired();
    }

    /** Получить токен авторизации */
    public getToken(): string | null {
        return localStorage.getItem(this._tokenKey);
    }

    /** Форсированный разлогин без вызова API */
    public forceLogoutLocal(): void {
        this.stopAutoLogoutTimer();
        this.clearSession();
    }

    /**
     * Создать новую сессию
     * @param jwt
     */
    private setSession(jwt: string): void {
        localStorage.setItem(this._tokenKey, jwt);

        const expiresAt: number =
            this.getExpiresAtFromJwt(jwt) ??
            (Date.now() + this._tokenTimeToLive);

        localStorage.setItem(this._expiresAtKey, String(expiresAt));
        this.startAutoLogoutTimer(expiresAt);
    }

    /** Очистить сессию */
    private clearSession(): void {
        localStorage.removeItem(this._tokenKey);
        localStorage.removeItem(this._expiresAtKey);
    }

    /** Получить срок жизни токена */
    private getExpiresAt(): number {
        return Number(localStorage.getItem(this._expiresAtKey));
    }

    /** Проверить, истек ли срок жизни токена */
    private isExpired(): boolean {
        const expiresAt: number = this.getExpiresAt();

        return !Number.isFinite(expiresAt) || Date.now() >= expiresAt;
    }

    /**
     * Запуситить таймер авторазлогина
     * @param expiresAt
     */
    private startAutoLogoutTimer(expiresAt: number): void {
        this.stopAutoLogoutTimer();

        const msLeft: number = expiresAt - Date.now();
        if (msLeft <= 0) {
            this.forceLogoutLocal();

            return;
        }

        this._logoutTimerId = window.setTimeout(() => {
            this.forceLogoutLocal();
        }, msLeft);
    }

    /** Остановить активный таймер авторазлогина */
    private stopAutoLogoutTimer(): void {
        if (this._logoutTimerId !== null) {
            window.clearTimeout(this._logoutTimerId);
            this._logoutTimerId = null;
        }
    }

    /**
     * Получить TTL токена из JWT формата
     * @param jwt
     */
    private getExpiresAtFromJwt(jwt: string): number | null {
        try {
            const payload: IJwtPayload = this.decodeJwtPayload(jwt);

            return typeof payload.exp === 'number' ? payload.exp * 1000 : null;
        } catch {
            return null;
        }
    }

    /**
     * Декодировать JWT
     * @param jwt
     */
    private decodeJwtPayload(jwt: string): IJwtPayload {
        const [, payloadBase64Url]: string[] = jwt.split('.');
        if (!payloadBase64Url) {
            throw new Error('Invalid JWT');
        }

        const base64: string = payloadBase64Url.replace(/-/g, '+').replace(/_/g, '/');
        const padded: string = base64.padEnd(base64.length + (4 - (base64.length % 4)) % 4, '=');
        const json: string = atob(padded);

        return JSON.parse(json) as IJwtPayload;
    }
}
