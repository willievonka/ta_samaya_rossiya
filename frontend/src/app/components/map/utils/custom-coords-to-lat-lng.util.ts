import { LatLng } from 'leaflet';

/**
 * Преобразование координат для корректного отображения в leaflet
 */
export function customCoordsToLatLng([lng, lat]: [number, number]): LatLng {
    return new LatLng(lat, lng < 0 ? lng + 360 : lng);
}
