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
  server: {
    port: 8081,
    proxy: {
      '/api': {
        target: 'http://localhost',
        changeOrigin: true,
        secure: false,
      },
     
      '/uploads': {
        target: 'http://localhost',
        changeOrigin: true,
        secure: false,
      },
      '/images': {
        target: 'http://localhost',
        changeOrigin: true,
        secure: false,
      }
    }
  }
})