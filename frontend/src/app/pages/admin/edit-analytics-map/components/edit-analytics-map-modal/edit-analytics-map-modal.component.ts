import { ChangeDetectionStrategy, Component, inject, OnDestroy } from '@angular/core';
import { EditMapModalBaseComponent } from '../../../../../components/edit-map-modal/edit-map-modal.base.component';
import { TuiAccordion } from '@taiga-ui/experimental';
import { TuiCell } from '@taiga-ui/layout';
import { TuiButton, TuiTextfield } from '@taiga-ui/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TuiFileLike } from '@taiga-ui/kit';
import { AsyncPipe } from '@angular/common';
import { ImageUploaderComponent } from '../../../../../components/image-uploader/image-uploader.component';
import { DomSanitizer, SafeStyle } from '@angular/platform-browser';
import { distinctUntilChanged, map, Observable, startWith } from 'rxjs';
import { FormFieldComponent } from '../../../../../components/form-field/form-field.component';
import { IAnalyticsMapSettingsForm } from './interfaces/analytics-map-settings-form.interface';

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
        ImageUploaderComponent,
        FormFieldComponent
    ]
})
export class EditAnalyticsMapModalComponent extends EditMapModalBaseComponent implements OnDestroy {
    protected readonly settingsForm: FormGroup<IAnalyticsMapSettingsForm> = new FormGroup<IAnalyticsMapSettingsForm>({
        title: new FormControl<string>('', { nonNullable: true, validators: [Validators.required] }),
        cardBackgroundImage: new FormControl<TuiFileLike | null>(null),
        cardDescription: new FormControl<string>('', { nonNullable: true }),
        mapInfo: new FormControl<string>('', { nonNullable: true })
    });

    protected readonly cardPreviewBackgroundImage$: Observable<SafeStyle | null> =
        this.settingsForm.controls.cardBackgroundImage.valueChanges
            .pipe(
                startWith(this.settingsForm.controls.cardBackgroundImage.value),
                distinctUntilChanged(),
                map(file => this.buildBgStyle(file))
            );

    private _objectUrl: string | null = null;
    private readonly _sanitizer: DomSanitizer = inject(DomSanitizer);

    public ngOnDestroy(): void {
        this.revokeObjectUrl();
    }

    /** Собрать стиль background-image из файла */
    private buildBgStyle(file: TuiFileLike | null): SafeStyle | null {
        this.revokeObjectUrl();

        if (!file) {
            return null;
        }
        const nativeFile: File = file as File;
        this._objectUrl = URL.createObjectURL(nativeFile);

        return this._sanitizer.bypassSecurityTrustStyle(`url("${this._objectUrl}")`);
    }

    /** Сбросить objectUrl */
    private revokeObjectUrl(): void {
        if (this._objectUrl) {
            URL.revokeObjectURL(this._objectUrl);
            this._objectUrl = null;
        }
    }
}
