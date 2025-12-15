export interface IMapPoint {
    id: string;
    title: string;
    coordinates: [number, number];
    year: number;
    imagePath: string;
    description: string;
    excursionUrl: string;
}

export interface IDefaultMapPointOptions {
    color: string;
    iconSize: number;
}
