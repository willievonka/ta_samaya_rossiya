import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export const coordinatesValidator: ValidatorFn = (control: AbstractControl<string>): ValidationErrors | null => {
    const value: string = control.value;
    if (!value) {
        return null;
    }

    const parts: string[] = value
        .split(',')
        .map((item: string) => item.trim());
    if (parts.length !== 2 || !parts[0] || !parts[1]) {
        return { coordinatesFormatError: true };
    }

    const lat: number = Number(parts[0]);
    const lng: number = Number(parts[1]);
    if (Number.isNaN(lat) || Number.isNaN(lng)) {
        return { coordinatesFormatError: true };
    }
    if (lat < -90 || lat > 90 || lng < -180 || lng > 180) {
        return { coordinatesFormatError: true };
    }

    return null;
};
