import { computed, inject, Injectable, signal, Signal, WritableSignal } from '@angular/core';
import { map, Map as LeafletMap } from 'leaflet';
import { IMapConfig } from '../interfaces/map-config.interface';
import { IMapZoomActions } from '../interfaces/map-zoom-actions.interface';
import { IMapLayer, IMapLayerProperties } from '../interfaces/map-layer.interface';
import { IMapPoint } from '../interfaces/map-point.interface';
import { mapConfig } from '../configs/map.config';
import { customCoordsToLatLng } from '../utils/custom-coords-to-lat-lng.util';
import { MapLayerRenderService } from './map-layer-render.service';
import { MapPointRenderService } from './map-point-render.service';

@Injectable()
export class MapRenderService {
    public readonly mapInstance: Signal<LeafletMap | null> = computed(() => this._mapInstance());

    private _config: IMapConfig = mapConfig;
    private readonly _mapInstance: WritableSignal<LeafletMap | null> = signal<LeafletMap | null>(null);
    private readonly _layerRenderer: MapLayerRenderService = inject(MapLayerRenderService);
    private readonly _pointRenderer: MapPointRenderService = inject(MapPointRenderService);

    /**
     * Инициализировать карту
     * @param container
     * @param onMapClick
     */
    public initMap(container: HTMLDivElement, onMapClick: () => void, config?: IMapConfig,): LeafletMap {
        if (config) {
            this._config = config;
        }
        const { options }: IMapConfig = this._config;

        const instance: LeafletMap = map(container, options)
            .setView(
                customCoordsToLatLng(options.center as [number, number]),
                options.minZoom
            );

        instance.on({ click: onMapClick });
        this._mapInstance.set(instance);

        return instance;
    }

    /**
     * Получить действия для зума
     * @param mapInstance
     */
    public getZoomActions(mapInstance: LeafletMap): IMapZoomActions {
        const { options }: IMapConfig = this._config;

        return {
            zoomIn: () => mapInstance.zoomIn(),
            zoomOut: () => mapInstance.zoomOut(),
            resetZoom: () => mapInstance.setView(
                customCoordsToLatLng(options.center as [number, number]),
                options.minZoom,
                { animate: true }
            )
        };
    }

    /**
     * Отрисовать слои
     * @param layers
     * @param layerWithPointsColor
     * @param onLayerSelected
     */
    public renderLayers(
        layers: IMapLayer[],
        layerWithPointsColor: string | undefined,
        onLayerSelected: (props: IMapLayerProperties) => void,
        isReadonly?: boolean
    ): void {
        const instance: LeafletMap | null = this._mapInstance();
        if (!instance) {
            return;
        }

        this._layerRenderer.renderLayers(
            instance,
            layers,
            layerWithPointsColor,
            onLayerSelected,
            isReadonly
        );
    }

    /**
     * Отрисовать точки
     * @param points
     * @param pointColor
     * @param onPointSelected
     */
    public renderPoints(
        points: IMapPoint[],
        pointColor: string | undefined,
        onPointSelected?: (point: IMapPoint) => void,
        isReadonly?: boolean
    ): void {
        const instance: LeafletMap | null = this._mapInstance();
        if (!instance) {
            return;
        }

        this._pointRenderer.renderPoints(
            instance,
            points,
            pointColor,
            onPointSelected,
            isReadonly
        );
    }

    /**
     * Установить активную точку по id
     * @param pointId
     * @param pointCoordinates
     */
    public setActivePointById(pointId: string, pointCoordinates: [number, number]): void {
        const instance: LeafletMap | null = this._mapInstance();
        if (!instance) {
            return;
        }

        this._pointRenderer.setActivePointById(pointId, pointCoordinates, instance);
    }

    /** Снять выделение с активного слоя */
    public resetActiveLayerSelection(): void {
        this._layerRenderer.resetActiveSelection();
    }

    /** Снять выделение с активной точки */
    public resetActivePointSelection(): void {
        this._pointRenderer.resetActiveSelection();
    }
}
