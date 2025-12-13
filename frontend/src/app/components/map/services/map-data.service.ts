import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { IMapLayerProperties } from '../interfaces/map-layer.interface';
import { environment } from '../../../../environments';
import { IMapDto, IParsedMapDto } from '../dto/map.dto';

@Injectable({ providedIn: 'root' })
export class MapDataService {
    private readonly _http: HttpClient = inject(HttpClient);
    private readonly _apiUrl: string = environment.clientApiUrl;

    /**
     * Получить данные для карты по mapId
     * @param mapId
     */
    public getMapData(mapId: string): Observable<IParsedMapDto> {
        return this._http.get<IMapDto>(`${this._apiUrl}/maps`, { params: { mapId } })
            .pipe(
                map((data: IMapDto) => ({
                    pageTitle: data.pageTitle,
                    infoText: data.infoText,
                    layers: data.layers.features.map((feature) => ({
                        geoData: feature.geometry,
                        properties: feature.properties as IMapLayerProperties
                    }))
                }))
            );
    }
}
