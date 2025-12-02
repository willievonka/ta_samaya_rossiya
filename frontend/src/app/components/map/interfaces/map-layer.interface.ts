import { PathOptions } from 'leaflet';

export interface IMapLayer {
    geoData: GeoJSON.Geometry;
    properties: IMapLayerProperties;
}

export interface IMapLayerProperties {
    id: string;
    regionName: string;
    isActive?: boolean;
    style?: PathOptions;
    analyticsData?: IAnalyticsMapLayerProperties;
}

export interface IAnalyticsMapLayerProperties {
    imagePath: string;
    partnersCount: number;
    excursionsCount: number;
    membersCount: number;
}
