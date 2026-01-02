import { ChangeDetectionStrategy, Component } from '@angular/core';
import { EditMapPageBaseComponent } from '../../../components/map-page/edit-map.page.base.component';
import { IMapPoint } from '../../../components/map/interfaces/map-point.interface';
import { PageHeaderComponent } from '../../../components/page-header/page-header.component';
import { MapComponent } from '../../../components/map/map.component';
import { MapZoomComponent } from '../../../components/map-zoom/map-zoom.component';

@Component({
    selector: 'edit-map',
    standalone: true,
    templateUrl: './edit-map.page.html',
    styleUrl: './styles/edit-map-page.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        PageHeaderComponent,
        MapComponent,
        MapZoomComponent
    ]
})
export class EditMapPageComponent extends EditMapPageBaseComponent<IMapPoint> {

}
