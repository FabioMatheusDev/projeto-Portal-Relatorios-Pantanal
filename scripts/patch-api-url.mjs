/**
 * Substitui __API_PUBLIC_URL__ em environment.ts antes do ng build.
 * - API_PUBLIC_URL (Netlify): URL absoluta da API ASP.NET, terminando em /api.
 * - Sem variável no Netlify: usa /api (build ok; configure a variável para o login funcionar).
 * - Local: http://localhost:5248/api se API_PUBLIC_URL não estiver definida.
 */
import { readFileSync, writeFileSync } from 'fs';
import { dirname, join } from 'path';
import { fileURLToPath } from 'url';

const __dirname = dirname(fileURLToPath(import.meta.url));
const envFile = join(__dirname, '../src/PortalRelatorios.Web/src/environments/environment.ts');

const isNetlify = process.env.NETLIFY === 'true';
const fromEnv = process.env.API_PUBLIC_URL?.trim();

let apiUrl;
if (fromEnv) {
  apiUrl = fromEnv;
} else if (isNetlify) {
  // Permite o build sem variável; login só funciona após definir API_PUBLIC_URL + redeploy
  // (URL absoluta da API ASP.NET, ex.: https://sua-api.azurewebsites.net/api).
  console.warn(
    '\n[patch-api-url] API_PUBLIC_URL não definida no Netlify — usando /api. O login falhará até você ' +
      'criar a variável (Site → Environment variables) com a URL pública da API e publicar de novo.\n',
  );
  apiUrl = '/api';
} else {
  apiUrl = 'http://localhost:5248/api';
}

let content = readFileSync(envFile, 'utf8');
if (!content.includes('__API_PUBLIC_URL__')) {
  console.warn('[patch-api-url] Placeholder __API_PUBLIC_URL__ não encontrado; nada alterado.');
  process.exit(0);
}

content = content.replace(/__API_PUBLIC_URL__/g, apiUrl);
writeFileSync(envFile, content);
console.log('[patch-api-url] apiUrl =', apiUrl);
