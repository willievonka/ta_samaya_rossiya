/* eslint-disable @typescript-eslint/no-empty-function */
import { AsyncPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, forwardRef, inject, input, InputSignal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { TuiFileLike, TuiFiles } from '@taiga-ui/kit';
import { BehaviorSubject, Observable, of, Subject, switchMap, tap } from 'rxjs';

@Component({
    selector: 'image-uploader',
    standalone: true,
    templateUrl: './image-uploader.component.html',
    styleUrl: './styles/image-uploader.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        ReactiveFormsModule,
        AsyncPipe,
        TuiFiles
    ],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => ImageUploaderComponent),
            multi: true
        }
    ]
})
export class ImageUploaderComponent implements ControlValueAccessor {
    public readonly fileControl: FormControl<TuiFileLike | null> = new FormControl<TuiFileLike | null>(null);
    public readonly fileType: InputSignal<string> = input('image/svg+xml');
    public readonly size: InputSignal<'m' | 'l'> = input<'m' | 'l'>('l');

    protected readonly failedFile$: Subject<TuiFileLike | null> = new Subject<TuiFileLike | null>();
    protected readonly loadingFile$: BehaviorSubject<TuiFileLike | null> = new BehaviorSubject<TuiFileLike | null>(null);
    protected readonly loadedFile$: BehaviorSubject<TuiFileLike | null> = new BehaviorSubject<TuiFileLike | null>(null);

    private readonly _destroyRef: DestroyRef = inject(DestroyRef);
    private _isWritingValue: boolean = false;

    constructor () {
        this.fileControl.valueChanges
            .pipe(
                switchMap(file => this._isWritingValue ? of(file) : this.processFile(file)),
                takeUntilDestroyed(this._destroyRef)
            )
            .subscribe();
    }

    /** @inheritdoc */
    public writeValue(value: TuiFileLike | null): void {
        this._isWritingValue = true;

        this.fileControl.setValue(value, { emitEvent: false });
        this.failedFile$.next(null);
        this.loadingFile$.next(null);
        this.loadedFile$.next(value);

        this._isWritingValue = false;
    }

    /** @inheritdoc */
    public registerOnChange(fn: (value: TuiFileLike | null) => void): void {
        this.onChange = fn;
    }

    /** @inheritdoc */
    public registerOnTouched(fn: () => void): void {
        this.onTouched = fn;
    }

    /** @inheritdoc */
    public setDisabledState(isDisabled: boolean): void {
        if (isDisabled) {
            this.fileControl.disable({ emitEvent: false });
        } else {
            this.fileControl.enable({ emitEvent: false });
        }
    }

    /** Удалить файл */
    protected removeFile(): void {
        this.fileControl.setValue(null);
        this.onTouched();
        this.onChange(null);
    }

    /** Загрузить файл */
    protected processFile(file: TuiFileLike | null): Observable<TuiFileLike | null> {
        this.failedFile$.next(null);

        if (!file) {
            this.loadingFile$.next(null);
            this.loadedFile$.next(null);
            this.onChange(null);

            return of(null);
        }

        this.loadingFile$.next(file);

        return of(file)
            .pipe(
                tap({
                    next: (loaded: TuiFileLike | null) => {
                        this.loadingFile$.next(null);
                        this.loadedFile$.next(loaded);
                        this.onTouched();
                        this.onChange(loaded);
                    },
                    error: () => {
                        this.loadingFile$.next(null);
                        this.loadedFile$.next(null);
                        this.failedFile$.next(file);
                        this.onTouched();
                        this.onChange(null);
                    }
                })
            );
    }

    protected onChange: (value: TuiFileLike | null) => void = () => {};
    protected onTouched: () => void = () => {};
}
