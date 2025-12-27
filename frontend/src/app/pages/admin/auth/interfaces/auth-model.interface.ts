import { FormControl } from '@angular/forms';

export interface IAuthModel {
    email: FormControl<string | null>,
    password: FormControl<string | null>
}
