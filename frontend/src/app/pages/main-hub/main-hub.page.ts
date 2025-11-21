
import { ChangeDetectionStrategy, Component, inject, signal, WritableSignal } from '@angular/core';
import { IMainHubCard } from './interfaces/main-hub-card.interface';
import { MainHubCardComponent } from './components/main-hub-card/main-hub-card.component';
import { MainHubService } from './services/main-hub.service';

@Component({
    selector: 'main-hub-page',
    standalone: true,
    templateUrl: './main-hub.page.html',
    styleUrl: './styles/main-hub.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        MainHubCardComponent
    ]
})
export class MainHubPageComponent {
    protected readonly analyticsMapCard: WritableSignal<IMainHubCard | undefined> = signal(undefined);
    protected readonly mapCardList: WritableSignal<IMainHubCard[]> = signal([]);

    private readonly _mainHubService: MainHubService = inject(MainHubService);

    constructor() {
        this.analyticsMapCard.set(this._mainHubService.getAnalyticsMapCard());
        this.mapCardList.set(this._mainHubService.getMapCardList());
    }

    /**
     * Редирект на страницу с картой
     * @param id
     */
    protected navigateToMap(id: string): void {
        this._mainHubService.navigateToMap(id);
    }
}
