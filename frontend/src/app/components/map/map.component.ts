import { AfterViewInit, ChangeDetectionStrategy, Component, computed, ElementRef, inject, input, InputSignal, output, OutputEmitterRef, Signal, signal, viewChild, WritableSignal } from '@angular/core';
import { Map } from 'leaflet';
import { IMapLayer, IMapLayerProperties } from './interfaces/map-layer.interface';
import { IMapZoomActions } from './interfaces/map-zoom-actions.interface';
import { IMapPoint } from './interfaces/map-point.interface';
import { MapRenderService } from './services/map-render.service';
import { MapLayerRenderService } from './services/map-layer-render.service';
import { MapPointRenderService } from './services/map-point-render.service';
import { IMapConfig } from './interfaces/map-config.interface';

@Component({
    selector: 'map',
    standalone: true,
    template: `<div class="map" #mapContainer></div>`,
    styleUrl: './styles/map.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    providers: [
        MapRenderService,
        MapLayerRenderService,
        MapPointRenderService
    ]
})
export class MapComponent implements AfterViewInit {
    /** Inputs */
    public readonly layers: InputSignal<IMapLayer[]> = input.required<IMapLayer[]>();
    public readonly layerWithPointsColor: InputSignal<string | undefined> = input<string>();
    public readonly pointColor: InputSignal<string | undefined> = input<string>();
    public readonly config: InputSignal<IMapConfig | undefined> = input<IMapConfig>();

    /** Outputs */
    public readonly regionSelected: OutputEmitterRef<IMapLayerProperties | null> = output<IMapLayerProperties | null>();
    public readonly pointSelected: OutputEmitterRef<IMapPoint | null> = output<IMapPoint | null>();

    /** Public fields */
    public readonly zoomActions: WritableSignal<IMapZoomActions | undefined> = signal<IMapZoomActions | undefined>(undefined);
    public readonly points: Signal<IMapPoint[]> = computed(() => {
        const layers: IMapLayer[] = this.layers();
        const allPoints: IMapPoint[] = layers.flatMap(layer => layer.properties.points ?? []);

        return allPoints.sort((a, b) => a.year - b.year);
    });

    /** Private fields */
    private readonly _mapContainer: Signal<ElementRef<HTMLDivElement>> = viewChild.required<ElementRef<HTMLDivElement>>('mapContainer');
    private readonly _renderService: MapRenderService = inject(MapRenderService);

    public ngAfterViewInit(): void {
        const container: HTMLDivElement = this._mapContainer().nativeElement;
        const mapInstance: Map = this.initializeMap(container);

        this.zoomActions.set(this._renderService.getZoomActions(mapInstance));
        this.renderMapContent();
    }

    /** Снять выделение со слоя */
    public clearRegionSelection(): void {
        this._renderService.resetActiveLayerSelection();
    }

    /**
     * Установить выделение точки
     * @param point
     */
    public setPointSelection(point: IMapPoint): void {
        this._renderService.setActivePointById(point.id, point.coordinates);
    }

    /** Снять выделение с точки */
    public clearPointSelection(): void {
        this._renderService.resetActivePointSelection();
    }

    /**
     * Инициализировать карту
     * @param container
     */
    private initializeMap(container: HTMLDivElement): Map {
        return this._renderService.initMap(
            container,
            () => {
                this._renderService.resetActiveLayerSelection();
                this._renderService.resetActivePointSelection();
                this.regionSelected.emit(null);
                this.pointSelected.emit(null);
            },
            this.config()
        );
    }

    /** Отрисовать наполнение карты */
    private renderMapContent(): void {
        this._renderService.renderLayers(
            this.layers(),
            this.layerWithPointsColor(),
            (props) => this.regionSelected.emit(props)
        );

        this._renderService.renderPoints(
            this.points(),
            this.pointColor(),
            (point) => this.pointSelected.emit(point)
        );
    }
}
