import { ChangeDetectionStrategy, Component, Signal, signal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-root',
    imports: [
        RouterOutlet,
        RouterLink
    ],
    templateUrl: './app.html',
    styleUrl: './app.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class App {
    protected readonly title: Signal<string> = signal('Интерактивные карты России');
}
