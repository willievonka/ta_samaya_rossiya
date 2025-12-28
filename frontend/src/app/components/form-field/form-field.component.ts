import { AsyncPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, input, InputSignal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { TuiError, TuiLabel, TuiTextfield } from '@taiga-ui/core';
import { TuiFieldErrorPipe, TuiTextarea, TuiInputNumberDirective, TuiInputColor } from '@taiga-ui/kit';

@Component({
    selector: 'form-field',
    standalone: true,
    templateUrl: './form-field.component.html',
    styleUrl: './styles/form-field.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        AsyncPipe,
        ReactiveFormsModule,
        TuiLabel,
        TuiTextfield,
        TuiTextarea,
        TuiError,
        TuiFieldErrorPipe,
        TuiInputNumberDirective,
        TuiInputColor
    ]
})
export class FormFieldComponent {
    public readonly label: InputSignal<string> = input.required();
    public readonly control: InputSignal<FormControl> = input.required();
    public readonly placeholder: InputSignal<string> = input('');
    public readonly size: InputSignal<'s' | 'm' | 'l'> = input<'s' | 'm' | 'l'>('l');
    public readonly multiline: InputSignal<boolean> = input(false);
    public readonly numeric: InputSignal<boolean> = input(false);
    public readonly color: InputSignal<boolean> = input(false);
    public readonly errorClass: InputSignal<string> = input('');
}
