import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal, WritableSignal } from '@angular/core';
import { IMainHubCard } from './interfaces/main-hub-card.interface';
import { MainHubCardComponent } from './components/main-hub-card/main-hub-card.component';
import { MainHubService } from './services/main-hub.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { tap } from 'rxjs';

@Component({
    selector: 'main-hub-page',
    standalone: true,
    templateUrl: './main-hub.page.html',
    styleUrl: './styles/main-hub.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [MainHubCardComponent],
    providers: [MainHubService]
})
export class MainHubPageComponent {
    protected readonly mapCardList: WritableSignal<IMainHubCard[] | undefined> = signal(undefined);

    private readonly _mainHubService: MainHubService = inject(MainHubService);
    private readonly _destroyRef: DestroyRef = inject(DestroyRef);
    private readonly _router: Router = inject(Router);

    constructor() {
        this.loadMapCardList();
    }

    /**
     * Редирект на страницу карты
     * @param card
     */
    protected navigateToMap(card: IMainHubCard): void {
        const path: string = card.isAnalytics ? '/analytics-map' : 'map';
        this._router.navigate([path], {
            queryParams: { id: card.id }
        });
    }

    /** Загрузить список карточек карт */
    private loadMapCardList(): void {
        this._mainHubService.getMapCardList()
            .pipe(
                tap(list => this.mapCardList.set(list)),
                takeUntilDestroyed(this._destroyRef)
            )
            .subscribe();
    }
}
