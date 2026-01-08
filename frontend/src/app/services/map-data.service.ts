import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, EMPTY, map, Observable } from 'rxjs';
import { IMapLayerProperties } from '../components/map/interfaces/map-layer.interface';
import { environment } from '../../environments';
import { IMapDto } from '../components/map/dto/map.dto';
import { IMapModel } from '../components/map/models/map.model';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class MapDataService {
    private readonly _http: HttpClient = inject(HttpClient);
    private readonly _clientApiUrl: string = environment.clientApiUrl;
    private readonly _adminApiUrl: string = environment.adminApiUrl;
    private readonly _router: Router = inject(Router);

    /**
     * Получить данные для карты по mapId
     * @param mapId
     */
    public getMapData(mapId: string, raw: boolean = false): Observable<IMapModel> {
        const baseUrl: string = raw ? this._adminApiUrl : this._clientApiUrl;
        const options: { params?: HttpParams } | undefined = raw
            ? undefined
            : { params: new HttpParams({ fromObject: { mapId } }) };

        return this._http.get<IMapDto>(`${baseUrl}/maps`, options)
            .pipe(
                map((dto: IMapDto) => this.mapDtoToModel(dto)),
                catchError(() => {
                    this._router.navigate(['/']);

                    return EMPTY;
                })
            );
    }

    /**
     * Смаппить dto в модель
     * @param dto
     */
    private mapDtoToModel(dto: IMapDto): IMapModel {
        return {
            pageTitle: dto.pageTitle,
            infoText: dto.infoText,
            layers: dto.layers.features.map((feature) => ({
                geoData: feature.geometry,
                properties: feature.properties as IMapLayerProperties
            })),
            layerWithPointsColor: dto.activeLayerColor,
            pointColor: dto.pointColor
        };
    }
}
