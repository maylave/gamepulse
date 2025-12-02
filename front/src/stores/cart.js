import { defineStore } from 'pinia'
import { api } from '@/services/api'
import { startTransition } from 'react';

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
    state.items.reduce((sum, item) => {
      
      const price = Number(item.price) || 0;
      console.log(price)
      const quantity = Number(item.quantity) || 1;
       console.log(quantity)
       console.log(sum)
      return sum + (price * quantity);
    }, 0),



  sumCart: (state) =>
state.items.reduce(( item) => {
      
      const price = Number(item.price) || 0;
      console.log(price)
      const quantity = Number(item.quantity) || 1;
       console.log(quantity)
      
      return  (price * quantity);
    }, 0),

  itemCount: (state) =>
    
    state.items.reduce((count, item) => Number(count) + Number(item.quantity), 0)
},


  actions: {
    async fetchCart() {

      this.loading = true
      this.error = null

      try {
        const response = await api.cart.get()

     
        if (Array.isArray(response)) {
          this.items = response
        } else if (response?.items && Array.isArray(response.items)) {
          this.items = response.items
        } else {
          this.items = []
        }

        this.initialized = true
      } catch (err) {
        if (err.response?.status === 404) {
         
          this.items = []
          this.initialized = true
          console.log('[Cart] Корзина не найдена, инициализирована пустая')
        } else {
          this.error = err.message || 'Не удалось загрузить корзину'
          console.error('[Cart] Ошибка загрузки:', err)
          this.items = []
        }
      } finally {
        this.loading = false
      }
    },

    async addToCart(game) {
      if (!game.id) {
        console.error('Товар не имеет ID!', game)
        return
      }
       if (typeof game.price !== 'number' || isNaN(game.price)) {
    console.error('Некорректная цена товара:', game.price);
    return;
  }
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

    async updateQuantity({ id, quantity }) {
      if (quantity <= 0) {
        await this.removeFromCart(id)
        return
      }

      const item = this.items.find(i => i.id === id)
      if (!item) {
        console.warn(`Товар с ID ${id} не найден в корзине`)
        return
      }

      const oldQty = item.quantity
      item.quantity = quantity

      try {
        await api.cart.update(id, quantity)
      } catch (err) {
        item.quantity = oldQty
        this.error = err.message
        console.error('[Cart] Ошибка обновления количества:', err)
      }
    },

    async removeFromCart(id) {
      const itemIndex = this.items.findIndex(i => i.id === id)
      if (itemIndex === -1) {
        console.warn(`Товар с ID ${id} уже удалён`)
        return
      }

      // Сохраняем только удаляемый товар
      const removedItem = this.items[itemIndex]
      this.items.splice(itemIndex, 1)

      try {
        await api.cart.remove(id)
      } catch (err) {
        // Возвращаем товар на прежнее место
        this.items.splice(itemIndex, 0, removedItem)
        this.error = err.message
        console.error('[Cart] Ошибка удаления:', err)
      }
    },

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
