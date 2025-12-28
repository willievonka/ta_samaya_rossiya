import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { TuiTextfield, TuiButton, TuiIcon, TuiError } from '@taiga-ui/core';
import { TuiFieldErrorPipe, TuiPassword } from '@taiga-ui/kit';
import { IAuthForm, IAuthFormErrors } from './interfaces/auth-form.interface';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AsyncPipe } from '@angular/common';
import { AuthService } from '../../../services/auth.service';
import { take } from 'rxjs';



@Component({
    selector: 'auth-page',
    standalone: true,
    templateUrl: './auth.page.html',
    styleUrl: './styles/auth-page.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        AsyncPipe,
        ReactiveFormsModule,
        TuiTextfield,
        TuiButton,
        TuiIcon,
        TuiPassword,
        TuiError,
        TuiFieldErrorPipe,
        RouterLink
    ]
})
export class AuthPageComponent {
    protected readonly authForm: FormGroup<IAuthForm> = new FormGroup<IAuthForm>({
        email: new FormControl<string>('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
        password: new FormControl<string>('', { nonNullable: true, validators: [Validators.required] })
    });

    private readonly _authService: AuthService = inject(AuthService);
    private readonly _router: Router = inject(Router);

    /** Войти в аккаунт */
    protected login(): void {
        this.authForm.markAllAsTouched();
        this.authForm.setErrors(null);

        if (this.authForm.invalid) {
            return;
        }

        const { email, password }: { email: string; password: string } = this.authForm.getRawValue();

        this._authService.login(email, password)
            .pipe(take(1))
            .subscribe({
                next: () => this._router.navigate(['admin']),
                error: (error) => {
                    const errors: IAuthFormErrors =
                        (error?.status === 401 || error?.status === 403)
                            ? { invalidCredits: true }
                            : { serverError: true };

                    this.authForm.setErrors(errors);
                }
            });
    }
}
