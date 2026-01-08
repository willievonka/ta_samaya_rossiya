import { TuiFileLike } from '@taiga-ui/kit';

/**
 * Сравнить два файла
 * @param a
 * @param b
 */
export const compareFiles = (a: TuiFileLike | File | null, b: TuiFileLike | File | null): boolean => {
    const fa: File | null = a as File | null;
    const fb: File | null = b as File | null;

    return (
        fa?.name === fb?.name &&
        fa?.size === fb?.size &&
        fa?.lastModified === fb?.lastModified
    );
};
