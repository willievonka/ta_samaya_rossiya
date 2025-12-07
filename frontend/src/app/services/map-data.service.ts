import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { IMapLayer } from '../components/map/interfaces/map-layer.interface';
import { IMapLayerProperties } from '../components/map/interfaces/map-layer.interface';
import { environment } from '../../environments';

@Injectable()
export class MapDataService {
    private readonly _http: HttpClient = inject(HttpClient);
    private readonly _apiUrl: string = environment.clientApiUrl;

    /**
     * Получить данные для аналитической карты
     * @param mapId
     */
    public getAnalyticsMapData(mapId: string): Observable<IMapLayer[]> {
        return this._http.get<GeoJSON.FeatureCollection>(`${this._apiUrl}/maps`, { params: { mapId } })
            .pipe(
                map(({ features }: GeoJSON.FeatureCollection) =>
                    features.map(feature => ({
                        geoData: feature.geometry,
                        properties: feature.properties as IMapLayerProperties
                    }))
                )
            );
    }
}
