import { ChangeDetectionStrategy, Component, inject, signal, WritableSignal } from '@angular/core';
import { HubCardComponent } from '../../components/hub-card/hub-card.component';
import { PageHeaderComponent } from '../../components/page-header/page-header.component';
import { IHubCard } from '../../components/hub-card/interfaces/hub-card.interface';
import { HubService } from '../../services/hub.service';
import { take } from 'rxjs';

@Component({
    selector: 'main-hub-page',
    standalone: true,
    templateUrl: './main-hub.page.html',
    styleUrl: './styles/main-hub-page.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        HubCardComponent,
        PageHeaderComponent
    ]
})
export class MainHubPageComponent {
    protected readonly mapCardsList: WritableSignal<IHubCard[] | undefined> = signal(undefined);
    private readonly _hubService: HubService = inject(HubService);

    constructor() {
        this.loadMapCardList();
    }

    /**
     * Редирект на страницу карты
     * @param card
     */
    protected navigateToMap(card: IHubCard): void {
        this._hubService.navigateToMap(card);
    }

    /** Загрузить список карточек карт */
    private loadMapCardList(): void {
        this._hubService.getMapCardsList()
            .pipe(take(1))
            .subscribe(list => this.mapCardsList.set(list));
    }
}
