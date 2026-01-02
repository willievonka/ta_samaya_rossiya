import { ChangeDetectionStrategy, Component } from '@angular/core';
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
import { Observable } from 'rxjs';
import { SafeStyle } from '@angular/platform-browser';

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
        FormFieldComponent
    ]
})
export class EditMapModalComponent
    extends EditMapModalBaseComponent<IMapSettingsForm, IEditPointForm>
{
    protected readonly cardPreviewBackgroundImage$: Observable<SafeStyle | null> = this.createImagePreview(
        this.settingsForm.controls.cardBackgroundImage
    );

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
    /** Собрать форму редакктирования точки */
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
}
