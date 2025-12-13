import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MapPageBaseComponent } from '../../components/map-page/map.page.base.component';
import { PageHeaderComponent } from '../../components/page-header/page-header.component';
import { MapComponent } from '../../components/map/map.component';
import { MapZoomComponent } from '../../components/map-zoom/map-zoom.component';
import { MapInfoComponent } from '../../components/map-info/map-info.component';

@Component({
    selector: 'map-page',
    standalone: true,
    templateUrl: './map.page.html',
    styleUrl: './styles/map-page.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        PageHeaderComponent,
        MapComponent,
        MapZoomComponent,
        MapInfoComponent
    ]
})
export class MapPageComponent extends MapPageBaseComponent {

}
