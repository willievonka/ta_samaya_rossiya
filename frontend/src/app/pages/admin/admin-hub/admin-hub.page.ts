import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { HubCardComponent } from '../../../components/hub-card/hub-card.component';
import { PageHeaderComponent } from '../../../components/page-header/page-header.component';
import { AuthService } from '../../../services/auth.service';
import { Router } from '@angular/router';
import { IHubCard } from '../../../components/hub-card/interfaces/hub-card.interface';
import { take } from 'rxjs';
import { IPageHeaderOptions } from '../../../components/page-header/interfaces/page-header-options.interface';
import { HubPageBaseComponent } from '../../../components/hub-page/hub-page.base.component';

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
export class AdminHubPageComponent extends HubPageBaseComponent {
    protected readonly headerOptions: IPageHeaderOptions = {
        adminState: {
            changeRedirect: true,
            showLogoutIcon: true
        }
    };

    private readonly _authService: AuthService = inject(AuthService);
    private readonly _router: Router = inject(Router);

    /** Разлогин и выход в клиентское приложение */
    protected logout(): void {
        this._authService.logout()
            .pipe(take(1))
            .subscribe(() => this._router.navigate(['/']));
    }

    /**
     * Редирект на страницу редактирования карты
     * @param card
     */
    protected navigateToEditMap(card: IHubCard): void {
        this.hubService.navigateToMap(card, true);
    }

    /** Редирект на страницу создания карты */
    protected navigateToCreateMap(): void {
        this.hubService.navigateToCreateMap();
    }
}
