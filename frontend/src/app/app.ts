import { TuiRoot } from '@taiga-ui/core';
import { AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
    selector: 'app-root',
    imports: [
        RouterOutlet,
        TuiRoot
    ],
    templateUrl: './app.html',
    styleUrl: './app.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class App implements OnInit, AfterViewInit {
    private readonly _originalHeight: number = 952;
    private readonly _host: ElementRef<HTMLElement> = inject(ElementRef<HTMLElement>);
    private readonly _authService: AuthService = inject(AuthService);

    public ngOnInit(): void {
        this._authService.initAuthTimer();
    }

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
