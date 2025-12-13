import { FeatureCollection } from 'geojson';

export interface IMapDto {
    pageTitle: string;
    infoText: string;
    layers: FeatureCollection;
    activeLayerColor?: string;
    pointColor?: string;
}
