import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'map-page',
    standalone: true,
    templateUrl: './map.page.html',
    styleUrl: './styles/map-page.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MapPageComponent {

}
