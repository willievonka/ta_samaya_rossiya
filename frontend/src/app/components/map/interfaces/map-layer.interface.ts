import { Geometry } from 'geojson';
import { PathOptions } from 'leaflet';
import { IMapPoint } from './map-point.interface';

export interface IMapLayer {
    geoData: Geometry;
    properties: IMapLayerProperties;
}

export interface IMapLayerProperties {
    id: string;
    regionName: string;
    isActive?: boolean;
    style?: PathOptions;
    analyticsData?: IAnalyticsMapLayerProperties;
    points?: IMapPoint[];
}

export interface IAnalyticsMapLayerProperties {
    imagePath: string;
    partnersCount: number;
    excursionsCount: number;
    membersCount: number;
}
