export const environment = {
  production: true,
  /** Substituído em CI (Netlify) por scripts/patch-api-url.mjs → variável API_PUBLIC_URL */
  apiUrl: '__API_PUBLIC_URL__',
};
