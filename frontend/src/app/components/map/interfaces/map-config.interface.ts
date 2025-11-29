import { MapOptions, PathOptions } from 'leaflet';

export interface IMapConfig {
    options: MapOptions;
    center: [number, number];
    initZoom: number;
    defaultLayerStyle: PathOptions;
}
