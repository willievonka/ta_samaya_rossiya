import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment, IEnvironment } from '../../environments';
import { IHubCard } from '../components/hub-card/interfaces/hub-card.interface';
import { NavigationExtras, Router } from '@angular/router';

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
     * Получить карточку по id
     * @param id
     */
    public getMapCardById(id: string): Observable<IHubCard | null> {
        return this.getMapCardsList()
            .pipe(
                map(list => list.find(card => card.id === id) ?? null)
            );
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

        const extras: NavigationExtras = {
            queryParams: { id: card.id },
            ...(edit ? { state: { card } } : {})
        };

        this._router.navigate([path], extras);
    }

    /** Перейти на страницу создания карты */
    public navigateToCreateMap(): void {
        this._router.navigate(['admin/create-map']);
    }
}
