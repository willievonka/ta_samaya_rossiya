import { FormControl } from '@angular/forms';

export interface IAnalyticsMapSettingsForm {
    title: FormControl<string>;
    cardBackgroundImage: FormControl<File | null>;
    cardDescription: FormControl<string>;
    mapInfo: FormControl<string>;
}
