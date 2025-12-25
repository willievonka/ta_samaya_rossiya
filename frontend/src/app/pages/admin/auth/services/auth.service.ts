import { Injectable } from '@angular/core';

@Injectable()
export class AuthService {
    /**
     * Войти в аккаунт
     * @param login
     * @param password
     */
    public login(login: string, password: string): void {
        console.log(login, password);
    }

    /** Выйти из аккаунта */
    public logout(): void {
        console.log('logout');
    }
}
