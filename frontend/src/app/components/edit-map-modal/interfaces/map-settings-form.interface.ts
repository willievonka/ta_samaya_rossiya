import { FormControl } from '@angular/forms';
import { IAnalyticsMapSettingsForm } from './analytics-map-settings-form.interface';

export interface IMapSettingsForm extends IAnalyticsMapSettingsForm {
    layerWithPointsColor: FormControl<string>;
    pointsColor: FormControl<string>;
}
