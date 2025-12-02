import { Path, PathOptions } from 'leaflet';

export interface IActiveLeafletLayer extends Path {
    originalStyle?: PathOptions;
    brighterColor?: string;
}
