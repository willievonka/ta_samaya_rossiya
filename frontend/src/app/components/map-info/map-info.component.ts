import { ChangeDetectionStrategy, Component, input, InputSignal } from '@angular/core';

@Component({
    selector: 'map-info',
    standalone: true,
    templateUrl: './map-info.component.html',
    styleUrl: './styles/map-info.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class MapInfoComponent {
    public readonly text: InputSignal<string> = input.required();
}
