import { ChangeDetectionStrategy, Component, inject, Signal, signal } from '@angular/core';
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
    ],
    providers: [
        MainHubService
    ]
})
export class MainHubPageComponent {
    protected readonly mainHubService: MainHubService = inject(MainHubService);

    protected readonly mapCardList: Signal<IMainHubCard[]> = signal(
        this.mainHubService.getMapCardList()
    );

    /**
     * Редирект на страницу с картой
     * @param id
     */
    protected navigateToMap(id: string): void {
        this.mainHubService.navigateToMap(id);
    }
}
