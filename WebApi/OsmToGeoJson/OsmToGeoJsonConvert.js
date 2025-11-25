// OsmToGeoJson/OsmToGeoJsonConvert.js
const osmtogeojson = require('osmtogeojson');

let inputData = '';

// Включаем чтение
process.stdin.setEncoding('utf8');
process.stdin.resume();

process.stdin.on('data', (chunk) => {
    inputData += chunk;
});

process.stdin.on('end', () => {
    if (!inputData.trim()) {
        process.stderr.write('Error: Empty input\n');
        process.exit(1);
        return;
    }

    let osmObj;
    try {
        osmObj = JSON.parse(inputData); // Прямо парсим OSM JSON
    } catch (err) {
        process.stderr.write(`JSON parse error: ${err.message}\n`);
        process.exit(1);
        return;
    }

    try {
        const geojson = osmtogeojson(osmObj); // Работает с OSM JSON
        process.stdout.write(JSON.stringify(geojson, null, 2) + '\n');
        process.exit(0);
    } catch (convertErr) {
        process.stderr.write(`Conversion error: ${convertErr.message}\n`);
        process.exit(1);
    }
});

process.stdin.on('error', (err) => {
    process.stderr.write(`Input error: ${err.message}\n`);
    process.exit(1);
});