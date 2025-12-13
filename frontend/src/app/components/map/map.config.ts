import { IMapConfig } from './interfaces/map-config.interface';

export const mapConfig: IMapConfig = {
    options: {
        zoomControl: false,
        attributionControl: false,
        zoomSnap: 0.1,
        minZoom: 3,
        maxZoom: 7,
        center: [105, 72.5]
    },
    defaultLayerStyle: {
        fillColor: '#B4B4B4',
        fillOpacity: 1,
        color: '#FFF',
        weight: 1
    }
};
