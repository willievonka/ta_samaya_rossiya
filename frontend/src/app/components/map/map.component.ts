import { AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, input, InputSignal, output, OutputEmitterRef, signal, Signal, viewChild, WritableSignal } from '@angular/core';
import { DomEvent, geoJSON, Layer, LeafletMouseEvent, map, Map, Path, PathOptions } from 'leaflet';
import { IMapLayer, IMapLayerProperties } from './interfaces/map-layer.interface';
import { customCoordsToLatLng } from './utils/custom-coords-to-lat-lng.util';
import { IMapConfig } from './interfaces/map-config.interface';
import { makeBrighterColor } from './utils/make-brighter-color.util';
import { IActiveLeafletLayer } from './interfaces/leaflet-layer.interface';
import { Feature, GeoJsonProperties } from 'geojson';
import { IMapZoomActions } from './interfaces/map-zoom-actions.interface';
import { mapConfig } from './map.config';

@Component({
    selector: 'map',
    standalone: true,
    template: `
        @if(isLoading()) {
            <p>ISLOADING</p>
        }
        <div class="map" #mapContainer></div>
    `,
    styleUrl: './styles/map.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class MapComponent implements AfterViewInit {
    public readonly layers: InputSignal<IMapLayer[]> = input.required();

    public readonly zoomActions: WritableSignal<IMapZoomActions | undefined> = signal(undefined);
    public readonly regionSelected: OutputEmitterRef<IMapLayerProperties | null> = output();

    protected readonly isLoading: WritableSignal<boolean> = signal(true);

    private readonly _config: IMapConfig = mapConfig;
    private readonly _mapContainer: Signal<ElementRef<HTMLDivElement>> = viewChild.required('mapContainer');
    private _map: Map | null = null;
    private _activeLeaflerLayer: Path | null = null;

    public ngAfterViewInit(): void {
        this.initMap();
        this.renderLayers(this.layers());
        this.isLoading.set(false);
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
        this.setZoomActions();
    }

    /** Установить действия для зума */
    private setZoomActions(): void {
        const config: IMapConfig = this._config;
        const mapInstance: Map | null = this._map;
        if (!mapInstance) {
            return;
        }

        this.zoomActions.set({
            zoomIn: () => mapInstance.zoomIn(),
            zoomOut: () => mapInstance.zoomOut(),
            resetZoom: () => mapInstance.setView(
                customCoordsToLatLng(config.options.center as [number, number]),
                config.options.minZoom,
                { animate: true }
            )
        });
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
            const properties: GeoJsonProperties = this.getLayerProperties(layer);
            geoJSON(layer.geoData, properties)
                .addTo(mapInstance);
        });
    }

    /**
     * Получить свойства слоя
     * @param mapLayer
     */
    private getLayerProperties(mapLayer: IMapLayer): GeoJsonProperties {
        const isActive: boolean = mapLayer.properties.isActive === true;

        return {
            ...mapLayer.properties,
            style: {
                ...this._config.defaultLayerStyle,
                ...mapLayer.properties.style,
            },
            interactive: isActive,
            coordsToLatLng: customCoordsToLatLng,
            onEachFeature: (feature: Feature, leafletLayer: Layer): void => {
                if (isActive) {
                    const pathLayer: IActiveLeafletLayer = leafletLayer as IActiveLeafletLayer;
                    pathLayer.originalStyle = { ...pathLayer.options };

                    leafletLayer.on({
                        click: (event: LeafletMouseEvent) => {
                            DomEvent.stopPropagation(event);
                            this.selectRegion(mapLayer, pathLayer);
                        }
                    });
                }
            }
        };
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
        const prev: Path | null = this._activeLeaflerLayer;

        if (prev && prev !== leafletLayer) {
            this.resetLayerStyle(prev);
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

        this._activeLeaflerLayer = leafletLayer;
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
        if (this._activeLeaflerLayer) {
            this.resetLayerStyle(this._activeLeaflerLayer);
            this._activeLeaflerLayer = null;
        }
    }
}
