import { ChangeDetectionStrategy, Component, input, InputSignal, output, OutputEmitterRef } from '@angular/core';
import { RouterLink } from '@angular/router';
import { IPageHeaderOptions } from './interfaces/page-header-options.interface';

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
    public readonly options: InputSignal<IPageHeaderOptions | null> = input<IPageHeaderOptions | null>(null);
    public readonly logout: OutputEmitterRef<void> = output();
}
