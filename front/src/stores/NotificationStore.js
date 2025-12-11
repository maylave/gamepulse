
import { defineStore } from 'pinia'

export const useNotificationStore = defineStore('notification', {
  state: () => ({
    isVisible: false,
    message: '',
    type: 'info',
    duration: 4000 
  }),

  actions: {
    show(message, type = 'info', duration = this.duration) {
      this.message = message
      this.type = type
      this.isVisible = true

      if (duration > 0) {
        setTimeout(() => {
          this.hide()
        }, duration)
      }
    },

    showSuccess(message, duration = 3000) {
      this.show(message, 'success', duration)
    },

    showError(message, duration = 5000) {
      this.show(message, 'error', duration)
    },

    showWarning(message, duration = 4000) {
      this.show(message, 'warning', duration)
    },

    hide() {
      this.isVisible = false
    }
  }
})