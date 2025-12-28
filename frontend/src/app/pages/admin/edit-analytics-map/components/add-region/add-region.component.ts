import { ChangeDetectionStrategy, Component, input, InputSignal, output, OutputEmitterRef } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { IAddRegionForm } from './interfaces/add-region-form.interface';
import { FormFieldComponent } from '../../../../../components/form-field/form-field.component';
import { ImageUploaderComponent } from '../../../../../components/image-uploader/image-uploader.component';
import { TuiButton } from '@taiga-ui/core';

@Component({
    selector: 'add-region',
    standalone: true,
    templateUrl: './add-region.component.html',
    styleUrl: './styles/add-region.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        ReactiveFormsModule,
        FormFieldComponent,
        ImageUploaderComponent,
        TuiButton
    ]
})
export class AddRegionComponent {
    public readonly form: InputSignal<FormGroup<IAddRegionForm>> = input.required();
    public readonly errorClass: InputSignal<string> = input('');
    public readonly regionsList: InputSignal<string[]> = input.required();
    public readonly closeModal: OutputEmitterRef<void> = output();
}
