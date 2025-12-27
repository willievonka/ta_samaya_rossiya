import { ChangeDetectionStrategy, Component, input, InputSignal, signal, WritableSignal } from '@angular/core';
import { IHubCard } from './interfaces/hub-card.interface';

@Component({
    selector: 'hub-card',
    standalone: true,
    templateUrl: './hub-card.component.html',
    styleUrl: './styles/hub-card.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HubCardComponent {
    public readonly data: InputSignal<IHubCard> = input.required();
    protected readonly isInfoState: WritableSignal<boolean> = signal(false);
}
