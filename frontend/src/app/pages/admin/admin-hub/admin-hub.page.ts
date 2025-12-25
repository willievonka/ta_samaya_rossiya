import { ChangeDetectionStrategy, Component, inject, signal, WritableSignal } from '@angular/core';
import { HubCardComponent } from '../../../components/hub-card/hub-card.component';
import { PageHeaderComponent } from '../../../components/page-header/page-header.component';
import { AuthService } from '../../../services/auth.service';
import { HubService } from '../../../services/hub.service';
import { Router } from '@angular/router';
import { IHubCard } from '../../../components/hub-card/interfaces/hub-card.interface';
import { take } from 'rxjs';

@Component({
    selector: 'admin-hub-page',
    standalone: true,
    templateUrl: './admin-hub.page.html',
    styleUrl: './styles/admin-hub-page.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        HubCardComponent,
        PageHeaderComponent
    ]
})
export class AdminHubPageComponent {
    protected readonly mapCardsList: WritableSignal<IHubCard[] | undefined> = signal(undefined);

    private readonly _authService: AuthService = inject(AuthService);
    private readonly _hubService: HubService = inject(HubService);
    private readonly _router: Router = inject(Router);

    constructor() {
        this.loadMapCardList();
    }

    /** Разлогин и выход в клиентское приложение */
    protected logout(): void {
        this._authService.logout();
        this._router.navigate(['/']);
    }

    /**
     * Редирект на страницу редактирования карты
     * @param card
     */
    protected navigateToEditMap(card: IHubCard): void {
        this._hubService.navigateToMap(card, true);
    }

    /** Загрузить список карточек карт */
    private loadMapCardList(): void {
        this._hubService.getMapCardsList()
            .pipe(take(1))
            .subscribe(list => this.mapCardsList.set(list));
    }
}
