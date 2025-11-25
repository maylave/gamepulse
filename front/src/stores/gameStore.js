// src/stores/gameStore.js
import { defineStore } from 'pinia'
import { api } from '@/services/api'

export const useGameStore = defineStore('game', {
  state: () => ({
    game: null,
    loading: false,
    error: null
  }),

  actions: {
    async fetchGame(id) {
      this.loading = true
      this.error = null

      try {
        const game = await api.games.getById(id)
        if (game) {
          this.game = game
          return { success: true }
        } else {
          throw new Error('Игра не найдена')
        }
      } catch (err) {
        this.error = err.message || 'Не удалось загрузить игру'
        console.error('[GameStore] Ошибка:', err)
        return { success: false }
      } finally {
        this.loading = false
      }
    },

    clear() {
      this.game = null
      this.error = null
    }
  }
})