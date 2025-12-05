// src/stores/superUser.js
import { defineStore } from 'pinia'
import { api } from '@/services/api'

export const useSuperUserStore = defineStore('superUser', {
  state: () => ({
    games: [],
    genres: [],
    loading: false
  }),

  actions: {
    async fetchGenres() {
      this.loading = true
      try {

        this.genres = await api.superUser.genres.getAll()
      } catch (error) {
        console.error('Ошибка загрузки жанров:', error)
        this.genres = []
      } finally {
        this.loading = false
      }
    },

    async createGame(data) {
      this.loading = true
      try {
       
       await api.superUser.games.create(data)
      } finally {
        this.loading = false
      }
    }
  }
})