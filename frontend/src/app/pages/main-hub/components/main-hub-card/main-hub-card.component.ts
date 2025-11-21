import { ChangeDetectionStrategy, Component, input, InputSignal, signal, WritableSignal } from '@angular/core';
import { IMainHubCard } from '../../interfaces/main-hub-card.interface';
import { CommonModule } from '@angular/common';
import { Base64ToUrlPipe } from '../../../../utils/base64-to-url.pipe';

@Component({
    selector: 'main-hub-card',
    standalone: true,
    templateUrl: './main-hub-card.component.html',
    styleUrl: './styles/main-hub-card.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        CommonModule,
        Base64ToUrlPipe
    ]
})
export class MainHubCardComponent {
    public readonly data: InputSignal<IMainHubCard> = input.required();
    protected readonly isInfoState: WritableSignal<boolean> = signal(false);
}
