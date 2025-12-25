import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment, IEnvironment } from '../../environments';
import { IHubCard } from '../components/hub-card/interfaces/hub-card.interface';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class HubService {
    private readonly _http: HttpClient = inject(HttpClient);
    private readonly _env: IEnvironment = environment;
    private readonly _router: Router = inject(Router);

    /**
     * Получить список карточек
     */
    public getMapCardsList(): Observable<IHubCard[]> {
        return this._http.get<IHubCard[]>(`${this._env.clientApiUrl}/maps/cards`);
    }

    /**
     * Перейти на страницу карты
     * @param card
     * @param edit
     */
    public navigateToMap(card: IHubCard, edit: boolean = false): void {
        const path: string = edit
            ? (card.isAnalytics ? 'admin/edit-analytics-map' : 'admin/edit-map')
            : (card.isAnalytics ? 'analytics-map' : 'map');

        this._router.navigate([path], { queryParams: { id: card.id } });
    }
}
