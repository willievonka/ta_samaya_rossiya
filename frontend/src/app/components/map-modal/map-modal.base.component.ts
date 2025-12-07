import { Directive, input, InputSignal, output, OutputEmitterRef } from '@angular/core';
import { IMapLayerProperties } from '../map/interfaces/map-layer.interface';

@Directive({
    standalone: true
})
export class MapModalBaseComponent {
    public readonly data: InputSignal<IMapLayerProperties> = input.required();
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
