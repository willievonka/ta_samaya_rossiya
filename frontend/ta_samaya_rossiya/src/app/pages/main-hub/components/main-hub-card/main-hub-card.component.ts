import { ChangeDetectionStrategy, Component, computed, input, InputSignal, Signal } from '@angular/core';
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
    public readonly isAnalyticsCard: InputSignal<boolean> = input(false);

    protected readonly isInfoState: Signal<boolean> = computed(() => {
        return false;
    });
}
