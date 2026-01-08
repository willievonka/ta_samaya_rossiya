import { FormControl } from '@angular/forms';
import { TuiFileLike } from '@taiga-ui/kit';

export interface IEditRegionForm {
    regionName: FormControl<string>;
    image: FormControl<TuiFileLike | null>;
    color: FormControl<string>;
    partnersCount: FormControl<number>;
    excursionsCount: FormControl<number>;
    membersCount: FormControl<number>;
}
