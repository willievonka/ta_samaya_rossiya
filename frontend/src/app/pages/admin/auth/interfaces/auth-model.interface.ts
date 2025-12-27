import { FormControl } from '@angular/forms';

export interface IAuthModel {
    login: FormControl<string | null>,
    password: FormControl<string | null>
}
