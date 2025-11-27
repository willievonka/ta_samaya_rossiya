const mapshaper = require('mapshaper');
const fs = require('fs');
const path = require('path');

// ==================== НАСТРОЙКИ ====================
const INPUT_FILE  = '../wwwroot/maps/map.geojson';   // входной файл
const OUTPUT_FILE = '../wwwroot/maps/simplifiedMap.geojson';  // куда сохранять результат
const KEEP_PERCENT = '20%';            // насколько упрощать (10% от исходного количества точек)
const METHOD = 'visvalingam';          // или 'dp' (Douglas-Peucker)
// ===================================================

// Проверяем существование входного файла
if (!fs.existsSync(INPUT_FILE)) {
    console.error(`Ошибка: файл "${INPUT_FILE}" не найден!`);
    process.exit(1);
}

console.log(`Читаем файл: ${INPUT_FILE}`);

// Читаем файл в память как строку (mapshaper принимает именно строки)
const geojsonString = fs.readFileSync(INPUT_FILE, 'utf8');

// Формируем объект, который ожидает mapshaper
const inputFiles = {
    [path.basename(INPUT_FILE)]: geojsonString
};

// Команда mapshaper (можно менять под свои нужды)
const commands = `
    -i "${path.basename(INPUT_FILE)}"
    -simplify ${METHOD} weighted ${KEEP_PERCENT}
    -o "${OUTPUT_FILE}" format=geojson
`;

console.log(`Запускаем упрощение (${KEEP_PERCENT} точек)...`);

mapshaper.applyCommands(commands, inputFiles, (err, output) => {
    if (err) {
        console.error('Ошибка mapshaper:', err);
        process.exit(1);
    }

    // Если указали -o с именем файла, mapshaper сам сохранит его,
    // но если вдруг вывел в output-объект — запишем вручную
    if (output && output[OUTPUT_FILE]) {
        fs.writeFileSync(OUTPUT_FILE, output[OUTPUT_FILE]);
        console.log(`Успешно сохранено в ${OUTPUT_FILE}`);
    } else {
        // В большинстве случаев файл уже записан на диск
        console.log(`Готово! Результат: ${path.resolve(OUTPUT_FILE)}`);
    }
});