import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal, WritableSignal } from '@angular/core';
import { IMainHubCard } from './interfaces/main-hub-card.interface';
import { MainHubCardComponent } from './components/main-hub-card/main-hub-card.component';
import { MainHubService } from './services/main-hub.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { tap } from 'rxjs';

@Component({
    selector: 'main-hub-page',
    standalone: true,
    templateUrl: './main-hub.page.html',
    styleUrl: './styles/main-hub.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        MainHubCardComponent,
        RouterLink
    ],
    providers: [
        MainHubService
    ]
})
export class MainHubPageComponent {
    protected readonly mapCardList: WritableSignal<IMainHubCard[] | undefined> = signal(undefined);

    private readonly _mainHubService: MainHubService = inject(MainHubService);
    private readonly _destroyRef: DestroyRef = inject(DestroyRef);

    constructor() {
        this.loadMapCardList();
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
