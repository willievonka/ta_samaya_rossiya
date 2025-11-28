import { AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, input, InputSignal, signal, Signal, viewChild, WritableSignal } from '@angular/core';
import { geoJSON, map, Map } from 'leaflet';
import { IMapLayer } from '../../interfaces/map-layer.interface';
import { customCoordsToLatLng } from './utils/custom-coords-to-lat-lng.util';

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

    protected readonly isLoading: WritableSignal<boolean> = signal(true);

    private readonly _map: WritableSignal<Map | null> = signal<Map | null>(null);
    private readonly _mapContainer: Signal<ElementRef<HTMLDivElement>> = viewChild.required('mapContainer');

    public ngAfterViewInit(): void {
        this.initMap();
        this.renderLayers(this.layers());
    }

    /**
     * Инит карты
     */
    private initMap(): void {
        const container: HTMLDivElement = this._mapContainer().nativeElement;
        const center: [number, number] = [105, 68.5];

        this._map.set(
            map(
                container,
                {
                    zoomControl: false,
                    attributionControl: false,
                    zoomSnap: 0.1
                }
            )
                .setView(customCoordsToLatLng(center), 3.2)
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

        setTimeout(() => {
            layers.forEach(layer => {
                geoJSON(layer.geoData, {
                    style: layer.style,
                    coordsToLatLng: customCoordsToLatLng,
                }).addTo(mapInstance);
            });

            this.isLoading.set(false);
        }, 2000);
    }
}
