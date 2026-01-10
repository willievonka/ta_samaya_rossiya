import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments';
import { catchError, finalize, Observable, tap, throwError } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
    private readonly _http: HttpClient = inject(HttpClient);
    private readonly _apiUrl: string = environment.adminApiUrl;
    private readonly _accessTokenKey: string = 'ACCESS_TOKEN';

    /**
     * Войти в аккаунт
     * @param email
     * @param password
     */
    public login(email: string, password: string): Observable<{ accessToken: string }> {
        return this._http.post<{ accessToken: string }>(
            `${this._apiUrl}/auth/login`,
            { email, password },
            { withCredentials: true }
        ).pipe(
            tap(res => localStorage.setItem(this._accessTokenKey, res.accessToken)),
            catchError(err => {
                this.clearToken();

                return throwError(() => err);
            })
        );
    }

    /** Выйти из аккаунта */
    public logout(): Observable<void> {
        return this._http.post<void>(
            `${this._apiUrl}/auth/logout`,
            null,
            { withCredentials: true }
        ).pipe(
            finalize(() => this.clearToken())
        );
    }

    /** Обновить токен */
    public refreshToken(): Observable<{ accessToken: string }> {
        return this._http.post<{ accessToken: string }>(
            `${this._apiUrl}/auth/refresh`,
            null,
            { withCredentials: true }
        ).pipe(
            tap(res => localStorage.setItem(this._accessTokenKey, res.accessToken)),
            catchError(err => {
                this.clearToken();

                return throwError(() => err);
            })
        );
    }

    /** Получить accessToken */
    public getAccessToken(): string | null {
        return localStorage.getItem(this._accessTokenKey);
    }

    /** Проверка авторизации */
    public checkAuth(): boolean {
        return !!this.getAccessToken();
    }

    /** Очистка токена */
    public forceLogoutLocal(): void {
        this.clearToken();
    }

    /** Очистить токен */
    private clearToken(): void {
        localStorage.removeItem(this._accessTokenKey);
    }
}
