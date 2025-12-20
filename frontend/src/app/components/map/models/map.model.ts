import { IMapLayer } from '../interfaces/map-layer.interface';

export interface IMapModel {
    pageTitle: string;
    infoText: string;
    layers: IMapLayer[];
    layerWithPointsColor?: string;
    pointColor?: string;
}
