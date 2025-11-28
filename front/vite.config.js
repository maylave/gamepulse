// vite.config.js
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': '/src',
      '@views': '/src/views'
    }
  },
  // === ДОБАВЛЕНО: прокси для API в режиме разработки ===
  server: {
    port: 8081,
    proxy: {
      '/api': {
        target: 'http://localhost', // ← трафик /api идёт на nginx (порт 80)
        changeOrigin: true,
        secure: false,
        // rewrite: (path) => path  // не нужно, если nginx уже обрабатывает /api
      }
    }
  }
})