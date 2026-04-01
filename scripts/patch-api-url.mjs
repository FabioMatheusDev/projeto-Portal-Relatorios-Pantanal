/**
 * Substitui __API_PUBLIC_URL__ em environment.ts antes do ng build.
 * No Netlify: defina API_PUBLIC_URL (URL pública do ASP.NET, com /api).
 * Local: sem NETLIFY, usa http://localhost:5248/api se API_PUBLIC_URL não estiver definida.
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
  console.error(`
[build] Defina API_PUBLIC_URL nas variáveis de ambiente do site Netlify
  (Site configuration → Environment variables).

Exemplo de valor (ajuste para o host onde a API ASP.NET está publicada):
  https://seu-app.azurewebsites.net/api
`);
  process.exit(1);
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
