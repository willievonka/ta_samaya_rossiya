import { Injectable, signal, WritableSignal } from '@angular/core';
import { divIcon, DivIcon, Marker, marker, LeafletMouseEvent, DomEvent, DomUtil, Map as LeafletMap } from 'leaflet';
import { IDefaultMapPointOptions, IMapPoint } from '../interfaces/map-point.interface';
import { customCoordsToLatLng } from '../utils/custom-coords-to-lat-lng.util';
import { mapConfig } from '../configs/map.config';

@Injectable()
export class MapPointRenderService {
    private readonly _activeMarker: WritableSignal<Marker | null> = signal<Marker | null>(null);
    private readonly _activeMarkerId: WritableSignal<string | null> = signal<string | null>(null);
    private readonly _markersByPointId: Map<string, Marker> = new Map<string, Marker>();
    private readonly _markerBaseZIndex: Map<string, number> = new Map<string, number>();
    private readonly _defaultPointOptions: IDefaultMapPointOptions = mapConfig.defaultPointOptions;
    private _markers: Marker[] = [];

    /**
     * Отрисовать массив точек на карте
     * @param mapInstance
     * @param points
     * @param pointColor
     * @param onPointSelected
     */
    public renderPoints(
        mapInstance: LeafletMap,
        points: IMapPoint[],
        pointColor: string | undefined,
        onPointSelected?: (point: IMapPoint) => void,
        isReadonly?: boolean
    ): void {
        this.clearPoints();

        points.forEach((point, index) =>
            this.renderSinglePoint(mapInstance, point, index, pointColor, onPointSelected, isReadonly)
        );
    }

    /**
     * Сделать точку активной по id
     * @param pointId
     * @param coordinates
     * @param mapInstance
     */
    public setActivePointById(pointId: string, coordinates: [number, number], mapInstance: LeafletMap): void {
        const pointMarker: Marker | undefined = this._markersByPointId.get(pointId);
        if (!pointMarker) {
            return;
        }

        this.applyActiveStyle(pointMarker, pointId);
        this.panToPoint(mapInstance, coordinates);
    }

    /** Снять выделение активной точки */
    public resetActiveSelection(): void {
        const currentMarker: Marker | null = this._activeMarker();
        const currentId: string | null = this._activeMarkerId();

        if (currentMarker) {
            const element: HTMLElement | undefined = currentMarker.getElement();
            if (element) {
                DomUtil.removeClass(element, 'map__point_is-active');
            }

            const originalZIndex: number = currentId ? (this._markerBaseZIndex.get(currentId) ?? 0) : 0;
            currentMarker.setZIndexOffset(originalZIndex);
        }

        this._activeMarker.set(null);
        this._activeMarkerId.set(null);
    }

    /** Очистить все точки */
    private clearPoints(): void {
        this._markersByPointId.clear();
        this._markerBaseZIndex.clear();
        this._activeMarker.set(null);
        this._activeMarkerId.set(null);
        this._markers.forEach(m => m.remove());
        this._markers = [];
    }

    /**
     * Отрисовать точку на карте
     * @param mapInstance
     * @param point
     * @param index
     * @param pointColor
     * @param onPointSelected
     */
    private renderSinglePoint(
        mapInstance: LeafletMap,
        point: IMapPoint,
        index: number,
        pointColor: string | undefined,
        onPointSelected?: (point: IMapPoint) => void,
        isReadonly?: boolean
    ): void {
        const color: string = pointColor || this._defaultPointOptions.color;
        const iconSize: number = this._defaultPointOptions.iconSize;
        const icon: DivIcon = this.createPointIcon(color, index, iconSize);

        const pointMarker: Marker = marker(customCoordsToLatLng(point.coordinates, true), { icon })
            .setZIndexOffset(-index)
            .addTo(mapInstance);

        this._markers.push(pointMarker);

        if (!isReadonly) {
            pointMarker.on({
                click: (event: LeafletMouseEvent) => {
                    DomEvent.stopPropagation(event);
                    onPointSelected?.(point);
                    this.applyActiveStyle(pointMarker, point.id);
                    this.panToPoint(mapInstance, point.coordinates);
                }
            });
        }

        this._markerBaseZIndex.set(point.id, -index);
        this._markersByPointId.set(point.id, pointMarker);
    }

    /**
     * Создать иконку-маркер для точки
     * @param color
     * @param index
     * @param iconSize
     */
    private createPointIcon(color: string, index: number, iconSize: number): DivIcon {
        const anchorSize: number = iconSize / 2;

        return divIcon({
            className: 'map__point',
            html: `
                <div class="map__point-icon" style="background-color: ${color}">
                    ${index + 1}
                </div>
            `,
            iconSize: [iconSize, iconSize],
            iconAnchor: [anchorSize, anchorSize]
        });
    }

    /**
     * Применить стиль активной точки
     * @param pointMarker
     * @param pointId
     */
    private applyActiveStyle(pointMarker: Marker, pointId: string): void {
        const currentId: string | null = this._activeMarkerId();
        const currentMarker: Marker | null = currentId ? this._activeMarker() : null;

        if (currentId && currentMarker && currentMarker !== pointMarker) {
            const currentElement: HTMLElement | undefined = currentMarker.getElement();
            if (currentElement) {
                DomUtil.removeClass(currentElement, 'map__point_is-active');
            }
            currentMarker.setZIndexOffset(this._markerBaseZIndex.get(currentId) ?? 0);
        }

        const element: HTMLElement | undefined = pointMarker.getElement();
        if (element) {
            DomUtil.addClass(element, 'map__point_is-active');
        }

        pointMarker.setZIndexOffset(10000);
        this._activeMarker.set(pointMarker);
        this._activeMarkerId.set(pointId);
    }

    /**
     * Панарамировать карту к точке
     * @param mapInstance
     * @param coordinates
     */
    private panToPoint(mapInstance: LeafletMap, coordinates: [number, number]): void {
        mapInstance.panTo(
            customCoordsToLatLng(coordinates, true),
            { animate: true, duration: 0.75 }
        );
    }
}
