import { computed, Injectable, signal, Signal, WritableSignal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthService {
    public readonly isLoggedIn: Signal<boolean> = computed(() => this._isLoggedIn());
    private readonly _isLoggedIn: WritableSignal<boolean> = signal(false);

    /**
     * Войти в аккаунт
     * @param login
     * @param password
     */
    public login(login: string, password: string): void {
        console.log(login, password);
        this._isLoggedIn.set(true);
    }

    /** Выйти из аккаунта */
    public logout(): void {
        console.log('logout');
        this._isLoggedIn.set(false);
    }
}
