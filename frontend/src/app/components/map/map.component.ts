import { AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, input, InputSignal, signal, Signal, viewChild, WritableSignal } from '@angular/core';
import { geoJSON, map, Map } from 'leaflet';
import { IMapLayer } from '../../interfaces/map-layer.interface';
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

        this._map.set(
            map(container, this.config().options)
                .setView(
                    customCoordsToLatLng(this.config().center),
                    this.config().initZoom
                )
        );
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
            geoJSON(
                layer.geoData,
                {
                    style: layer.style,
                    coordsToLatLng: customCoordsToLatLng,
                }
            ).addTo(mapInstance);
        });
    }
}
