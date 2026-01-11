import { computed, DestroyRef, Directive, inject, input, InputSignal, OnInit, output, OutputEmitterRef, Signal, signal, WritableSignal } from '@angular/core';
import { IMapModel } from '../map/models/map.model';
import { IHubCard } from '../hub-card/interfaces/hub-card.interface';
import { AbstractControl, FormControl, FormGroup, ɵFormGroupRawValue } from '@angular/forms';
import { FileService } from '../../services/file.service';
import { TuiFileLike } from '@taiga-ui/kit';
import { distinctUntilChanged, merge, Observable, Subscription } from 'rxjs';
import { SafeStyle } from '@angular/platform-browser';
import { compareFiles } from '../../utils/compare-files.util';
import { IMapLayerProperties } from '../map/interfaces/map-layer.interface';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Directive()
export abstract class EditMapModalBaseComponent<
    TSettingsForm extends { [K in keyof TSettingsForm]: AbstractControl },
    TItemForm extends { [K in keyof TItemForm]: AbstractControl },
    TItem
> implements OnInit {
    public readonly model: InputSignal<IMapModel> = input.required();
    public readonly card: InputSignal<IHubCard | null> = input.required();
    public readonly isSaving: InputSignal<boolean> = input.required();

    public readonly dirtyChange: OutputEmitterRef<boolean> = output();
    public readonly activeItemsChanged: OutputEmitterRef<TItem[]> = output();
    public readonly mapSaved: OutputEmitterRef<ɵFormGroupRawValue<TSettingsForm>> = output();

    protected readonly settingsForm: FormGroup<TSettingsForm>;
    protected readonly editItemForm: FormGroup<TItemForm>;

    protected readonly allRegions: WritableSignal<IMapLayerProperties[]> = signal([]);
    protected readonly allRegionsNames: Signal<string[]> = computed(() => this.allRegions().map(r => r.regionName));
    protected readonly activeItems: WritableSignal<TItem[]> = signal([]);

    protected readonly isEditItemModalOpen: WritableSignal<boolean> = signal(false);
    protected readonly showEditingDeleteError: WritableSignal<boolean> = signal(false);

    protected readonly fileService: FileService = inject(FileService);
    protected readonly destroyRef: DestroyRef = inject(DestroyRef);

    protected editingItemName: string | null = null;

    private _revokePreview: (() => void) | null = null;

    constructor() {
        this.settingsForm = new FormGroup(this.buildSettingsForm());
        this.editItemForm = new FormGroup(this.buildEditItemForm());
    }

    protected abstract buildSettingsForm(): TSettingsForm;
    protected abstract buildEditItemForm(): TItemForm;

    public ngOnInit(): void {
        merge(
            this.settingsForm.valueChanges,
            this.editItemForm.valueChanges
        )
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(() =>
                this.dirtyChange.emit(
                    this.settingsForm.dirty || this.editItemForm.dirty
                )
            );
        this.allRegions.set(this.getAllRegions());
    }

    /** Сохранить карту */
    protected saveMap(): void {
        this.settingsForm.markAllAsTouched();
        this.settingsForm.updateValueAndValidity();
        if (this.settingsForm.invalid) {
            return;
        }

        this.mapSaved.emit(this.settingsForm.getRawValue());
    }

    /** Открыть модалку редактирования элемента карты */
    protected openEditItemModal(): void {
        if (this.isEditItemModalOpen()) {
            this.showEditingDeleteError.set(true);

            return;
        }

        this.editingItemName = null;
        this.isEditItemModalOpen.set(true);
    }

    /** Закрыть модалку редактирования элемента карты */
    protected closeEditItemModal(): void {
        this.editItemForm.reset();
        this.showEditingDeleteError.set(false);
        this.isEditItemModalOpen.set(false);
    }

    /**
     * Создать поток с превью изображения
     * @param control
     */
    protected createImagePreview(
        control: FormControl<TuiFileLike | null>,
    ): Observable<SafeStyle | null> {
        return new Observable<SafeStyle | null>(subscriber => {
            if (control.value) {
                subscriber.next(this.buildBgStyle(control.value));
            }

            const sub: Subscription = control.valueChanges
                .pipe(distinctUntilChanged(compareFiles))
                .subscribe(file => subscriber.next(this.buildBgStyle(file)));

            return () => {
                sub.unsubscribe();
                this._revokePreview?.();
                this._revokePreview = null;
            };
        });
    }

    /**
     * Получить все регионы и их свойства
     */
    protected getAllRegions(): IMapLayerProperties[] {
        const model: IMapModel = this.model();

        return this.sortRegions(model.layers.map(layer => layer.properties));
    }

    /**
     * Отсортировать регионы по имени
     * @param list
     */
    protected sortRegions(list: IMapLayerProperties[]): IMapLayerProperties[] {
        return list.slice().sort((a, b) =>
            a.regionName.localeCompare(b.regionName, 'ru', { sensitivity: 'base' }),
        );
    }

    /**
     * Собрать стиль background-image по файлу
     * @param file
     */
    private buildBgStyle(file: TuiFileLike | null): SafeStyle | null {
        this._revokePreview?.();

        const { style, revoke }: {
            style: SafeStyle | null,
            revoke: () => void
        } = this.fileService.buildBackgroundImagePreview(file);

        this._revokePreview = revoke;

        return style;
    }
}
