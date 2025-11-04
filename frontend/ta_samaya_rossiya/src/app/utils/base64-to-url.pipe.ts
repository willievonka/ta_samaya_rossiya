import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'base64ToUrl',
    standalone: true
})
export class Base64ToUrlPipe implements PipeTransform {
    /** @inheritdoc */
    public transform(base64: string | null | undefined): string {
        if (!base64) {
            return '';
        }
        if (base64.startsWith('data:')) {
            return `url(${base64})`;
        }

        return `url(data:image/svg+xml;base64,${base64})`;
    }
}
