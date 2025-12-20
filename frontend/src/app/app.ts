import { AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet],
    templateUrl: './app.html',
    styleUrl: './app.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class App implements AfterViewInit {
    private readonly _originalHeight: number = 952;
    private readonly _host: ElementRef<HTMLElement> = inject(ElementRef<HTMLElement>);

    public ngAfterViewInit(): void {
        this.applyZoom();
    }

    /** Применить зум к хосту */
    private applyZoom(): void {
        const viewportHeight: number = document.documentElement.clientHeight;
        const zoomNumber: number = Math.min(1, viewportHeight / this._originalHeight);
        this._host.nativeElement.style.zoom = String(zoomNumber);
        this._host.nativeElement.style.height = `${viewportHeight / zoomNumber}px`;
    }
}
