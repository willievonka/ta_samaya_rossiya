import { FormControl, ValidationErrors } from '@angular/forms';

/**
 * Убрать ошибку из контрола по ключу
 * @param control
 * @param key
 */
export const clearControlError = (control: FormControl<string>, key: string): void => {
    const errors: ValidationErrors | null = control.errors;
    if (!errors || !errors[key]) {
        return;
    }

    delete errors[key];
    control.setErrors(Object.keys(errors).length ? errors : null);
};
