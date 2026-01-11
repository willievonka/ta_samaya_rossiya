export interface IMapPoint {
    id: string;
    title: string;
    coordinates: [number, number];
    year: number;
    imagePath: string;
    image?: File | null;
    description: string;
    excursionUrl: string;
    regionName?: string;
}

export interface IDefaultMapPointOptions {
    color: string;
    iconSize: number;
}
