import { defineConfig } from 'vite';
import { resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = fileURLToPath(new URL('.', import.meta.url));

export default defineConfig({
  build: {
    rollupOptions: {
      input: {
        main: resolve(root, 'index.html'),
        info: resolve(root, 'info.html'),
        settings: resolve(root, 'settings.html'),
        settings: resolve(root, 'settings.html'),
      },
    },
  },
});
