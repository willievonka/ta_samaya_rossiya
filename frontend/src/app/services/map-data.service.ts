import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { IMapLayer } from '../interfaces/map-layer.interface';

@Injectable()
export class MapDataService {
    private readonly _http: HttpClient = inject(HttpClient);

    /**
     * Получить данные для аналитической карты
     */
    public getAnalyticsMapData(): Observable<IMapLayer[]> {
        return this._http.get<GeoJSON.FeatureCollection>('/maps/map.geojson')
            .pipe(
                map(geoData => [
                    {
                        geoData,
                        style: {
                            color: 'white',
                            fillColor: '#B4B4B4',
                            weight: 1,
                            fillOpacity: 1,
                        }
                    }
                ])
            );
    }
}
