import { MapOptions, PathOptions } from 'leaflet';
import { IDefaultMapPointOptions } from './map-point.interface';

export interface IMapConfig {
    options: MapOptions;
    defaultLayerStyle: PathOptions & { layerWithPointsColor: string };
    defaultPointOptions: IDefaultMapPointOptions;
}
