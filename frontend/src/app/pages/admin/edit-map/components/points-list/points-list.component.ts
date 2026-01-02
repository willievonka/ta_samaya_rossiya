import { ChangeDetectionStrategy, Component, input, InputSignal } from '@angular/core';
import { IMapPoint } from '../../../../../components/map/interfaces/map-point.interface';
import { MapItemsList } from '../../../../../components/edit-map-modal/components/map-items-list.base.component';
import { TuiError, TuiIcon } from '@taiga-ui/core';
import { TuiAnimated } from '@taiga-ui/cdk/directives/animated';

@Component({
    selector: 'points-list',
    standalone: true,
    templateUrl: './points-list.component.html',
    styleUrl: './styles/points-list.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        TuiIcon,
        TuiError,
        TuiAnimated
    ]
})
export class PointsListComponent extends MapItemsList<IMapPoint> {
    public readonly iconColor: InputSignal<string> = input.required();
}
