import { PathOptions } from 'leaflet';

export interface IMapLayer {
    geoData: GeoJSON.FeatureCollection,
    style?: PathOptions
}
