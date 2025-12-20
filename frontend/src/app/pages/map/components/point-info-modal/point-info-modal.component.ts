import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MapModalBaseComponent } from '../../../../components/map-modal/map-modal.base.component';
import { IMapPoint } from '../../../../components/map/interfaces/map-point.interface';

@Component({
    selector: 'point-info-modal',
    standalone: true,
    templateUrl: './point-info-modal.component.html',
    styleUrl: './styles/point-info-modal.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class PointInfoModalComponent extends MapModalBaseComponent<IMapPoint> {}
