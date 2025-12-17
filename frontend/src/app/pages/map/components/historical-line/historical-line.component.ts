import { ChangeDetectionStrategy, Component, effect, input, InputSignal, output, OutputEmitterRef, signal, WritableSignal } from '@angular/core';
import { IMapPoint } from '../../../../components/map/interfaces/map-point.interface';

@Component({
    selector: 'historical-line',
    standalone: true,
    templateUrl: './historical-line.component.html',
    styleUrl: './styles/historical-line.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HistoricalLineComponent {
    public readonly items: InputSignal<IMapPoint[]> = input.required();
    public readonly iconColor: InputSignal<string | undefined> = input();
    public readonly activeItem: InputSignal<IMapPoint | null> = input<IMapPoint | null>(null);

    public readonly itemSelected: OutputEmitterRef<IMapPoint> = output();

    protected readonly activeItemIndex: WritableSignal<number | null> = signal(null);

    constructor() {
        effect(() => {
            const active: IMapPoint | null = this.activeItem();
            const items: IMapPoint[] = this.items();

            if (!active) {
                this.activeItemIndex.set(null);

                return;
            }

            const index: number = items.findIndex((item) => item.id === active.id);
            this.activeItemIndex.set(index >= 0 ? index : null);
        });
    }

    /**
     * Сделать активным элемент по индексу
     * @param index
     */
    public setActiveItem(index: number): void {
        const item: IMapPoint = this.items()[index];
        this.itemSelected.emit(item);
        this.activeItemIndex.set(index);
    }

    /** Сбросить активный элемент */
    public resetActiveItem(): void {
        this.activeItemIndex.set(null);
    }
}
