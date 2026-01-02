import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { FormFieldComponent } from '../../../../../components/form-field/form-field.component';
import { ImageUploaderComponent } from '../../../../../components/image-uploader/image-uploader.component';
import { SelectAutocompleteComponent } from '../../../../../components/select-autocomplete/select-autocomplete.component';
import { TuiButton } from '@taiga-ui/core';
import { EditMapItemModalBaseComponent } from '../../../../../components/edit-map-modal/components/edit-map-item-modal.base.component';
import { IEditPointForm } from '../../interfaces/edit-point-form.interface';
import { CoordinatesInputComponent } from '../../../../../components/coordinates-input/coordinates-input.component';

@Component({
    selector: 'edit-point-modal',
    standalone: true,
    templateUrl: './edit-point-modal.component.html',
    styleUrl: './styles/edit-point-modal.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        ReactiveFormsModule,
        FormFieldComponent,
        ImageUploaderComponent,
        SelectAutocompleteComponent,
        CoordinatesInputComponent,
        TuiButton
    ]
})
export class EditPointModalComponent extends EditMapItemModalBaseComponent<IEditPointForm> {}
