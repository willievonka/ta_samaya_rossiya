import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { IEditRegionForm } from '../../interfaces/edit-region-form.interface';
import { FormFieldComponent } from '../../../../../components/form-field/form-field.component';
import { ImageUploaderComponent } from '../../../../../components/image-uploader/image-uploader.component';
import { TuiButton } from '@taiga-ui/core';
import { SelectAutocompleteComponent } from '../../../../../components/select-autocomplete/select-autocomplete.component';
import { EditMapItemModalBaseComponent } from '../../../../../components/edit-map-modal/components/edit-map-item-modal.base.component';

@Component({
    selector: 'edit-region-modal',
    standalone: true,
    templateUrl: './edit-region-modal.component.html',
    styleUrl: './styles/edit-region-modal.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        ReactiveFormsModule,
        FormFieldComponent,
        ImageUploaderComponent,
        SelectAutocompleteComponent,
        TuiButton
    ]
})
export class EditRegionModalComponent extends EditMapItemModalBaseComponent<IEditRegionForm> {}
