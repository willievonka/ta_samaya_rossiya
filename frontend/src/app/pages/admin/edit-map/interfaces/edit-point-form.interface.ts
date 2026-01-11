import { FormControl } from '@angular/forms';

export interface IEditPointForm {
    pointName: FormControl<string>;
    regionName: FormControl<string>;
    coordinates: FormControl<[number, number]>;
    image: FormControl<File | null>;
    year: FormControl<number>;
    description: FormControl<string>;
    excursionUrl: FormControl<string>;
}
