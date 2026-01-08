import { computed, Directive, effect, inject, Injector, signal, Signal, viewChild, WritableSignal } from '@angular/core';
import { IMapZoomActions } from '../map/interfaces/map-zoom-actions.interface';
import { MapComponent } from '../map/map.component';
import { ActivatedRoute } from '@angular/router';
import { MapDataService } from '../../services/map-data.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { IMapModel } from '../map/models/map.model';
import { distinctUntilChanged, map, of, switchMap } from 'rxjs';
import { IPageHeaderOptions } from '../page-header/interfaces/page-header-options.interface';
import { IHubCard } from '../hub-card/interfaces/hub-card.interface';
import { HubService } from '../../services/hub.service';

@Directive({
    standalone: true
})
export abstract class MapPageBaseComponent<TData> {
    protected readonly activeItem: WritableSignal<TData | null> = signal(null);
    protected readonly mapInstance: Signal<MapComponent | undefined> = viewChild(MapComponent);
    protected readonly zoomActions: Signal<IMapZoomActions | undefined> = computed(() => this.mapInstance()?.zoomActions());
    protected readonly model: WritableSignal<IMapModel | undefined> = signal(undefined);
    protected readonly card: Signal<IHubCard | null> = computed(() => this._cardFromRoute());
    protected readonly headerOptions: IPageHeaderOptions = {
        isDetached: true
    };

    private readonly _mapDataService: MapDataService = inject(MapDataService);
    private readonly _hubService: HubService = inject(HubService);
    private readonly _route: ActivatedRoute = inject(ActivatedRoute);
    private readonly _injector: Injector = inject(Injector);

    private readonly _mapData: Signal<IMapModel | undefined> = toSignal(
        this._route.url.pipe(
            switchMap(segments => {
                const isCreateRoute: boolean = segments.some(s => s.path === 'create-map');
                const mapId: string = this._route.snapshot.queryParamMap.get('id') ?? '';

                return this._mapDataService.getMapData(mapId, isCreateRoute);
            })
        ),
        { initialValue: undefined, injector: this._injector }
    );

    private readonly _isEditRoute: Signal<boolean> = toSignal(
        this._route.url.pipe(
            map(segments => segments.some(s => s.path === 'admin')),
            distinctUntilChanged()
        ),
        { initialValue: false, injector: this._injector },
    );

    private readonly _cardFromRoute: Signal<IHubCard | null> = toSignal(
        this._route.queryParamMap
            .pipe(
                map((paramMap) => paramMap.get('id') ?? ''),
                distinctUntilChanged(),
                switchMap((id) => {
                    if (!this._isEditRoute()) {
                        return of(null);
                    }

                    const fromState: IHubCard | undefined = history.state?.card as IHubCard | undefined;
                    if (fromState?.id === id) {
                        return of(fromState);
                    }

                    return this._hubService.getMapCardById(id);
                })
            ),
        { initialValue: null, injector: this._injector }
    );

    constructor() {
        effect(() => {
            this.model.set(this._mapData());
        });
    }
}
