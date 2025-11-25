
import { defineStore } from 'pinia'

export const useUiStore = defineStore('ui', {
  state: () => ({
    isLoading: false,
    loadingMessage: 'Загрузка...',
  }),

  actions: {
    startLoading(message = 'Загрузка...') {
      this.isLoading = true
      this.loadingMessage = message
    },

    endLoading() {
      this.isLoading = false
    },
  },
})