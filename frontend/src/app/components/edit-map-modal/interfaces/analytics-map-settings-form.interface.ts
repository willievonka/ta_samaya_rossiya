import { FormControl } from '@angular/forms';
import { TuiFileLike } from '@taiga-ui/kit';

export interface IAnalyticsMapSettingsForm {
    title: FormControl<string>;
    cardBackgroundImage: FormControl<TuiFileLike | null>;
    cardDescription: FormControl<string>;
    mapInfo: FormControl<string>;
}
