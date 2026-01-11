import type { IEnvironment } from '../interfaces/environment.interface';

export const environment: IEnvironment = {
    production: false,
    host: 'http://localhost:8080',
    clientApiUrl: 'http://localhost:8080/api/client',
    adminApiUrl: 'http://localhost:8080/api/admin',
};
