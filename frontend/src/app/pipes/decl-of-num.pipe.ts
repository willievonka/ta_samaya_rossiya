import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'declOfNum',
    standalone: true
})
export class DeclOfNumPipe implements PipeTransform {
    /**
     * Склонение слова в зависимости от числа
     * @param value - число
     * @param titles - массив форм по падежам
     */
    public transform(value: number | undefined | null, titles: [string, string, string]): string {
        if (!value && value !== 0) {
            return '';
        }
        const cases: number[] = [2, 0, 1, 1, 1, 2];

        return titles[
            (value % 100 > 4 && value % 100 < 20)
                ? 2
                : cases[Math.min(value % 10, 5)]
        ];
    }
}
