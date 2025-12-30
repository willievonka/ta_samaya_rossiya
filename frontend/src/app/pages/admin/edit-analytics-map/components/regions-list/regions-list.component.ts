import { ChangeDetectionStrategy, Component, input, InputSignal, output, OutputEmitterRef } from '@angular/core';
import { IMapLayerProperties } from '../../../../../components/map/interfaces/map-layer.interface';
import { TuiIcon, TuiError } from '@taiga-ui/core';
import { TuiAnimated } from '@taiga-ui/cdk/directives/animated';

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
export class RegionsListComponent {
    public readonly regions: InputSignal<IMapLayerProperties[]> = input.required();
    public readonly editingRegionName: InputSignal<string | null> = input<string | null>(null);
    public readonly showEditingDeleteError: InputSignal<boolean> = input(false);
    public readonly currentFormRegionName: InputSignal<string> = input('');

    public readonly edit: OutputEmitterRef<IMapLayerProperties> = output();
    public readonly delete: OutputEmitterRef<IMapLayerProperties> = output();
}
