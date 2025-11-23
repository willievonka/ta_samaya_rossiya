import { inject, Injectable } from '@angular/core';
import { IMainHubCard } from '../interfaces/main-hub-card.interface';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class MainHubService {
    private readonly _http: HttpClient = inject(HttpClient);
    private readonly _router: Router = inject(Router);

    /**
     * Получить список карточек
     */
    public getMapCardList(): Observable<IMainHubCard[]> {
        return this._http.get<IMainHubCard[]>('/main-hub-cards/cards.json');
    }

    /**
     * Редирект на страницу с картой
     * @param id
     */
    public navigateToMap(id: string): void {
        if (id === '0') {
            this._router.navigate(['analytics-map']);
        } else {
            console.log(id);
        }
    }
}
