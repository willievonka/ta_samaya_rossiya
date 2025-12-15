import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { IMapLayerProperties } from '../interfaces/map-layer.interface';
import { environment } from '../../../../environments';
import { IMapDto } from '../dto/map.dto';
import { IMapModel } from '../models/map.model';

@Injectable({ providedIn: 'root' })
export class MapDataService {
    private readonly _http: HttpClient = inject(HttpClient);
    private readonly _apiUrl: string = environment.clientApiUrl;

    /**
     * Получить данные для карты по mapId
     * @param mapId
     */
    public getMapData(mapId: string): Observable<IMapModel> {
        return this._http.get<IMapDto>(`${this._apiUrl}/maps`, { params: { mapId } })
            .pipe(
                map((dto) => this.mapDtoToModel(dto))
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
            activeLayerColor: dto.activeLayerColor,
            pointColor: dto.pointColor
        };
    }
}
