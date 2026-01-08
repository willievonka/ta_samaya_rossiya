import { ChangeDetectionStrategy, Component, input, InputSignal } from '@angular/core';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { TuiDataList, TuiError, TuiLabel, TuiScrollable, TuiScrollbar, TuiTextfield } from '@taiga-ui/core';
import { TuiChevron, TuiComboBox, TuiFieldErrorPipe, TuiFilterByInputPipe } from '@taiga-ui/kit';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { AsyncPipe } from '@angular/common';

@Component({
    selector: 'select-autocomplete',
    standalone: true,
    templateUrl: './select-autocomplete.component.html',
    styleUrl: './styles/select-autocomplete.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        AsyncPipe,
        ReactiveFormsModule,
        ScrollingModule,
        TuiChevron,
        TuiComboBox,
        TuiDataList,
        TuiFilterByInputPipe,
        TuiScrollable,
        TuiTextfield,
        TuiLabel,
        TuiError,
        TuiFieldErrorPipe,
        TuiScrollbar
    ]
})
export class SelectAutocompleteComponent {
    public readonly label: InputSignal<string> = input.required();
    public readonly control: InputSignal<FormControl> = input.required();
    public readonly itemsList: InputSignal<string[]> = input.required();
    public readonly placeholder: InputSignal<string> = input('');
}
