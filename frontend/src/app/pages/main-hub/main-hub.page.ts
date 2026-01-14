import { ChangeDetectionStrategy, Component } from '@angular/core';
import { HubCardComponent } from '../../components/hub-card/hub-card.component';
import { PageHeaderComponent } from '../../components/page-header/page-header.component';
import { IHubCard } from '../../components/hub-card/interfaces/hub-card.interface';
import { HubPageBaseComponent } from '../../components/hub-page/hub-page.base.component';

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
export class MainHubPageComponent extends HubPageBaseComponent {
    /**
     * Редирект на страницу карты
     * @param card
     */
    protected navigateToMap(card: IHubCard): void {
        this.hubService.navigateToMap(card);
    }
}
