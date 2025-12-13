import { FeatureCollection } from 'geojson';
import { IMapLayer } from '../interfaces/map-layer.interface';

export interface IMapDto {
    pageTitle: string;
    infoText: string;
    layers: FeatureCollection
}

export interface IParsedMapDto {
    pageTitle: IMapDto['pageTitle'];
    infoText: IMapDto['infoText'];
    layers: IMapLayer[]
}
