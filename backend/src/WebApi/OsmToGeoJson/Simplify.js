const mapshaper = require('mapshaper');
const fs = require('fs');
const path = require('path');

const INPUT_FILE  = '../wwwroot/maps/map.geojson';
const OUTPUT_FILE = '../wwwroot/maps/simplifiedMap.geojson';
const KEEP_PERCENT = '20%';
const METHOD = 'visvalingam';

if (!fs.existsSync(INPUT_FILE)) {
    console.error(`Ошибка: файл "${INPUT_FILE}" не найден!`);
    process.exit(1);
}

const geojsonString = fs.readFileSync(INPUT_FILE, 'utf8');
const fileName = path.basename(INPUT_FILE);

// В inputFiles ключом делаем просто имя файла
const inputFiles = {
    [fileName]: geojsonString
};

// В командах НЕ используем -o для записи на диск, 
// вместо этого просим mapshaper выдать результат в формате geojson
const commands = `-i ${fileName} -simplify ${METHOD} weighted ${KEEP_PERCENT} -o output.json format=geojson`;

console.log(`Запускаем упрощение...`);

mapshaper.applyCommands(commands, inputFiles, (err, output) => {
    if (err) {
        console.error('Ошибка mapshaper:', err);
        return;
    }

    // Mapshaper вернет объект, где ключи — это имена файлов из команды -o
    if (output && output['output.json']) {
        try {
            // Создаем папку, если её нет
            const dir = path.dirname(OUTPUT_FILE);
            if (!fs.existsSync(dir)) {
                fs.mkdirSync(dir, { recursive: true });
            }

            fs.writeFileSync(OUTPUT_FILE, output['output.json']);
            console.log(`Успешно сохранено! Файл: ${path.resolve(OUTPUT_FILE)}`);
        } catch (fsErr) {
            console.error('Ошибка при записи файла на диск:', fsErr);
        }
    } else {
        console.error('Ошибка: Mapshaper не вернул данные. Проверьте синтаксис команд.');
    }
});