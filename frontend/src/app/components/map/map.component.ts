import { AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, input, InputSignal, output, OutputEmitterRef, signal, Signal, viewChild, WritableSignal } from '@angular/core';
import { geoJSON, Layer, map, Map } from 'leaflet';
import { IMapLayer, IMapLayerProperties } from './interfaces/map-layer.interface';
import { customCoordsToLatLng } from './utils/custom-coords-to-lat-lng.util';
import { IMapConfig } from './interfaces/map-config.interface';

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
    public readonly config: InputSignal<IMapConfig> = input.required();
    public readonly regionSelected: OutputEmitterRef<IMapLayerProperties> = output();

    protected readonly isLoading: WritableSignal<boolean> = signal(true);

    private readonly _map: WritableSignal<Map | null> = signal<Map | null>(null);
    private readonly _mapContainer: Signal<ElementRef<HTMLDivElement>> = viewChild.required('mapContainer');

    public ngAfterViewInit(): void {
        this.initMap();
        this.renderLayers(this.layers());
        this.isLoading.set(false);
    }

    /** Инит карты */
    private initMap(): void {
        const container: HTMLDivElement = this._mapContainer().nativeElement;
        const config: IMapConfig = this.config();

        const mapInstance: Map = map(container, config.options)
            .setView(
                customCoordsToLatLng(config.center),
                config.initZoom
            );

        this._map.set(mapInstance);
    }

    /**
     * Рендер слоев карты
     * @param layers
     */
    private renderLayers(layers: IMapLayer[]): void {
        const mapInstance: Map | null = this._map();
        if (!mapInstance) {
            return;
        }

        layers.forEach(layer => {
            const properties: GeoJSON.GeoJsonProperties = this.getLayerProperties(layer);
            geoJSON(layer.geoData, properties)
                .addTo(mapInstance);
        });
    }

    /**
     * Получить свойства слоя
     * @param layer
     */
    private getLayerProperties(layer: IMapLayer): GeoJSON.GeoJsonProperties {
        const isActive: boolean = layer.properties.isActive === true;

        return {
            ...layer.properties,
            style: {
                ...this.config().defaultLayerStyle,
                ...layer.properties.style,
            },
            interactive: isActive,
            coordsToLatLng: customCoordsToLatLng,
            onEachFeature: (feature: GeoJSON.Feature, leafletLayer: Layer): void => {
                if (isActive) {
                    leafletLayer.on('click', () => this.regionSelected.emit(layer.properties));
                }
            }
        };
    }
}
