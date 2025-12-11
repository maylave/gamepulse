// src/stores/wishlistStore.js
import { defineStore } from 'pinia'
import { api } from '@/services/api'
import { useAuthStore } from '@/stores/auth'

export const useWishlistStore = defineStore('wishlist', {
  state: () => ({
    items: new Set(), // хранит gameId
    loading: false,
    error: null
  }),

  getters: {
    isGameInWishlist: (state) => (gameId) => {
      return state.items.has(gameId)
    },
    itemCount: (state) => state.items.size
  },

  actions: {
    // === ЕДИНЫЙ МЕТОД TOGGLE ===
    async toggleWishlist(game) {
      if (!game?.id) return

      const gameId = game.id
      const wasIn = this.items.has(gameId)

      // Оптимистичное обновление
      if (wasIn) {
        this.items.delete(gameId)
      } else {
        this.items.add(gameId)
      }

      // Синхронизируем с localStorage сразу
      this.syncToLocalStorage()

      const authStore = useAuthStore()

      // Если пользователь авторизован — синхронизируем с сервером
      if (authStore.isAuthenticated) {
        try {
          await api.wishlist.toggle(gameId)
          // Успешно — состояние уже обновлено
        } catch (err) {
          // Откатываем при ошибке
          if (wasIn) {
            this.items.add(gameId)
          } else {
            this.items.delete(gameId)
          }
          this.syncToLocalStorage()
          throw err
        }
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
          if (Array.isArray(ids)) {
            this.items = new Set(ids.map(id => Number(id)).filter(id => !isNaN(id)))
          }
        } catch (e) {
          console.warn('Неверный формат wishlist в localStorage')
        }
      }
    },

    
    async fetchWishlist() {
      const authStore = useAuthStore()
      if (!authStore.isAuthenticated) return

      this.loading = true
      this.error = null

      try {
        const data = await api.wishlist.get() 

        
        const ids = data.map(item => {
       
          return item.id 
        })

        this.items = new Set(ids)
        this.syncToLocalStorage() 
      } catch (err) {
        console.error('Не удалось загрузить избранное', err)
        this.error = err.message
      } finally {
        this.loading = false
      }
    },

    
    async init() {
      this.hydrateFromLocalStorage()

      const authStore = useAuthStore()
      if (authStore.isAuthenticated) {
        await this.fetchWishlist()
      }
    }
  }
})