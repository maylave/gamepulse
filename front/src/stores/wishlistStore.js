import { defineStore } from 'pinia'
import { api } from '@/services/api'

export const useWishlistStore = defineStore('wishlist', {
  state: () => ({
    items: new Set(),
    loading: false,
    error: null
  }),

  getters: {
    isGameInWishlist: (state) => (gameId) => state.items.has(gameId)
  },

  actions: {
    async toggleWishlist(game) {
      const wasIn = this.items.has(game.id)
      if (wasIn) {
        this.items.delete(game.id)
      } else {
        this.items.add(game.id)
      }

      const authStore = useAuthStore()
      if (authStore.isAuthenticated) {
        try {
          if (wasIn) {
            await api.wishlist.remove(game.id)
          } else {
            await api.wishlist.add(game.id)
          }
        } catch (err) {
          if (wasIn) this.items.add(game.id)
          else this.items.delete(game.id)
          throw err
        }
      } else {
        this.syncToLocalStorage()
      }
    },

    syncToLocalStorage() {
      const arr = Array.from(this.items)
      localStorage.setItem('wishlist', JSON.stringify(arr))
    },

    hydrateFromLocalStorage() {
      const saved = localStorage.getItem('wishlist')
      if (saved) {
        try {
          const ids = JSON.parse(saved)
          this.items = new Set(ids)
        } catch (e) {
          console.warn('Неверный формат wishlist в localStorage')
        }
      }
    },

    async fetchWishlist() {
      const authStore = useAuthStore()
      if (!authStore.isAuthenticated) return

      try {
        const data = await api.wishlist.get()
        const ids = data.map(item => item.gameId || item.id)
        this.items = new Set(ids)
      } catch (err) {
        console.error('Не удалось загрузить избранное', err)
      }
    }
  }
})