import { FormControl } from '@angular/forms';
import { TuiFileLike } from '@taiga-ui/kit';

export interface IEditPointForm {
    pointName: FormControl<string>;
    coordinates: FormControl<[number, number]>;
    image: FormControl<TuiFileLike | null>;
    year: FormControl<number>;
    description: FormControl<string>;
    excursionUrl: FormControl<string>;
}
