import { IMapLayerProperties } from '../interfaces/map-layer.interface';

export interface ISaveMapDto {
    isAnalytics: boolean;
    title: string;
    description: string;
    infoText: string;
    activeLayerColor?: string;
    pointColor?: string;
    backgroundImage?: File | null;
    layers: IMapLayerProperties[]
}
