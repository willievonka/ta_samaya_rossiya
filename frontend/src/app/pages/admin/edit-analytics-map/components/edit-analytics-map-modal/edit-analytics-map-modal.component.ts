import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { EditMapModalBaseComponent } from '../../../../../components/edit-map-modal/edit-map-modal.base.component';
import { TuiAccordion } from '@taiga-ui/experimental';
import { TuiCell } from '@taiga-ui/layout';
import { TuiButton, TuiError, TuiLabel, TuiTextfield } from '@taiga-ui/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TuiFieldErrorPipe, TuiFileLike, TuiTextarea } from '@taiga-ui/kit';
import { AsyncPipe } from '@angular/common';
import { ImageUploaderComponent } from '../../../../../components/image-uploader/image-uploader.component';
import { DomSanitizer, SafeStyle } from '@angular/platform-browser';
import { distinctUntilChanged, map, Observable, startWith } from 'rxjs';

@Component({
    selector: 'edit-analytics-map-modal',
    standalone: true,
    templateUrl: './edit-analytics-map-modal.component.html',
    styleUrl: './styles/edit-analytics-map-modal.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        AsyncPipe,
        ReactiveFormsModule,
        TuiAccordion,
        TuiCell,
        TuiTextfield,
        TuiButton,
        TuiError,
        TuiFieldErrorPipe,
        TuiLabel,
        TuiTextarea,
        ImageUploaderComponent
    ]
})
export class EditAnalyticsMapModalComponent extends EditMapModalBaseComponent {
    protected readonly settingsForm: FormGroup = new FormGroup({
        title: new FormControl<string>('', [Validators.required]),
        cardBackgroundImage: new FormControl<TuiFileLike | null>(null),
        cardDescription: new FormControl<string>('')
    });

    protected readonly cardPreviewBackgroundImage$: Observable<SafeStyle | null> =
        this.settingsForm.controls['cardBackgroundImage'].valueChanges
            .pipe(
                startWith(this.settingsForm.controls['cardBackgroundImage'].value),
                distinctUntilChanged(),
                map(file => this.buildBgStyle(file))
            );

    private readonly _sanitizer: DomSanitizer = inject(DomSanitizer);

    private _objectUrl: string | null = null;

    /** Сбилдить стиль background-image из файла */
    private buildBgStyle(file: TuiFileLike | null): SafeStyle | null {
        if (this._objectUrl) {
            URL.revokeObjectURL(this._objectUrl);
            this._objectUrl = null;
        }
        if (!file) {
            return null;
        }

        const nativeFile: File = file as File;
        this._objectUrl = URL.createObjectURL(nativeFile);

        return this._sanitizer.bypassSecurityTrustStyle(`url("${this._objectUrl}")`);
    }
}
