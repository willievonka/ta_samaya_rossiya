const mapshaper = require('mapshaper');

let inputJson = '';

process.stdin.on('data', chunk => inputJson += chunk);

process.stdin.on('end', () => {
    try {
        const input = { 'input.geojson': inputJson };
        const command = `
            -i input.geojson
            -simplify visvalingam weighted 10%
            -o output.geojson
        `;

        mapshaper.applyCommands(command, input, function(err, output) {
            if (err) {
                console.error('Error:', err);
                process.exit(1);
            }
            // Преобразуем результат в строку GeoJSON и выводим
            const simplifiedGeoJson = output['output1.geojson'].toString();
            console.log(simplifiedGeoJson);
        });
    } catch (err) {
        console.error('Error:', err);
        process.exit(1);
    }
});