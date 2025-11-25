import { defineStore } from 'pinia'
import { api } from '@/services/api'

export const useAdminStore = defineStore('admin', {
  state: () => ({
    users: [],
    games: [],
    genres: [], // ✅ добавлено
    loading: false
  }),

  actions: {
   
    async fetchUsers() {
      this.loading = true
      try {
        this.users = await api.admin.users.getAll()
      } catch (error) {
        console.error('Ошибка загрузки пользователей:', error)
        this.users = []
      } finally {
        this.loading = false
      }
    },

    async createUser(data) {
      await api.admin.users.create(data)
      await this.fetchUsers()
    },

    async updateUser(id, data) {
      await api.admin.users.update(id, data)
      await this.fetchUsers()
    },

    async deleteUser(id) {
      await api.admin.users.delete(id)
      await this.fetchUsers()
    },

 
    async fetchGames() {
      this.loading = true
      try {
        this.games = await api.admin.games.getAll()
      } catch (error) {
        console.error('Ошибка загрузки игр:', error)
        this.games = []
      } finally {
        this.loading = false
      }
    },

    async createGame(data) { 
      await api.admin.games.create(data)
      await this.fetchGames()
    },

    async updateGame(id, data) { 
      await api.admin.games.update(id, data)
      await this.fetchGames()
    },

    async deleteGame(id) { 
      await api.admin.games.delete(id)
      await this.fetchGames()
    },

  
    async fetchGenres() {
      this.loading = true
      try {
        this.genres = await api.admin.genres.getAll()
      } catch (error) {
        console.error('Ошибка загрузки жанров:', error)
        this.genres = []
      } finally {
        this.loading = false
      }
    },

    async createGenre(data) {
      await api.admin.genres.create(data)
      await this.fetchGenres()
    },

    async updateGenre(id, data) {
      await api.admin.genres.update(id, data)
      await this.fetchGenres()
    },

    async deleteGenre(id) {
      await api.admin.genres.delete(id)
      await this.fetchGenres()
    }
  }
})