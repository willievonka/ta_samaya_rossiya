import { Directive, input, InputSignal, output, OutputEmitterRef } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';

@Directive({
    standalone: true
})
export class EditMapItemModalBaseComponent<TItemForm extends { [K in keyof TItemForm]: AbstractControl }> {
    public readonly form: InputSignal<FormGroup<TItemForm>> = input.required();
    public readonly allRegionsList: InputSignal<string[]> = input.required();

    public readonly closeModal: OutputEmitterRef<void> = output();
    public readonly itemSaved: OutputEmitterRef<void> = output();
}
