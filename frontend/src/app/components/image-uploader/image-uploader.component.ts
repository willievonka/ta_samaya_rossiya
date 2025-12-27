/* eslint-disable @typescript-eslint/no-empty-function */
import { AsyncPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, forwardRef, input, InputSignal } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { TuiFileLike, TuiFiles } from '@taiga-ui/kit';
import { BehaviorSubject, distinctUntilChanged, Observable, of, Subject, switchMap, tap } from 'rxjs';

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

    protected readonly failedFile$: Subject<TuiFileLike | null> = new Subject<TuiFileLike | null>();
    protected readonly loadingFile$: BehaviorSubject<TuiFileLike | null> = new BehaviorSubject<TuiFileLike | null>(null);
    protected readonly loadedFile$: Observable<TuiFileLike | null> =
        this.fileControl.valueChanges
            .pipe(
                distinctUntilChanged(),
                switchMap((file) => this.processFile(file))
            );

    /** @inheritdoc */
    public writeValue(value: TuiFileLike | null): void {
        this.fileControl.setValue(value, { emitEvent: false });
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
            this.onChange(null);

            return of(null);
        }

        this.loadingFile$.next(file);

        return of(file)
            .pipe(
                tap({
                    next: (loaded: TuiFileLike | null) => {
                        this.loadingFile$.next(null);
                        this.onTouched();
                        this.onChange(loaded);
                    },
                    error: () => {
                        this.loadingFile$.next(null);
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
