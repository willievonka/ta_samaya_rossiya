import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'main-hub-page',
    standalone: true,
    templateUrl: './main-hub.page.html',
    styleUrl: './styles/main-hub.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class MainHubPageComponent {

}
