import { ChangeDetectionStrategy, Component, Signal, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet],
    templateUrl: './app.html',
    styleUrl: './app.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class App {
    protected readonly title: Signal<string> = signal('ta_samaya_rossiya');
}
