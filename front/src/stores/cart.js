import { defineStore } from 'pinia'
import { api } from '@/services/api'


export const useCartStore = defineStore('cart', {
  state: () => ({
    lastOrderEmail: '',
    items: [],
    loading: false,
    error: null,
    initialized: false,
    sum: 0
  }),

getters: {
            
  cartItems: (state) =>
  state.items
    .filter(item => item != null && item.game != null)
    .map(item => {
     
      let genre = item.game.genre
      if (!genre) {
        genre = []
      } else if (!Array.isArray(genre)) {
       
        genre = [String(genre).trim()]
      }

      return {
        id: item.id,
        gameId: item.game.id,
        title: item.game.title,
        price: Number(item.game.price) || 0,
        image: item.game.imageUrl || '',
        genre, 
        quantity: Number(item.quantity) || 1
      }
    }),
total: (state) =>
    state.cartItems.reduce((sum, item) => sum + item.price * item.quantity, 0),



  sumCart: (state) =>
state.items.reduce(( item) => {
      
      const price = Number(item.price) || 0;
      
      const quantity = Number(item.quantity) || 1;
    
      
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
         
        } else {
          this.error = err.message || 'Не удалось загрузить корзину'
          
          this.items = []
        }
      } finally {
        this.loading = false
      }
    },

    async addToCart(game) {
      if (!game.id) {
        return
      }
       if (typeof game.price !== 'number' || isNaN(game.price)) {
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
        this.fetchCart()
      } catch (err) {
        if (existing) {
          existing.quantity -= 1
        } else {
          this.items.pop()
        }
        this.error = err.message
      }
    },

    async updateQuantity({ id, quantity }) {
      if (quantity <= 0) {
        await this.removeFromCart(id)
        return
      }

      const item = this.items.find(i => i.id === id)
      if (!item) {
       
        return
      }

      const oldQty = item.quantity
      item.quantity = quantity

      try {
        await api.cart.update(id, quantity)
      } catch (err) {
        item.quantity = oldQty
        this.error = err.message
      
      }
    },

    async removeFromCart(id) {
      const itemIndex = this.items.findIndex(i => i.id === id)
      if (itemIndex === -1) {
       
        return
      }


      const removedItem = this.items[itemIndex]
      this.items.splice(itemIndex, 1)

      try {
        await api.cart.remove(id)
      } catch (err) {
       
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
