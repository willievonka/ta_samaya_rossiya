import { MapOptions, PathOptions } from 'leaflet';

export interface IMapConfig {
    options: MapOptions;
    defaultLayerStyle: PathOptions;
}
