import { ChangeDetectionStrategy, Component, DestroyRef, inject, input, InputSignal, OnDestroy, OnInit, signal, WritableSignal } from '@angular/core';
import { EditMapModalBaseComponent } from '../../../../../components/edit-map-modal/edit-map-modal.base.component';
import { TuiAccordion } from '@taiga-ui/experimental';
import { TuiCell } from '@taiga-ui/layout';
import { TuiButton, TuiScrollbar, TuiTextfield } from '@taiga-ui/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
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
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RegionsListComponent } from '../regions-list/regions-list.component';
import { clearControlError } from '../../../../../utils/clear-control-error.util';
import { compareFiles } from '../../../../../utils/compare-files.util';

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
        TuiScrollbar,
        ImageUploaderComponent,
        FormFieldComponent,
        AddRegionComponent,
        RegionsListComponent
    ],
    providers: [
        tuiValidationErrorsProvider({
            required: 'Поле обязательно для заполнения',
            regionAlreadyExists: 'Регион уже добавлен в список'
        })
    ]
})
export class EditAnalyticsMapModalComponent extends EditMapModalBaseComponent implements OnInit, OnDestroy {
    public readonly model: InputSignal<IMapModel> = input.required();
    public readonly card: InputSignal<IHubCard | null> = input.required();

    protected readonly isRegionModalOpen: WritableSignal<boolean> = signal(false);
    protected readonly allRegions: WritableSignal<string[]> = signal([]);
    protected readonly activeRegions: WritableSignal<IMapLayerProperties[]> = signal([]);
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
                distinctUntilChanged(compareFiles),
                map(file => this.buildBgStyle(file))
            );

    protected editingRegionName: string | null = null;

    private readonly _fileService: FileService = inject(FileService);
    private readonly _destroyRef: DestroyRef = inject(DestroyRef);
    private _revokePreview: (() => void) | null = null;

    public ngOnInit(): void {
        this.initModel();
        this.listenRegionNameChanges();
    }

    public ngOnDestroy(): void {
        this._revokePreview?.();
    }

    // ---------------------------
    // Region modal
    // ---------------------------

    /** Открыть модалку редактирования региона */
    protected openRegionModal(): void {
        this.editingRegionName = null;
        this.isRegionModalOpen.set(true);
    }

    /** Закрыть модалку редактирования региона */
    protected closeRegionModal(): void {
        this.addRegionForm.reset();
        this.showEditingDeleteError.set(false);
        this.isRegionModalOpen.set(false);
    }

    // ---------------------------
    // Region CRUD
    // ---------------------------

    /** Сохранить регион */
    protected saveRegion(): void {
        if (!this.validateRegionForm()) {
            return;
        }

        const region: IMapLayerProperties = this.buildRegionFromForm();

        this.activeRegions.update(list =>
            this.sortRegions(
                this.editingRegionName
                    ? list.map(item =>
                        item.regionName === this.editingRegionName ? region : item,
                    )
                    : [...list, region],
            ),
        );

        this.editingRegionName = null;
        this.closeRegionModal();
    }

    /**
     * Редактировать регион
     * @param item
     */
    protected editRegion(item: IMapLayerProperties): void {
        this.showEditingDeleteError.set(false);
        this.editingRegionName = item.regionName;

        this.addRegionForm.patchValue({
            regionName: item.regionName,
            color: item.style?.fillColor ?? '',
            partnersCount: item.analyticsData?.partnersCount ?? 0,
            excursionsCount: item.analyticsData?.excursionsCount ?? 0,
            membersCount: item.analyticsData?.membersCount ?? 0
        });

        this.loadRegionImage(item);
        this.isRegionModalOpen.set(true);
    }

    /**
     * Удалить регион
     * @param item
     */
    protected deleteRegion(item: IMapLayerProperties): void {
        if (this.addRegionForm.controls.regionName.value === item.regionName) {
            this.showEditingDeleteError.set(true);

            return;
        }

        const url: string | undefined = item.analyticsData?.imagePath?.trim();
        if (url) {
            this._fileService.removeCachedFileByUrl(url);
        }

        this.activeRegions.update(list =>
            list.filter(region => region.regionName !== item.regionName)
        );
    }

    // ---------------------------
    // Init
    // ---------------------------

    /** Инициализировать модель */
    private initModel(): void {
        const model: IMapModel = this.model();
        const props: IMapLayerProperties[] = this.sortRegions(
            model.layers.map(layer => layer.properties)
        );

        this.allRegions.set(props.map(p => p.regionName));
        this.activeRegions.set(props.filter(p => p.isActive));

        this.settingsForm.patchValue({
            title: model.pageTitle,
            mapInfo: model.infoText,
            cardDescription: this.card()?.description ?? ''
        });

        this.preloadFiles();
    }

    /** Предзагрузить файлы по ссылкам */
    private preloadFiles(): void {
        const tasks: Array<Observable<unknown>> = [];

        const cardUrl: string | undefined = this.card()?.backgroundImagePath?.trim();
        if (cardUrl) {
            tasks.push(
                this._fileService.downloadAsFile(
                    cardUrl,
                    this._fileService.getFileNameFromUrl(cardUrl) ?? 'card-background.svg',
                    'image/svg+xml'
                ).pipe(
                    tap(file => this.settingsForm.controls.cardBackgroundImage.setValue(file)),
                    catchError(() => of(null))
                )
            );
        }

        this.activeRegions().forEach(region => {
            const url: string | undefined = region.analyticsData?.imagePath?.trim();
            if (!url) {
                return;
            }

            tasks.push(
                this._fileService.downloadAsFile(
                    url,
                    this._fileService.getFileNameFromUrl(url) ?? `${region.regionName}.png`
                ).pipe(catchError(() => of(null)))
            );
        });

        if (tasks.length) {
            forkJoin(tasks)
                .pipe(take(1))
                .subscribe();
        }
    }

    // ---------------------------
    // Helpers
    // ---------------------------

    /** Провалидировать форму регионов */
    private validateRegionForm(): boolean {
        this.addRegionForm.markAllAsTouched();
        this.addRegionForm.updateValueAndValidity();

        if (this.addRegionForm.invalid) {
            return false;
        }

        const name: string = this.addRegionForm.controls.regionName.value.trim();

        if (!this.editingRegionName &&
            this.activeRegions().some(r => r.regionName === name)
        ) {
            this.addRegionForm.controls.regionName.setErrors({ regionAlreadyExists: true });

            return false;
        }

        return true;
    }

    /** Собрать регион из формы */
    private buildRegionFromForm(): IMapLayerProperties {
        const f: IAddRegionForm = this.addRegionForm.controls;

        return {
            regionName: f.regionName.value.trim(),
            isActive: true,
            style: {
                ...(this.activeRegions().find(r => r.regionName === this.editingRegionName)?.style ?? {}),
                fillColor: f.color.value,
            },
            analyticsData: {
                partnersCount: f.partnersCount.value,
                excursionsCount: f.excursionsCount.value,
                membersCount: f.membersCount.value,
                imageFile: f.image.value as File | null,
                imagePath: '',
            },
        } as IMapLayerProperties;
    }

    /**
     * Загрузить изображение региона
     * @param item
     */
    private loadRegionImage(item: IMapLayerProperties): void {
        const stored: File | null = item.analyticsData?.imageFile as File | null;
        if (stored) {
            this.addRegionForm.controls.image.setValue(stored);

            return;
        }

        const url: string | undefined = item.analyticsData?.imagePath?.trim();
        this.addRegionForm.controls.image.setValue(
            url ? this._fileService.getCachedFileByUrl(url) : null,
        );
    }

    /**
     * Отсортировать регионы по имени
     * @param list
     */
    private sortRegions(list: IMapLayerProperties[]): IMapLayerProperties[] {
        return list.slice().sort((a, b) =>
            a.regionName.localeCompare(b.regionName, 'ru', { sensitivity: 'base' }),
        );
    }

    /**
     * Собрать стиль background-image по файлу
     * @param file
     * @returns
     */
    private buildBgStyle(file: TuiFileLike | null): SafeStyle | null {
        this._revokePreview?.();

        const { style, revoke }: {
            style: SafeStyle | null,
            revoke: () => void
        } = this._fileService.buildBackgroundImagePreview(file);
        this._revokePreview = revoke;

        return style;
    }

    /** Подписка на изменения имени региона */
    private listenRegionNameChanges(): void {
        this.addRegionForm.controls.regionName.valueChanges
            .pipe(
                tap(() => clearControlError(this.addRegionForm.controls.regionName, 'regionAlreadyExists')),
                takeUntilDestroyed(this._destroyRef)
            )
            .subscribe();
    }
}
