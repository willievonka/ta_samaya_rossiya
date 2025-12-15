import { IMapLayer } from '../interfaces/map-layer.interface';

export interface IMapModel {
    pageTitle: string;
    infoText: string;
    layers: IMapLayer[];
    activeLayerColor?: string;
    pointColor?: string;
}
