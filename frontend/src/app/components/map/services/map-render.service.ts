import { computed, Injectable, signal, Signal, WritableSignal } from '@angular/core';
import { IMapConfig } from '../interfaces/map-config.interface';
import { mapConfig } from '../map.config';
import { divIcon, DivIcon, DomEvent, geoJSON, Layer, LeafletMouseEvent, map, Map, marker, PathOptions } from 'leaflet';
import { customCoordsToLatLng } from '../utils/custom-coords-to-lat-lng.util';
import { IMapZoomActions } from '../interfaces/map-zoom-actions.interface';
import { IMapLayer, IMapLayerProperties } from '../interfaces/map-layer.interface';
import { Feature, GeoJsonProperties } from 'geojson';
import { IActiveLeafletLayer } from '../interfaces/leaflet-layer.interface';
import { makeBrighterColor } from '../utils/make-brighter-color.util';
import { IMapPoint } from '../interfaces/map-point.interface';

@Injectable()
export class MapRenderService {
    public readonly mapInstance: Signal<Map | null> = computed(() => this._map());

    private readonly _config: IMapConfig = mapConfig;
    private readonly _map: WritableSignal<Map | null> = signal(null);
    private readonly _activeLeafletLayer: WritableSignal<IActiveLeafletLayer | null> = signal(null);

    /** Public Methods */

    /**
     * Инит карты
     * @param container
     * @param onMapClick
     */
    public initMap(container: HTMLDivElement, onMapClick: () => void): Map {
        const config: IMapConfig = this._config;

        const mapInstance: Map = map(container, config.options)
            .setView(
                customCoordsToLatLng(config.options.center as [number, number]),
                config.options.minZoom
            );
        mapInstance.on({ click: onMapClick });
        this._map.set(mapInstance);

        return mapInstance;
    }

    /**
     * Получить действия для зума
     * @param mapInstance
     */
    public getZoomActions(mapInstance: Map): IMapZoomActions {
        const config: IMapConfig = this._config;

        return {
            zoomIn: () => mapInstance.zoomIn(),
            zoomOut: () => mapInstance.zoomOut(),
            resetZoom: () => mapInstance.setView(
                customCoordsToLatLng(config.options.center as [number, number]),
                config.options.minZoom,
                { animate: true }
            )
        };
    }

    /**
     * Отрисовать слои для карты
     * @param layers
     * @param layerWithPointsColor
     * @param onLayerSelected
     */
    public renderLayers(
        layers: IMapLayer[],
        layerWithPointsColor: string | undefined,
        onLayerSelected: (props: IMapLayerProperties) => void
    ): void {
        const mapInstance: Map | null = this._map();
        if (!mapInstance) {
            return;
        }

        layers.forEach((layer) => {
            const properties: GeoJsonProperties = this.getLayerProperties(
                layer,
                layerWithPointsColor,
                onLayerSelected
            );

            geoJSON(layer.geoData, properties).addTo(mapInstance);
        });
    }

    /**
     * Отрисовать массив точек
     * @param points
     * @param pointColor
     */
    public renderPoints(points: IMapPoint[], pointColor?: string): void {
        const mapInstance: Map | null = this._map();
        if (!mapInstance) {
            return;
        }

        points.forEach((point, index) => this.renderPoint(point, mapInstance, index, pointColor));
    }

    /** Сбросить выделение активного слоя */
    public resetActiveLayerSelection(): void {
        const currentActiveLayer: IActiveLeafletLayer | null = this._activeLeafletLayer();
        if (!currentActiveLayer) {
            return;
        }

        this.resetActiveLayerStyle(currentActiveLayer);
        this._activeLeafletLayer.set(null);
    }

    /** Private layer help methods */

    /**
     * Получить свойства слоя
     * @param mapLayer
     * @param layerWithPointsColor
     * @param onRegionSelected
     */
    private getLayerProperties(
        mapLayer: IMapLayer,
        layerWithPointsColor: string | undefined,
        onLayerSelected: (props: IMapLayerProperties) => void
    ): GeoJsonProperties {
        const isActive: boolean = !!mapLayer.properties.isActive;
        const hasPoints: boolean = !!mapLayer.properties.points?.length;

        const layerStyle: PathOptions = this.createLayerStyle(
            mapLayer.properties.style,
            hasPoints,
            layerWithPointsColor
        );

        const onLayerClick = (feature: Feature, leafletLayer: Layer): void => {
            if (!isActive) {
                return;
            }

            const pathLayer: IActiveLeafletLayer = leafletLayer as IActiveLeafletLayer;
            pathLayer.originalStyle = { ...pathLayer.options };

            leafletLayer.on({
                click: (event: LeafletMouseEvent) => {
                    DomEvent.stopPropagation(event);
                    onLayerSelected(mapLayer.properties);
                    this.applyActiveLayerStyle(pathLayer);
                    pathLayer.bringToFront();
                }
            });
        };

        return {
            ...mapLayer.properties,
            style: layerStyle,
            interactive: isActive,
            coordsToLatLng: customCoordsToLatLng,
            onEachFeature: onLayerClick
        };
    }

    /**
     * Создать и получить стиль слоя
     * @param originalStyle
     * @param hasPoints
     * @param layerWithPointsColor
     */
    private createLayerStyle(
        originalStyle: PathOptions | undefined,
        hasPoints?: boolean,
        layerWithPointsColor?: string
    ): PathOptions {
        const defaultStyle: PathOptions = this._config.defaultLayerStyle;
        const newColor: string | undefined = hasPoints ? layerWithPointsColor : undefined;

        return {
            ...defaultStyle,
            ...originalStyle,
            ...(newColor ? { fillColor: newColor } : {})
        };
    }

    /**
     * Применить стиль активного слоя
     * @param leafletLayer
     */
    private applyActiveLayerStyle(leafletLayer: IActiveLeafletLayer): void {
        const currentActiveLayer: IActiveLeafletLayer | null = this._activeLeafletLayer();
        if (currentActiveLayer && currentActiveLayer !== leafletLayer) {
            this.resetActiveLayerStyle(currentActiveLayer);
        }

        const originalStyle: PathOptions = leafletLayer.originalStyle!;
        if (!leafletLayer.brighterColor) {
            leafletLayer.brighterColor = makeBrighterColor(originalStyle.fillColor!, 15);
        }

        leafletLayer.setStyle({
            ...originalStyle,
            weight: (this._config.defaultLayerStyle.weight ?? 1) * 3,
            fillColor: leafletLayer.brighterColor
        });

        const nativeElement: SVGElement | undefined = leafletLayer.getElement() as SVGElement;
        if (nativeElement) {
            nativeElement.style.filter = 'drop-shadow(0px 4px 4px rgba(0, 0, 0, 0.33))';
        }

        this._activeLeafletLayer.set(leafletLayer);
    }

    /**
     * Сбросить стиль слоя с активного к оригинальному
     * @param leafletLayer
     */
    private resetActiveLayerStyle(leafletLayer: IActiveLeafletLayer): void {
        const originalStyle: PathOptions = leafletLayer.originalStyle ?? this._config.defaultLayerStyle;
        leafletLayer.setStyle(originalStyle);

        const nativeElement: SVGElement | undefined = leafletLayer.getElement() as SVGElement;
        if (nativeElement) {
            nativeElement.style.filter = '';
        }
    }

    /** Private point help methods */

    /**
     * Отрисовать точку
     * @param point
     * @param mapInstance
     * @param index
     * @param pointColor
     */
    private renderPoint(
        point: IMapPoint,
        mapInstance: Map,
        index: number,
        pointColor?: string
    ): void {
        const color: string = pointColor || this._config.defaultPointOptions.color;
        const iconSize: number = this._config.defaultPointOptions.iconSize;
        const ancorSize: number = iconSize / 2;

        const icon: DivIcon = divIcon({
            className: 'map__point',
            html: `
                <div class="map__point-icon" style="background-color: ${color}">
                ${index + 1}
                </div>
            `,
            iconSize: [iconSize, iconSize],
            iconAnchor: [ancorSize, ancorSize]
        });

        marker(customCoordsToLatLng(point.coordinates, true), { icon })
            .setZIndexOffset(-index)
            .addTo(mapInstance);
    }
}
