import { ChangeDetectionStrategy, Component, DestroyRef, inject, input, InputSignal, OnInit } from '@angular/core';
import { FormControl, ReactiveFormsModule, ValidatorFn } from '@angular/forms';
import { coordinatesValidator } from './validators/coordinates.validator';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TuiError, TuiTextfield } from '@taiga-ui/core';
import { TuiFieldErrorPipe, tuiValidationErrorsProvider } from '@taiga-ui/kit';
import { AsyncPipe } from '@angular/common';
import { startWith } from 'rxjs';

@Component({
    selector: 'coordinates-input',
    standalone: true,
    templateUrl: './coordinates-input.component.html',
    styleUrl: './styles/coordinates-input.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        ReactiveFormsModule,
        AsyncPipe,
        TuiTextfield,
        TuiError,
        TuiFieldErrorPipe
    ],
    providers: [
        tuiValidationErrorsProvider({
            required: 'Поле обязательно для заполнения',
            coordinatesFormatError: 'Неверный формат координат'
        })
    ]
})
export class CoordinatesInputComponent implements OnInit {
    public readonly label: InputSignal<string> = input.required();
    public readonly control: InputSignal<FormControl> = input.required();
    public readonly placeholder: InputSignal<string> = input('');

    protected readonly innerControl: FormControl<string> = new FormControl<string>('', {
        validators: [coordinatesValidator],
        nonNullable: true
    });

    private readonly _destroyRef: DestroyRef = inject(DestroyRef);

    public ngOnInit(): void {
        this.syncValidators();
        this.syncFromOuter();
        this.syncToOuter();
    }

    /** Синхронизировать валидаторы */
    private syncValidators(): void {
        const outer: FormControl = this.control();
        const outerValidator: ValidatorFn | null = outer.validator;

        this.innerControl.setValidators(
            outerValidator
                ? [coordinatesValidator, outerValidator]
                : [coordinatesValidator]
        );

        this.innerControl.updateValueAndValidity({ emitEvent: false });
    }

    /** Синхронизировать с внешними изменениями */
    private syncFromOuter(): void {
        this.control().valueChanges
            .pipe(
                startWith(this.control().value),
                takeUntilDestroyed(this._destroyRef),
            )
            .subscribe(value => {
                const formatted: string =
                    value && value.length === 2 ? `${value[0]}, ${value[1]}` : '';

                if (this.innerControl.value !== formatted) {
                    this.innerControl.setValue(formatted, { emitEvent: false });
                }
            });
    }

    /** Синхронизировать с внутренними изменениями */
    private syncToOuter(): void {
        this.innerControl.valueChanges
            .pipe(takeUntilDestroyed(this._destroyRef))
            .subscribe(raw => {
                const outer: FormControl = this.control();

                if (this.innerControl.invalid) {
                    outer.setErrors(this.innerControl.errors);

                    return;
                }

                const parsed: [number, number] | null = this.parseCoordinates(raw);

                if (!parsed) {
                    outer.setErrors({ coordinatesFormatError: true });

                    return;
                }

                outer.setErrors(null);
                outer.setValue(parsed, { emitEvent: false });
            });
    }

    /** Спарсить строку в координаты */
    private parseCoordinates(value: string): [number, number] | null {
        const parts: string[] = value.split(',').map(x => x.trim());
        if (parts.length !== 2) {
            return null;
        }

        const lat: number = Number(parts[0]);
        const lng: number = Number(parts[1]);
        if (isNaN(lat) || isNaN(lng)) {
            return null;
        }

        return [lat, lng];
    }
}
