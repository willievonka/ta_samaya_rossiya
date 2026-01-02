import { ChangeDetectionStrategy, Component, OnInit, output, OutputEmitterRef, signal, WritableSignal } from '@angular/core';
import { EditMapModalBaseComponent } from '../../../../../components/edit-map-modal/edit-map-modal.base.component';
import { IMapSettingsForm } from '../../../../../components/edit-map-modal/interfaces/map-settings-form.interface';
import { IEditPointForm } from '../../interfaces/edit-point-form.interface';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { TuiFileLike } from '@taiga-ui/kit';
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
        PointsListComponent
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

    public ngOnInit(): void {
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

    /** Инициализировать модель */
    private initModel(): void {
        const model: IMapModel = this.model();
        this.activePoints.set(
            model.layers.flatMap(layer => layer.properties.points ?? []).sort((a, b) => a.year - b.year)
        );

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
                    this.fileService.getFileNameFromUrl(url) ?? `${point.title}.png`
                ).pipe(catchError(() => of(null)))
            );
        });

        if (tasks.length) {
            forkJoin(tasks)
                .pipe(take(1))
                .subscribe();
        }
    }
}
