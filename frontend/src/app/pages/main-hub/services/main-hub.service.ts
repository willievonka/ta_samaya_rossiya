import { inject, Injectable } from '@angular/core';
import { IMainHubCard } from '../interfaces/main-hub-card.interface';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment, IEnvironment } from '../../../../environments';

@Injectable()
export class MainHubService {
    private readonly _http: HttpClient = inject(HttpClient);
    private readonly _env: IEnvironment = environment;

    /**
     * Получить список карточек
     */
    public getMapCardList(): Observable<IMainHubCard[]> {
        return this._http.get<IMainHubCard[]>(`${this._env.clientApiUrl}/maps/cards`);
    }
}
