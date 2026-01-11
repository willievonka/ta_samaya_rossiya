import { FormControl } from '@angular/forms';

export interface IEditRegionForm {
    regionName: FormControl<string>;
    image: FormControl<File | null>;
    color: FormControl<string>;
    partnersCount: FormControl<number>;
    excursionsCount: FormControl<number>;
    membersCount: FormControl<number>;
}
