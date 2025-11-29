import { MapOptions } from 'leaflet';

export interface IMapConfig {
    options: MapOptions;
    center: [number, number];
    initZoom: number;
}
