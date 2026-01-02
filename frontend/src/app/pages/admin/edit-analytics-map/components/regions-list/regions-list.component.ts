import { ChangeDetectionStrategy, Component } from '@angular/core';
import { IMapLayerProperties } from '../../../../../components/map/interfaces/map-layer.interface';
import { TuiIcon, TuiError } from '@taiga-ui/core';
import { TuiAnimated } from '@taiga-ui/cdk/directives/animated';
import { MapItemsList } from '../../../../../components/edit-map-modal/components/map-items-list/map-items-list.base.component';

@Component({
    selector: 'regions-list',
    standalone: true,
    templateUrl: './regions-list.component.html',
    styleUrl: './styles/regions-list.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        TuiIcon,
        TuiError,
        TuiAnimated
    ]
})
export class RegionsListComponent extends MapItemsList<IMapLayerProperties> {}
