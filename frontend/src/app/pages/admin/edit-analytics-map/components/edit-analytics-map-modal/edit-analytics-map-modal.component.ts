import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { EditMapModalBaseComponent } from '../../../../../components/edit-map-modal/edit-map-modal.base.component';
import { TuiAccordion } from '@taiga-ui/experimental';
import { TuiCell } from '@taiga-ui/layout';
import { TuiButton, TuiScrollbar, TuiTextfield } from '@taiga-ui/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { TuiButtonLoading, tuiValidationErrorsProvider } from '@taiga-ui/kit';
import { AsyncPipe } from '@angular/common';
import { ImageUploaderComponent } from '../../../../../components/image-uploader/image-uploader.component';
import { SafeStyle } from '@angular/platform-browser';
import { catchError, forkJoin, Observable, of, take, tap } from 'rxjs';
import { FormFieldComponent } from '../../../../../components/form-field/form-field.component';
import { IAnalyticsMapSettingsForm } from '../../../../../components/edit-map-modal/interfaces/analytics-map-settings-form.interface';
import { IEditRegionForm } from '../../interfaces/edit-region-form.interface';
import { IMapLayerProperties } from '../../../../../components/map/interfaces/map-layer.interface';
import { IMapModel } from '../../../../../components/map/models/map.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RegionsListComponent } from '../regions-list/regions-list.component';
import { clearControlError } from '../../../../../utils/clear-control-error.util';
import { EditRegionModalComponent } from '../edit-region-modal/edit-region-modal.component';

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
        TuiButtonLoading,
        TuiScrollbar,
        ImageUploaderComponent,
        FormFieldComponent,
        EditRegionModalComponent,
        RegionsListComponent
    ],
    providers: [
        tuiValidationErrorsProvider({
            required: 'Поле обязательно для заполнения',
            regionAlreadyExists: 'Регион уже добавлен в список'
        })
    ]
})
export class EditAnalyticsMapModalComponent
    extends EditMapModalBaseComponent<IAnalyticsMapSettingsForm, IEditRegionForm, IMapLayerProperties>
    implements OnInit
{
    protected readonly cardPreviewBackgroundImage$: Observable<SafeStyle | null> = this.createImagePreview(
        this.settingsForm.controls.cardBackgroundImage
    );

    public override ngOnInit(): void {
        super.ngOnInit();
        this.initModel();
        this.listenRegionNameChanges();
    }

    /** Собрать форму настроек */
    protected buildSettingsForm(): IAnalyticsMapSettingsForm {
        return {
            title: new FormControl('', {
                nonNullable: true,
                validators: [Validators.required]
            }),
            cardBackgroundImage: new FormControl<File | null>(null),
            cardDescription: new FormControl ('', {
                nonNullable: true,
                validators: [Validators.required]
            }),
            mapInfo: new FormControl ('', {
                nonNullable: true,
                validators: [Validators.required]
            })
        };
    }

    /** Собрать форму редактирования региона */
    protected buildEditItemForm(): IEditRegionForm {
        return {
            regionName: new FormControl('', {
                nonNullable: true,
                validators: [Validators.required]
            }),
            image: new FormControl<File | null>(null),
            color: new FormControl('', {
                nonNullable: true,
                validators: [Validators.required]
            }),
            partnersCount: new FormControl(0, {
                nonNullable: true,
                validators: [Validators.required]
            }),
            excursionsCount: new FormControl(0, {
                nonNullable: true,
                validators: [Validators.required]
            }),
            membersCount: new FormControl(0, {
                nonNullable: true,
                validators: [Validators.required]
            }),
        };
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
        this.activeItems.update(list =>
            this.sortRegions(
                this.editingItemName
                    ? list.map(item =>
                        item.regionName === this.editingItemName ? region : item,
                    )
                    : [...list, region],
            ),
        );

        this.editingItemName = null;
        this.closeEditItemModal();
        this.activeItemsChanged.emit(this.activeItems());
    }

    /**
     * Редактировать регион
     * @param item
     */
    protected editRegion(item: IMapLayerProperties): void {
        this.showEditingDeleteError.set(false);
        this.editingItemName = item.regionName;

        this.editItemForm.patchValue({
            regionName: item.regionName,
            color: item.style?.fillColor ?? '',
            partnersCount: item.analyticsData?.partnersCount ?? 0,
            excursionsCount: item.analyticsData?.excursionsCount ?? 0,
            membersCount: item.analyticsData?.membersCount ?? 0
        });

        this.loadRegionImage(item);
        this.isEditItemModalOpen.set(true);
    }

    /**
     * Удалить регион
     * @param item
     */
    protected deleteRegion(item: IMapLayerProperties): void {
        if (this.editItemForm.controls.regionName.value === item.regionName) {
            this.showEditingDeleteError.set(true);

            return;
        }

        const url: string | undefined = item.analyticsData?.imagePath?.trim();
        if (url) {
            this.fileService.removeCachedFileByUrl(url);
        }

        this.activeItems.update(list =>
            list.filter(region => region.regionName !== item.regionName)
        );
        this.activeItemsChanged.emit(this.activeItems());
    }

    // ---------------------------
    // Init
    // ---------------------------

    /** Инициализировать модель */
    private initModel(): void {
        this.activeItems.set(this.allRegions().filter(p => p.isActive));

        const model: IMapModel = this.model();
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
                this.fileService.downloadAsFile(
                    cardUrl,
                    this.fileService.getFileNameFromUrl(cardUrl) ?? 'card-background.svg',
                    'image/svg+xml'
                ).pipe(
                    tap(file => this.settingsForm.controls.cardBackgroundImage.setValue(file)),
                    catchError(() => of(null))
                )
            );
        }

        this.activeItems().forEach(region => {
            const url: string | undefined = region.analyticsData?.imagePath?.trim();
            if (!url) {
                return;
            }

            tasks.push(
                this.fileService.downloadAsFile(
                    url,
                    this.fileService.getFileNameFromUrl(url) ?? `${region.regionName}`
                ).pipe(catchError(() => of(null)))
            );
        });

        if (tasks.length) {
            forkJoin(tasks)
                .pipe(take(1))
                .subscribe();
        }
    }

    /** Подписка на изменения имени региона */
    private listenRegionNameChanges(): void {
        this.editItemForm.controls.regionName.valueChanges
            .pipe(
                tap(() => clearControlError(this.editItemForm.controls.regionName, 'regionAlreadyExists')),
                takeUntilDestroyed(this.destroyRef)
            )
            .subscribe();
    }

    // ---------------------------
    // Helpers
    // ---------------------------

    /** Провалидировать форму редактирования регионов */
    private validateRegionForm(): boolean {
        this.editItemForm.markAllAsTouched();
        this.editItemForm.updateValueAndValidity();

        if (this.editItemForm.invalid) {
            return false;
        }

        const name: string = this.editItemForm.controls.regionName.value.trim();

        if (!this.editingItemName &&
            this.activeItems().some(r => r.regionName === name)
        ) {
            this.editItemForm.controls.regionName.setErrors({ regionAlreadyExists: true });

            return false;
        }

        return true;
    }

    /** Собрать регион из формы */
    private buildRegionFromForm(): IMapLayerProperties {
        const controls: IEditRegionForm = this.editItemForm.controls;
        const regionName: string = controls.regionName.value.trim();
        const existing: IMapLayerProperties | undefined = this.activeItems().find(r => r.regionName === this.editingItemName);

        return {
            id: this.allRegions().find(r => r.regionName === regionName)?.id,
            regionName,
            isActive: true,
            style: {
                ...(existing?.style ?? {}),
                fillColor: controls.color.value,
            },
            analyticsData: {
                partnersCount: controls.partnersCount.value,
                excursionsCount: controls.excursionsCount.value,
                membersCount: controls.membersCount.value,
                image: controls.image.value,
                imagePath: '',
            },
        } as IMapLayerProperties;
    }

    /**
     * Загрузить изображение региона
     * @param item
     */
    private loadRegionImage(item: IMapLayerProperties): void {
        const stored: File | null | undefined = item.analyticsData?.image;
        if (stored) {
            this.editItemForm.controls.image.setValue(stored);

            return;
        }

        const url: string | undefined = item.analyticsData?.imagePath?.trim();
        this.editItemForm.controls.image.setValue(
            url ? this.fileService.getCachedFileByUrl(url) : null,
        );
    }
}
