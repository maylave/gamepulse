// src/stores/cartStore.js
import { defineStore } from 'pinia'
import { api } from '@/services/api' 

export const useCartStore = defineStore('cart', {
  state: () => ({
    items: [],
    loading: false,
    error: null,
    initialized: false
  }),

  getters: {
    cartItems: (state) => state.items,
    total: (state) =>
      state.items.reduce((sum, item) => sum + item.price * item.quantity, 0),
    itemCount: (state) =>
      state.items.reduce((count, item) => count + item.quantity, 0)
  },

  actions: {
  
    async fetchCart() {
      if (this.initialized) return

      this.loading = true
      this.error = null

      try {
 
        const response = await api.cart.get()
       
        this.items = Array.isArray(response?.items) ? response.items : response || []
        this.initialized = true
      } catch (err) {
        this.error = err.message || 'Не удалось загрузить корзину'
        console.error('[Cart] Ошибка загрузки:', err)
        this.items = [] // или оставить как есть — по вашему выбору
      } finally {
        this.loading = false
      }
    },

  
    async addToCart(game) {
     
      const existing = this.items.find(item => item.id === game.id)
      if (existing) {
        existing.quantity += 1
      } else {
        this.items.push({ ...game, quantity: 1 })
      }

    
      try {
        await api.cart.add(game.id)
      } catch (err) {
     
        if (existing) {
          existing.quantity -= 1
        } else {
          this.items.pop()
        }
        this.error = err.message
        console.error('[Cart] Ошибка добавления:', err)
      }
    },

    // Обновление количества
    async updateQuantity({ id, quantity }) {
      if (quantity <= 0) {
        await this.removeFromCart(id)
        return
      }

      const item = this.items.find(i => i.id === id)
      if (!item) return

      const oldQty = item.quantity
      item.quantity = quantity

      try {
      
        await api.cart.update?.(id, { quantity }) // опционально, если метод есть
      
      } catch (err) {
        item.quantity = oldQty
        this.error = err.message
      }
    },

    
    async removeFromCart(id) {
      const original = [...this.items]
      this.items = this.items.filter(i => i.id !== id)

      try {
        await api.cart.remove(id)
      } catch (err) {
        this.items = original
        this.error = err.message
      }
    },

    // Очистка
    async clearCart() {
      const original = [...this.items]
      this.items = []

      try {
        await api.cart.clear()
      } catch (err) {
        this.items = original
        this.error = err.message
      }
    }
  }
})