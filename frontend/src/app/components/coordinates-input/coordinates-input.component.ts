import { ChangeDetectionStrategy, Component, DestroyRef, forwardRef, inject, input, InputSignal, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALIDATORS, NG_VALUE_ACCESSOR, ReactiveFormsModule, ValidationErrors, ValidatorFn } from '@angular/forms';
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
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => CoordinatesInputComponent),
            multi: true
        },
        {
            provide: NG_VALIDATORS,
            useExisting: forwardRef(() => CoordinatesInputComponent),
            multi: true
        },
        tuiValidationErrorsProvider({
            required: 'Поле обязательно для заполнения',
            coordinatesFormatError: 'Неверный формат координат'
        })
    ]
})
export class CoordinatesInputComponent implements ControlValueAccessor, OnInit {
    public readonly label: InputSignal<string> = input.required();
    public readonly control: InputSignal<FormControl> = input.required();
    public readonly placeholder: InputSignal<string> = input('');

    protected readonly innerControl: FormControl<string> = new FormControl<string>('', { validators: [coordinatesValidator], nonNullable: true });
    private readonly _destroyRef: DestroyRef = inject(DestroyRef);

    public ngOnInit(): void {
        this.init();
    }

    /** @inheritdoc */
    public writeValue(obj: [number, number] | null): void {
        if (obj) {
            const formatted: string = `${obj[0]}, ${obj[1]}`;
            if (this.innerControl.value !== formatted) {
                this.innerControl.setValue(formatted, { emitEvent: false });
            }
        } else {
            this.innerControl.setValue('', { emitEvent: false });
        }
    }

    /** @inheritdoc */
    public registerOnChange(fn: (value: [number, number] | null) => void): void {
        this.onChange = fn;
    }

    /** @inheritdoc */
    public registerOnTouched(fn: () => void): void {
        this.onTouched = fn;
    }

    /** @inheritdoc */
    public setDisabledState(): void {
        this.innerControl.disable({ emitEvent: false });
    }

    /** Валидация контрола */
    public validate(): ValidationErrors | null {
        return this.innerControl.errors;
    }

    /** Инициализация контрола */
    private init(): void {
        const parentValidator: ValidatorFn | null = this.control().validator;
        this.innerControl.setValidators(
            parentValidator
                ? [coordinatesValidator, parentValidator]
                : [coordinatesValidator]
        );
        this.innerControl.updateValueAndValidity({ emitEvent: false });

        this.control().valueChanges
            .pipe(
                startWith(this.control().value),
                takeUntilDestroyed(this._destroyRef)
            )
            .subscribe(value => this.writeValue(value));

        this.innerControl.valueChanges
            .pipe(takeUntilDestroyed(this._destroyRef))
            .subscribe((raw: string) => {
                if (this.innerControl.invalid) {
                    this.onChange(null);

                    return;
                }
                const parsed: [number, number] | null = this.parseCoordinates(raw);
                this.onChange(parsed);
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

    private onChange: (value: [number, number] | null) => void = () => { /* noop */ };
    private onTouched: () => void = () => { /* noop */ };
}
