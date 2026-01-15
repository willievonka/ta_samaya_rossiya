import { IMapConfig } from '../interfaces/map-config.interface';

export const mapConfig: IMapConfig = {
    options: {
        zoomControl: false,
        attributionControl: false,
        zoomSnap: 0.1,
        minZoom: 3.2,
        maxZoom: 7,
        center: [82, 72]
    },
    defaultLayerStyle: {
        fillColor: '#B4B4B4',
        fillOpacity: 1,
        color: '#FFF',
        layerWithPointsColor: '#3E56D5',
        weight: 1
    },
    defaultPointOptions: {
        color: '#C2121F',
        iconSize: 42
    }
};
