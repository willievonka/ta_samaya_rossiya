/**
 * Делает переданный цвет в формате hex ярче на deltaPercent
 * @param hexColor
 * @param deltaPercent
 */
export function makeBrighterColor(hexColor: string, deltaPercent: number): string {
    let clean: string = hexColor.replace('#', '').trim().toLowerCase();
    if (clean.length === 3) {
        clean = clean
            .split('')
            .map(c => c + c)
            .join('');
    }

    const hasAlpha: boolean = clean.length === 8;

    let r: number = parseInt(clean.slice(0, 2), 16);
    let g: number = parseInt(clean.slice(2, 4), 16);
    let b: number = parseInt(clean.slice(4, 6), 16);
    const a: number | null = hasAlpha ? parseInt(clean.slice(6, 8), 16) : null;

    const factor: number = 1 + Math.max(0, Math.min(deltaPercent, 100)) / 100;

    r = Math.min(255, Math.round(r * factor));
    g = Math.min(255, Math.round(g * factor));
    b = Math.min(255, Math.round(b * factor));

    const toHex = (v: number): string => v.toString(16).padStart(2, '0');

    let result: string = `#${toHex(r)}${toHex(g)}${toHex(b)}`;
    if (a !== null) {
        result += toHex(a);
    }

    return result.toUpperCase();
}
