import { Directive, inject, input, InputSignal, signal, WritableSignal } from '@angular/core';
import { IMapModel } from '../map/models/map.model';
import { IHubCard } from '../hub-card/interfaces/hub-card.interface';
import { AbstractControl, FormControl, FormGroup } from '@angular/forms';
import { FileService } from '../../services/file.service';
import { TuiFileLike } from '@taiga-ui/kit';
import { distinctUntilChanged, Observable, Subscription } from 'rxjs';
import { SafeStyle } from '@angular/platform-browser';
import { compareFiles } from '../../utils/compare-files.util';

@Directive()
export abstract class EditMapModalBaseComponent<
    TSettingsForm extends { [K in keyof TSettingsForm]: AbstractControl },
    TItemForm extends { [K in keyof TItemForm]: AbstractControl }
> {
    public readonly model: InputSignal<IMapModel> = input.required();
    public readonly card: InputSignal<IHubCard | null> = input.required();

    protected readonly settingsForm: FormGroup<TSettingsForm>;
    protected readonly editItemForm: FormGroup<TItemForm>;

    protected readonly isEditItemModalOpen: WritableSignal<boolean> = signal(false);
    protected readonly showEditingDeleteError: WritableSignal<boolean> = signal(false);

    protected readonly fileService: FileService = inject(FileService);

    protected editingItemName: string | null = null;

    private _revokePreview: (() => void) | null = null;

    constructor() {
        this.settingsForm = new FormGroup(this.buildSettingsForm());
        this.editItemForm = new FormGroup(this.buildEditItemForm());
    }

    protected abstract buildSettingsForm(): TSettingsForm;
    protected abstract buildEditItemForm(): TItemForm;

    /** Открыть модалку редактирования элемента карты */
    protected openEditItemModal(): void {
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
