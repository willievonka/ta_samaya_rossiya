import { ChangeDetectionStrategy, Component, input, InputSignal } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'page-header',
    standalone: true,
    templateUrl: './page-header.component.html',
    styleUrl: './styles/page-header.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterLink]
})
export class PageHeaderComponent {
    public readonly title: InputSignal<string> = input.required();
    public readonly isDetached: InputSignal<boolean> = input(false);
}
