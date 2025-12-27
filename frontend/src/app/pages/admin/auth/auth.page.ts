import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { TuiTextfield, TuiButton, TuiIcon, TuiError } from '@taiga-ui/core';
import { TuiFieldErrorPipe, TuiPassword, tuiValidationErrorsProvider } from '@taiga-ui/kit';
import { IAuthModel } from './interfaces/auth-model.interface';
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
    ],
    providers: [
        tuiValidationErrorsProvider({
            required: 'Поле обязательно для заполнения',
            email: 'Неверный формат электронной почты',
            invalidCredits: 'Неверный email или пароль',
            serverError: 'Сервис временно недоступен. Попробуйте позже'
        })
    ]
})
export class AuthPageComponent {
    protected readonly authForm: FormGroup = new FormGroup<IAuthModel>(
        {
            'email': new FormControl<string | null>(null, [Validators.required, Validators.email]),
            'password': new FormControl<string | null>(null, [Validators.required])
        },
        Validators.required
    );
    private readonly _authService: AuthService = inject(AuthService);
    private readonly _router: Router = inject(Router);

    /** Войти в аккаунт */
    protected login(): void {
        this.authForm.markAllAsTouched();

        if (this.authForm.valid) {
            const email: string = this.authForm.controls['email'].value;
            const password: string = this.authForm.controls['password'].value;

            this._authService.login(email, password)
                .pipe(take(1))
                .subscribe({
                    next: () => this._router.navigate(['admin']),
                    error: (error) => {
                        if (error?.status === 401 || error?.status === 403) {
                            this.authForm.setErrors({ invalidCredits: true });
                        } else {
                            this.authForm.setErrors({ serverError: true });
                        }
                    }
                });
        }
    }
}
