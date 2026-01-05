import { ChangeDetectionStrategy, Component, OnInit, output, OutputEmitterRef, signal, WritableSignal } from '@angular/core';
import { EditMapModalBaseComponent } from '../../../../../components/edit-map-modal/edit-map-modal.base.component';
import { IMapSettingsForm } from '../../../../../components/edit-map-modal/interfaces/map-settings-form.interface';
import { IEditPointForm } from '../../interfaces/edit-point-form.interface';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { TuiFileLike, tuiValidationErrorsProvider } from '@taiga-ui/kit';
import { AsyncPipe } from '@angular/common';
import { TuiCell } from '@taiga-ui/layout';
import { TuiButton, TuiScrollbar, TuiTextfield } from '@taiga-ui/core';
import { ImageUploaderComponent } from '../../../../../components/image-uploader/image-uploader.component';
import { FormFieldComponent } from '../../../../../components/form-field/form-field.component';
import { TuiAccordion } from '@taiga-ui/experimental';
import { catchError, forkJoin, Observable, of, take, tap } from 'rxjs';
import { SafeStyle } from '@angular/platform-browser';
import { IMapPoint } from '../../../../../components/map/interfaces/map-point.interface';
import { PointsListComponent } from '../points-list/points-list.component';
import { IMapModel } from '../../../../../components/map/models/map.model';
import { IMapLayerProperties } from '../../../../../components/map/interfaces/map-layer.interface';
import { EditPointModalComponent } from '../edit-point-modal/edit-point-modal.component';

@Component({
    selector: 'edit-map-modal',
    standalone: true,
    templateUrl: './edit-map-modal.component.html',
    styleUrl: './styles/edit-map-modal.master.scss',
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
        PointsListComponent,
        EditPointModalComponent
    ],
    providers: [
        tuiValidationErrorsProvider({
            required: 'Поле обязательно для заполнения',
            pointAlreadyExists: 'Точка уже добавлена в список'
        })
    ]
})
export class EditMapModalComponent
    extends EditMapModalBaseComponent<IMapSettingsForm, IEditPointForm>
    implements OnInit
{
    public readonly activePointsChanged: OutputEmitterRef<IMapPoint[]> = output<IMapPoint[]>();

    protected readonly activePoints: WritableSignal<IMapPoint[]> = signal([]);
    protected readonly cardPreviewBackgroundImage$: Observable<SafeStyle | null> = this.createImagePreview(
        this.settingsForm.controls.cardBackgroundImage
    );

    public override ngOnInit(): void {
        super.ngOnInit();
        this.initModel();
    }

    /** Собрать форму настроек */
    protected buildSettingsForm(): IMapSettingsForm {
        return {
            title: new FormControl('', {
                nonNullable: true,
                validators: [Validators.required],
            }),
            cardBackgroundImage: new FormControl<TuiFileLike | null>(null),
            cardDescription: new FormControl('', { nonNullable: true }),
            mapInfo: new FormControl('', { nonNullable: true }),
            layerWithPointsColor: new FormControl ('', {
                nonNullable: true,
                validators: [Validators.required]
            }),
            pointsColor: new FormControl('', {
                nonNullable: true,
                validators: [Validators.required]
            })
        };
    }

    /** Собрать форму редактирования точки */
    protected buildEditItemForm(): IEditPointForm {
        return {
            regionName: new FormControl('', {
                nonNullable: true,
                validators: [Validators.required]
            }),
            pointName: new FormControl('', {
                nonNullable: true,
                validators: [Validators.required]
            }),
            coordinates: new FormControl<[number, number]>([0, 0], {
                nonNullable: true,
                validators: [Validators.required]
            }),
            image: new FormControl<TuiFileLike | null>(null),
            year: new FormControl<number>(new Date().getFullYear(), {
                nonNullable: true,
                validators: [Validators.required]
            }),
            description: new FormControl('', { nonNullable: true }),
            excursionUrl: new FormControl('', { nonNullable: true })
        };
    }

    // ---------------------------
    // Region CRUD
    // ---------------------------

    /** Сохранить точку */
    protected savePoint(): void {
        if (!this.validatePointForm()) {
            return;
        }

        const point: IMapPoint = this.buildPointFromForm();
        this.activePoints.update(list => {
            const updatedList: IMapPoint[] = this.editingItemName
                ? list.map(item =>
                    item.title === this.editingItemName ? point : item
                )
                : [...list, point];

            return [...updatedList].sort(
                (a, b) => a.year - b.year
            );
        });

        this.editingItemName = null;
        this.closeEditItemModal();
        this.activePointsChanged.emit(this.activePoints());
    }

    /**
     * Редактировать точку
     * @param item
     */
    protected editPoint(item: IMapPoint): void {
        this.showEditingDeleteError.set(false);
        this.editingItemName = item.title;

        this.editItemForm.patchValue({
            regionName: item.regionName,
            pointName: item.title,
            coordinates: item.coordinates,
            year: item.year,
            description: item.description,
            excursionUrl: item.excursionUrl
        });

        this.loadPointImage(item);
        this.isEditItemModalOpen.set(true);
    }

    /**
     * Удалить точку
     * @param item
     */
    protected deletePoint(item: IMapPoint): void {
        if (this.editItemForm.controls.pointName.value === item.title) {
            this.showEditingDeleteError.set(true);

            return;
        }

        const url: string | undefined = item.imagePath?.trim();
        if (url) {
            this.fileService.removeCachedFileByUrl(url);
        }

        this.activePoints.update(list =>
            list.filter(point => point.title !== item.title)
        );
        this.activePointsChanged.emit(this.activePoints());
    }

    // ---------------------------
    // Init
    // ---------------------------

    /** Инициализировать модель */
    private initModel(): void {
        const allRegions: IMapLayerProperties[] = this.getAllRegions();
        this.allRegions.set(allRegions.map(p => p.regionName));
        this.activePoints.set(
            allRegions
                .filter(region => !!region.points && region.points.length > 0)
                .flatMap(region =>
                    region.points!.map(point => ({
                        ...point,
                        regionName: region.regionName
                    }))
                )
                .sort((a, b) => a.year - b.year)
        );

        const model: IMapModel = this.model();
        this.settingsForm.patchValue({
            title: model.pageTitle,
            mapInfo: model.infoText,
            cardDescription: this.card()?.description ?? '',
            pointsColor: model.pointColor ?? '',
            layerWithPointsColor: model.layerWithPointsColor ?? ''
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

        this.activePoints().forEach(point => {
            const url: string | undefined = point.imagePath?.trim();
            if (!url) {
                return;
            }

            tasks.push(
                this.fileService.downloadAsFile(
                    url,
                    this.fileService.getFileNameFromUrl(url) ?? `${point.title}`
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

    /** Провалидировать форму редактирования точек */
    private validatePointForm(): boolean {
        this.editItemForm.markAllAsTouched();
        this.editItemForm.updateValueAndValidity();

        if (this.editItemForm.invalid) {
            return false;
        }

        const name: string = this.editItemForm.controls.pointName.value.trim();

        if (!this.editingItemName &&
            this.activePoints().some(p => p.title === name)
        ) {
            this.editItemForm.controls.pointName.setErrors({ pointAlreadyExists: true });

            return false;
        }

        return true;
    }

    /** Собрать точку из формы */
    private buildPointFromForm(): IMapPoint {
        const controls: IEditPointForm = this.editItemForm.controls;

        return {
            title: controls.pointName.value.trim(),
            regionName: controls.regionName.value.trim(),
            coordinates: controls.coordinates.value,
            year: controls.year.value,
            imageFile: controls.image.value as File | null,
            imagePath: '',
            description: controls.description.value.trim(),
            excursionUrl: controls.excursionUrl.value.trim()
        } as IMapPoint;
    }

    /**
     * Загрузить изображение точки
     * @param item
     */
    private loadPointImage(item: IMapPoint): void {
        const stored: File | null = item.imageFile as File | null;
        if (stored) {
            this.editItemForm.controls.image.setValue(stored);

            return;
        }

        const url: string | undefined = item.imagePath.trim();
        this.editItemForm.controls.image.setValue(
            url ? this.fileService.getCachedFileByUrl(url) : null,
        );
    }
}
