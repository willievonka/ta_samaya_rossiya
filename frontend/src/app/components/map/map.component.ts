import { AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, input, InputSignal, output, OutputEmitterRef, signal, Signal, viewChild, WritableSignal } from '@angular/core';
import { divIcon, DivIcon, DomEvent, geoJSON, Layer, LeafletMouseEvent, map, Map, marker, Path, PathOptions } from 'leaflet';
import { IMapLayer, IMapLayerProperties } from './interfaces/map-layer.interface';
import { customCoordsToLatLng } from './utils/custom-coords-to-lat-lng.util';
import { IMapConfig } from './interfaces/map-config.interface';
import { makeBrighterColor } from './utils/make-brighter-color.util';
import { IActiveLeafletLayer } from './interfaces/leaflet-layer.interface';
import { Feature, GeoJsonProperties } from 'geojson';
import { IMapZoomActions } from './interfaces/map-zoom-actions.interface';
import { mapConfig } from './map.config';
import { IMapPoint } from './interfaces/map-point.interface';

@Component({
    selector: 'map',
    standalone: true,
    template: `<div class="map" #mapContainer></div>`,
    styleUrl: './styles/map.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class MapComponent implements AfterViewInit {
    /** Inputs */
    public readonly layers: InputSignal<IMapLayer[]> = input.required();
    public readonly activeLayerColor: InputSignal<string | undefined> = input();
    public readonly pointColor: InputSignal<string | undefined> = input();

    /** Outputs */
    public readonly regionSelected: OutputEmitterRef<IMapLayerProperties | null> = output();
    public readonly pointSelected: OutputEmitterRef<IMapPoint | null> = output();

    /** Public fields*/
    public readonly zoomActions: WritableSignal<IMapZoomActions | undefined> = signal(undefined);

    /** Private fields */
    private readonly _config: IMapConfig = mapConfig;
    private readonly _mapContainer: Signal<ElementRef<HTMLDivElement>> = viewChild.required('mapContainer');
    private _map: Map | null = null;
    private _activeLeafletLayer: Path | null = null;

    public ngAfterViewInit(): void {
        this.initMap();
        this.renderLayers(this.layers());
    }

    /** Снять выделение с активного слоя */
    public clearRegionSelection(): void {
        this.clearActiveLayer();
    }

    /** Инит карты */
    private initMap(): void {
        const container: HTMLDivElement = this._mapContainer().nativeElement;
        const config: IMapConfig = this._config;

        const mapInstance: Map = map(container, config.options)
            .setView(
                customCoordsToLatLng(config.options.center as [number, number]),
                config.options.minZoom
            );

        mapInstance.on({
            click: () => {
                this.clearActiveLayer();
                this.regionSelected.emit(null);
            }
        });

        this._map = mapInstance;
        this.setZoomActions(mapInstance);
    }

    /** Установить действия для зума */
    private setZoomActions(mapInstance: Map): void {
        const config: IMapConfig = this._config;

        const actions: IMapZoomActions = {
            zoomIn: () => mapInstance.zoomIn(),
            zoomOut: () => mapInstance.zoomOut(),
            resetZoom: () => mapInstance.setView(
                customCoordsToLatLng(config.options.center as [number, number]),
                config.options.minZoom,
                { animate: true }
            )
        };

        this.zoomActions.set(actions);
    }

    /**
     * Отрисовать слои карты
     * @param layers
     */
    private renderLayers(layers: IMapLayer[]): void {
        const mapInstance: Map | null = this._map;
        if (!mapInstance) {
            return;
        }

        layers.forEach(layer => {
            const properties: GeoJsonProperties = this.buildLayerProperties(layer);
            geoJSON(layer.geoData, properties)
                .addTo(mapInstance);

            const points: IMapPoint[] | undefined = layer.properties.points;
            if (points?.length) {
                points.forEach((point) => this.renderPoint(point, mapInstance));
            }
        });
    }

    /**
     * Отрисовать точку на карте
     * @param point
     */
    private renderPoint(point: IMapPoint, mapInstance: Map): void {
        const color: string = this.pointColor() || this._config.defaultPointOptions.color;
        const iconSize: number = this._config.defaultPointOptions.iconSize;
        const ancorSize: number = iconSize / 2;

        const icon: DivIcon = divIcon({
            className: 'map__point',
            html: `
                <div class="map__point-icon" style="background-color: ${color}">
                    1
                </div>
            `,
            iconSize: [iconSize, iconSize],
            iconAnchor: [ancorSize, ancorSize]
        });

        marker(customCoordsToLatLng(point.coordinates, true), { icon })
            .addTo(mapInstance);
    }

    /**
     * Получить свойства слоя
     * @param mapLayer
     */
    private buildLayerProperties(mapLayer: IMapLayer): GeoJsonProperties {
        const isActive: boolean = mapLayer.properties.isActive === true;
        const hasPoints: boolean = !!mapLayer.properties.points?.length;

        const layerStyle: PathOptions = this.createLayerStyle(
            mapLayer.properties.style,
            hasPoints
        );

        const properties: GeoJsonProperties = {
            ...mapLayer.properties,
            style: layerStyle,
            interactive: isActive,
            coordsToLatLng: customCoordsToLatLng,
            onEachFeature: (feature: Feature, leafletLayer: Layer): void => {
                if (!isActive) {
                    return;
                }

                const pathLayer: IActiveLeafletLayer = leafletLayer as IActiveLeafletLayer;
                pathLayer.originalStyle = { ...pathLayer.options };

                leafletLayer.on({
                    click: (event: LeafletMouseEvent) => {
                        DomEvent.stopPropagation(event);
                        this.selectRegion(mapLayer, pathLayer);
                    }
                });
            }
        };

        return properties;
    }

    /**
     * Создать стиль для слоя
     * @param currentLayerStyle
     * @param hasPoints
     */
    private createLayerStyle(currentLayerStyle: PathOptions | undefined, hasPoints: boolean): PathOptions {
        const defaultLayerStyle: PathOptions = this._config.defaultLayerStyle;
        const overrideColor: string | undefined = hasPoints ? this.activeLayerColor() : undefined;

        const resultStyle: PathOptions = {
            ...defaultLayerStyle,
            ...currentLayerStyle,
            ...(overrideColor ? { fillColor: overrideColor } : {})
        };

        return resultStyle;
    }

    /**
     * Выделить регион
     * @param mapLayer
     * @param leafletLayer
     */
    private selectRegion(mapLayer: IMapLayer, leafletLayer: IActiveLeafletLayer): void {
        this.regionSelected.emit(mapLayer.properties);
        this.applyActiveLayerStyle(leafletLayer);
        leafletLayer.bringToFront();
    }

    /**
     * Применить стиль активного слоя
     * @param leafletLayer
     */
    private applyActiveLayerStyle(leafletLayer: IActiveLeafletLayer): void {
        const previous: Path | null = this._activeLeafletLayer;
        if (previous && previous !== leafletLayer) {
            this.resetLayerStyle(previous);
        }

        const base: PathOptions = leafletLayer.originalStyle!;
        if (!leafletLayer.brighterColor) {
            leafletLayer.brighterColor = makeBrighterColor(base.fillColor!, 15);
        }

        leafletLayer.setStyle({
            ...base,
            weight: 3,
            fillColor: leafletLayer.brighterColor
        });

        const nativeElement: SVGElement | undefined = leafletLayer.getElement() as SVGElement;
        if (nativeElement) {
            nativeElement.style.filter = 'drop-shadow(0px 4px 4px rgba(0, 0, 0, 0.33))';
        }

        this._activeLeafletLayer = leafletLayer;
    }

    /**
     * Сбросить стиль слоя к оригинальному
     * @param leafletLayer
     */
    private resetLayerStyle(leafletLayer: IActiveLeafletLayer): void {
        if (!leafletLayer.originalStyle) {
            return;
        }

        leafletLayer.setStyle(leafletLayer.originalStyle);

        const nativeElement: SVGElement | undefined = leafletLayer.getElement() as SVGElement;
        if (nativeElement) {
            nativeElement.style.filter = '';
        }
    }

    /** Очистить активный слой */
    private clearActiveLayer(): void {
        if (!this._activeLeafletLayer) {
            return;
        }

        this.resetLayerStyle(this._activeLeafletLayer);
        this._activeLeafletLayer = null;
    }
}
