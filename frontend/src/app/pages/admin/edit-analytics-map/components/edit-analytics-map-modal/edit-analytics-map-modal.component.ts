import { ChangeDetectionStrategy, Component, DestroyRef, inject, input, InputSignal, OnDestroy, OnInit, signal, WritableSignal } from '@angular/core';
import { EditMapModalBaseComponent } from '../../../../../components/edit-map-modal/edit-map-modal.base.component';
import { TuiAccordion } from '@taiga-ui/experimental';
import { TuiCell } from '@taiga-ui/layout';
import { TuiButton, TuiError, TuiIcon, TuiScrollbar, TuiTextfield } from '@taiga-ui/core';
import { FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { TuiFileLike, tuiValidationErrorsProvider } from '@taiga-ui/kit';
import { AsyncPipe } from '@angular/common';
import { ImageUploaderComponent } from '../../../../../components/image-uploader/image-uploader.component';
import { SafeStyle } from '@angular/platform-browser';
import { catchError, distinctUntilChanged, forkJoin, map, Observable, of, startWith, take, tap } from 'rxjs';
import { FormFieldComponent } from '../../../../../components/form-field/form-field.component';
import { IAnalyticsMapSettingsForm } from '../../interfaces/analytics-map-settings-form.interface';
import { IAddRegionForm } from '../../interfaces/add-region-form.interface';
import { AddRegionComponent } from '../add-region/add-region.component';
import { IMapLayerProperties } from '../../../../../components/map/interfaces/map-layer.interface';
import { IMapModel } from '../../../../../components/map/models/map.model';
import { IHubCard } from '../../../../../components/hub-card/interfaces/hub-card.interface';
import { FileService } from '../../../../../services/file.service';
import { TuiAnimated } from '@taiga-ui/cdk/directives/animated';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
    selector: 'edit-analytics-map-modal',
    standalone: true,
    templateUrl: './edit-analytics-map-modal.component.html',
    styleUrl: './styles/edit-analytics-map-modal.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        AsyncPipe,
        ReactiveFormsModule,
        TuiAccordion,
        TuiCell,
        TuiTextfield,
        TuiButton,
        TuiIcon,
        TuiScrollbar,
        TuiError,
        TuiAnimated,
        ImageUploaderComponent,
        FormFieldComponent,
        AddRegionComponent
    ],
    providers: [
        tuiValidationErrorsProvider({
            regionAlreadyExists: 'Регион уже добавлен в список'
        })
    ]
})
export class EditAnalyticsMapModalComponent extends EditMapModalBaseComponent implements OnInit, OnDestroy {
    public readonly model: InputSignal<IMapModel> = input.required();
    public readonly card: InputSignal<IHubCard | null> = input.required();

    protected readonly isModalOpen: WritableSignal<boolean> = signal(false);
    protected readonly allRegions: WritableSignal<string[]> = signal([]);
    protected readonly activeRegionsList: WritableSignal<IMapLayerProperties[]> = signal([]);
    protected readonly showEditingDeleteError: WritableSignal<boolean> = signal(false);

    protected readonly settingsForm: FormGroup<IAnalyticsMapSettingsForm> = new FormGroup<IAnalyticsMapSettingsForm>({
        title: new FormControl<string>('', { nonNullable: true, validators: [Validators.required] }),
        cardBackgroundImage: new FormControl<TuiFileLike | null>(null),
        cardDescription: new FormControl<string>('', { nonNullable: true }),
        mapInfo: new FormControl<string>('', { nonNullable: true })
    });

    protected readonly addRegionForm: FormGroup<IAddRegionForm> = new FormGroup<IAddRegionForm>({
        regionName: new FormControl<string>('', { nonNullable: true, validators: [Validators.required] }),
        image: new FormControl<TuiFileLike | null>(null),
        color: new FormControl<string>('', { nonNullable: true, validators: [Validators.required] }),
        partnersCount: new FormControl<number>(0, { nonNullable: true, validators: [Validators.required] }),
        excursionsCount: new FormControl<number>(0, { nonNullable: true, validators: [Validators.required] }),
        membersCount: new FormControl<number>(0, { nonNullable: true, validators: [Validators.required] })
    });

    protected readonly cardPreviewBackgroundImage$: Observable<SafeStyle | null> =
        this.settingsForm.controls.cardBackgroundImage.valueChanges
            .pipe(
                startWith(this.settingsForm.controls.cardBackgroundImage.value),
                map(file => file as File | null),
                distinctUntilChanged((a, b) =>
                    (a?.name === b?.name) && (a?.size === b?.size) && (a?.lastModified === b?.lastModified)
                ),
                map(file => this.buildBgStyle(file))
            );

    private _revokePreview: (() => void) | null = null;
    private _editingRegionName: string | null = null;

    private readonly _fileService: FileService = inject(FileService);
    private readonly _destroyRef: DestroyRef = inject(DestroyRef);

    public ngOnInit(): void {
        this.init();

        this.addRegionForm.controls.regionName.valueChanges
            .pipe(
                tap(() => this.clearControlError(this.addRegionForm.controls.regionName, 'regionAlreadyExists')),
                takeUntilDestroyed(this._destroyRef)
            )
            .subscribe();
    }

    public ngOnDestroy(): void {
        this._revokePreview?.();
        this._revokePreview = null;
    }

    /**
     * Открыть/закрыть модалку настроек региона
     * @param isOpen
     */
    protected toggleRegionSettingsModal(isOpen: boolean): void {
        this.isModalOpen.set(isOpen);

        if (isOpen) {
            this._editingRegionName = null;
        } else {
            this.addRegionForm.reset();
            this.showEditingDeleteError.set(false);
        }
    }

    /**
     * Сохранить активный регион в список
     * @param item
     */
    protected saveActiveRegionListItem(): void {
        const form: FormGroup<IAddRegionForm> = this.addRegionForm;
        form.updateValueAndValidity();
        form.markAllAsTouched();
        if (form.invalid) {
            return;
        }

        const controls: IAddRegionForm = form.controls;
        const regionName: string = controls.regionName.value.trim();

        if (!this._editingRegionName) {
            const exists: boolean = this.activeRegionsList().some(r => r.regionName.trim() === regionName);

            if (exists) {
                controls.regionName.setErrors({ regionAlreadyExists: true });
                controls.regionName.markAsTouched();

                return;
            }
        }

        const imageFile: File | null = controls.image.value as File | null;

        const nextItem: IMapLayerProperties = {
            regionName,
            isActive: true,
            style: {
                ...(this.activeRegionsList().find(r => r.regionName === this._editingRegionName)?.style ?? {}),
                fillColor: controls.color.value
            },
            analyticsData: {
                partnersCount: controls.partnersCount.value,
                excursionsCount: controls.excursionsCount.value,
                membersCount: controls.membersCount.value,
                imageFile,
                imagePath: ''
            }
        } as IMapLayerProperties;

        if (this._editingRegionName) {
            const prevName: string = this._editingRegionName;

            this.activeRegionsList.update(list =>
                list.map(item => item.regionName === prevName ? nextItem : item)
            );
        } else {
            this.activeRegionsList.update(list => [...list, nextItem]);
        }

        this._editingRegionName = null;
        this.toggleRegionSettingsModal(false);
    }

    /**
     * Редактировать активный регион из списка
     * @param item
     */
    protected editActiveRegionListItem(item: IMapLayerProperties): void {
        this.showEditingDeleteError.set(false);
        this._editingRegionName = item.regionName;

        this.addRegionForm.patchValue({
            regionName: item.regionName,
            color: item.style?.fillColor ?? '',
            partnersCount: item.analyticsData?.partnersCount ?? 0,
            excursionsCount: item.analyticsData?.excursionsCount ?? 0,
            membersCount: item.analyticsData?.membersCount ?? 0
        });

        const storedFile: File | null = (item.analyticsData?.imageFile as File | null) ?? null;
        if (storedFile) {
            this.addRegionForm.controls.image.setValue(storedFile);
            this.isModalOpen.set(true);

            return;
        }

        const imageUrl: string | undefined = item.analyticsData?.imagePath?.trim();
        if (imageUrl) {
            const cached: File | null = this._fileService.getCachedFileByUrl(imageUrl);
            this.addRegionForm.controls.image.setValue(cached);
        } else {
            this.addRegionForm.controls.image.setValue(null);
        }

        this.isModalOpen.set(true);
    }

    /**
     * Удалить активный регион из списка
     * @param item
     */
    protected deleteActiveRegionListItem(item: IMapLayerProperties): void {
        const editingName: string = this.addRegionForm.controls.regionName.value;
        if (editingName === item.regionName) {
            this.showEditingDeleteError.set(true);

            return;
        }

        const imageUrl: string | undefined = item.analyticsData?.imagePath?.trim();
        if (imageUrl) {
            this._fileService.removeCachedFileByUrl(imageUrl);
        }

        this.showEditingDeleteError.set(false);
        this.activeRegionsList.update(list =>
            list.filter(region => region.regionName !== item.regionName)
        );
    }

    /** Инициализация настроек */
    private init(): void {
        const model: IMapModel = this.model();

        const props: IMapLayerProperties[] = model.layers
            .map(l => l.properties)
            .slice()
            .sort((a, b) => a.regionName.localeCompare(b.regionName, 'ru', { sensitivity: 'base' }));

        this.allRegions.set(props.map(p => p.regionName));
        const active: IMapLayerProperties[] = props.filter(p => p.isActive);
        this.activeRegionsList.set(active);

        const settingsControls: IAnalyticsMapSettingsForm = this.settingsForm.controls;
        settingsControls.title.setValue(model.pageTitle);
        settingsControls.mapInfo.setValue(model.infoText);

        const card: IHubCard | null = this.card();
        settingsControls.cardDescription.setValue(card?.description ?? '');

        const preloadTasks: Array<Observable<unknown>> = [];

        const cardUrl: string | undefined = card?.backgroundImagePath?.trim();
        if (cardUrl) {
            const cardFileName: string = this._fileService.getFileNameFromUrl(cardUrl) ?? 'card-background.svg';

            preloadTasks.push(
                this._fileService.downloadAsFile(cardUrl, cardFileName, 'image/svg+xml').pipe(
                    take(1),
                    tap(file => settingsControls.cardBackgroundImage.setValue(file)),
                    catchError(() => of(null))
                )
            );
        }

        active.forEach(region => {
            const url: string | undefined = region.analyticsData?.imagePath?.trim();
            if (!url) {
                return;
            }

            const name: string = this._fileService.getFileNameFromUrl(url) ?? `${region.regionName}.png`;

            preloadTasks.push(
                this._fileService.downloadAsFile(url, name).pipe(
                    take(1),
                    catchError(() => of(null))
                )
            );
        });

        if (preloadTasks.length) {
            forkJoin(preloadTasks).pipe(take(1)).subscribe();
        }
    }

    /**
     * Собрать стиль background-image из файла
     * @param file
     */
    private buildBgStyle(file: TuiFileLike | null): SafeStyle | null {
        this._revokePreview?.();
        this._revokePreview = null;

        const { style, revoke }: { style: SafeStyle | null, revoke: () => void } = this._fileService.buildBackgroundImagePreview(file);
        this._revokePreview = revoke;

        return style;
    }

    /**
     * Убрать ошибку из контрола по ключу
     * @param control
     * @param key
     * @returns
     */
    private clearControlError(control: FormControl<string>, key: string): void {
        const errors: ValidationErrors | null = control.errors;
        if (!errors || !errors[key]) {
            return;
        }

        delete errors[key];

        control.setErrors(Object.keys(errors).length ? errors : null);
    }
}
