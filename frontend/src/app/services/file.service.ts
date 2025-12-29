/* eslint-disable @typescript-eslint/no-empty-function */
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { DomSanitizer, SafeStyle } from '@angular/platform-browser';
import { TuiFileLike } from '@taiga-ui/kit';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class FileService {
    private readonly _http: HttpClient = inject(HttpClient);
    private readonly _sanitizer: DomSanitizer = inject(DomSanitizer);

    /**
     * Скачать файл по ссылке
     * @param url
     * @param fileName
     * @param mime
     */
    public downloadAsFile(url: string, fileName: string, mime?: string): Observable<File> {
        return this._http.get(url, { responseType: 'blob' })
            .pipe(
                map(blob => new File([blob], fileName, { type: mime ?? blob.type }))
            );
    }

    /**
     * Получить название файла по url
     * @param url
     */
    public getFileNameFromUrl(url: string): string | null {
        try {
            const u: URL = new URL(url, window.location.origin);
            const last: string | undefined = u.pathname.split('/').filter(Boolean).pop();

            return last ? decodeURIComponent(last) : null;
        } catch {
            const last: string | undefined = url.split('?')[0]?.split('#')[0]?.split('/').pop();

            return last ?? null;
        }
    }

    /**
     * Собрать стиль background-image из файла
     * @param file
     */
    public buildBackgroundImagePreview(file: TuiFileLike | null): { style: SafeStyle | null; revoke: () => void } {
        if (!file) {
            return { style: null, revoke: (): void => {} };
        }

        const objectUrl: string = URL.createObjectURL(file as File);
        const style: SafeStyle = this._sanitizer.bypassSecurityTrustStyle(`url("${objectUrl}")`);

        return {
            style,
            revoke: () => URL.revokeObjectURL(objectUrl)
        };
    }
}
