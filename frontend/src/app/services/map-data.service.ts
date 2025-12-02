import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { IMapLayer } from '../components/map/interfaces/map-layer.interface';
import { IMapLayerProperties } from '../components/map/interfaces/map-layer.interface';

@Injectable()
export class MapDataService {
    private readonly _http: HttpClient = inject(HttpClient);

    /**
     * Получить данные для аналитической карты
     */
    public getAnalyticsMapData(): Observable<IMapLayer[]> {
        return this._http.get<GeoJSON.FeatureCollection>('/maps/test.geojson')
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
