import { ChangeDetectionStrategy, Component, input, InputSignal } from '@angular/core';
import { IMapZoomActions } from '../map/interfaces/map-zoom-actions.interface';

@Component({
    selector: 'map-zoom',
    standalone: true,
    templateUrl: './map-zoom.component.html',
    styleUrl: './styles/map-zoom.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class MapZoomComponent {
    public readonly zoomActions: InputSignal<IMapZoomActions> = input.required();

    /** Увеличить зум */
    protected zoomIn(): void {
        this.zoomActions().zoomIn();
    }

    /** Уменьшить зум */
    protected zoomOut(): void {
        this.zoomActions().zoomOut();
    }

    /** Сбросить зум */
    protected resetZoom(): void {
        this.zoomActions().resetZoom();
    }
}
