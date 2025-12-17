import { Directive, input, InputSignal, output, OutputEmitterRef } from '@angular/core';

@Directive({
    standalone: true
})
export abstract class MapModalBaseComponent<TData> {
    public readonly data: InputSignal<TData> = input.required();
    public readonly closeModal: OutputEmitterRef<void> = output();

    /**
     * Фоллбэк для картинок
     * @param event
     * @param fallbackImagePath
     */
    protected handleImageFallback(event: Event, fallbackImagePath: string): void {
        const img: HTMLImageElement = event.target as HTMLImageElement;
        img.src = fallbackImagePath;
    }
}
