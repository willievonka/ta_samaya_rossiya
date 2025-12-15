import { LatLng } from 'leaflet';

/**
 * Преобразование координат для корректного отображения в leaflet
 */
export function customCoordsToLatLng([lng, lat]: [number, number], reverseOrder?: boolean): LatLng {
    return reverseOrder
        ? new LatLng(lng, lat < 0 ? lat + 360 : lat)
        : new LatLng(lat, lng < 0 ? lng + 360 : lng);
}
