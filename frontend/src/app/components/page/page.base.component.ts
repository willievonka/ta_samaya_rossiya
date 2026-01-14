import { Directive, signal, WritableSignal } from '@angular/core';

@Directive({
    standalone: true
})
export class PageBaseComponent {
    protected readonly isLoading: WritableSignal<boolean> = signal(true);
}
