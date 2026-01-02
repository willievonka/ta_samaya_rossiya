import { Directive, input, InputSignal, output, OutputEmitterRef } from '@angular/core';

@Directive({
    standalone: true
})
export class MapItemsList<TItemType> {
    public readonly items: InputSignal<TItemType[]> = input.required<TItemType[]>();
    public readonly editingItemName: InputSignal<string | null> = input<string | null>(null);
    public readonly showEditingDeleteError: InputSignal<boolean> = input(false);
    public readonly currentFormItemName: InputSignal<string> = input('');

    public readonly edit: OutputEmitterRef<TItemType> = output();
    public readonly delete: OutputEmitterRef<TItemType> = output();
}
