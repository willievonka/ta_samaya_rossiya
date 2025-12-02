import { ChangeDetectionStrategy, Component, input, InputSignal, signal, WritableSignal } from '@angular/core';
import { IMainHubCard } from '../../interfaces/main-hub-card.interface';

@Component({
    selector: 'main-hub-card',
    standalone: true,
    templateUrl: './main-hub-card.component.html',
    styleUrl: './styles/main-hub-card.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MainHubCardComponent {
    public readonly data: InputSignal<IMainHubCard> = input.required();
    protected readonly isInfoState: WritableSignal<boolean> = signal(false);
}
