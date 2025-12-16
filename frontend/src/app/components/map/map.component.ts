import { AfterViewInit, ChangeDetectionStrategy, Component, computed, ElementRef, inject, input, InputSignal, output, OutputEmitterRef, signal, Signal, viewChild, WritableSignal } from '@angular/core';
import { Map } from 'leaflet';
import { IMapLayer, IMapLayerProperties } from './interfaces/map-layer.interface';
import { IMapZoomActions } from './interfaces/map-zoom-actions.interface';
import { IMapPoint } from './interfaces/map-point.interface';
import { MapRenderService } from './services/map-render.service';

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
    public readonly layerWithPointsColor: InputSignal<string | undefined> = input();
    public readonly pointColor: InputSignal<string | undefined> = input();

    /** Outputs */
    public readonly regionSelected: OutputEmitterRef<IMapLayerProperties | null> = output();
    public readonly pointSelected: OutputEmitterRef<IMapPoint | null> = output();

    /** Public fields*/
    public readonly zoomActions: WritableSignal<IMapZoomActions | undefined> = signal(undefined);
    public readonly points: Signal<IMapPoint[]> = computed(() => {
        const layers: IMapLayer[] = this.layers();
        const all: IMapPoint[] = layers.flatMap(layer => layer.properties.points ?? []);

        return all.slice().sort((a, b) => a.year - b.year);
    });

    /** Private fields */
    private readonly _mapContainer: Signal<ElementRef<HTMLDivElement>> = viewChild.required('mapContainer');
    private readonly _renderService: MapRenderService = inject(MapRenderService);

    public ngAfterViewInit(): void {
        const container: HTMLDivElement = this._mapContainer().nativeElement;
        const renderer: MapRenderService = this._renderService;
        const mapInstance: Map = renderer.initMap(container, () => {
            renderer.resetActiveLayerSelection();
            this.regionSelected.emit(null);
        });

        this.zoomActions.set(renderer.getZoomActions(mapInstance));

        renderer.renderLayers(
            this.layers(),
            this.layerWithPointsColor(),
            (props) => this.regionSelected.emit(props)
        );
        renderer.renderPoints(this.points(), this.pointColor());
    }

    /** Снять выделение с активного слоя */
    public clearRegionSelection(): void {
        this._renderService.resetActiveLayerSelection();
    }
}
