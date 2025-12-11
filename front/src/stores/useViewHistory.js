// composables/useViewHistory.js
import { ref, onMounted } from 'vue'

const MAX_HISTORY = 6
const STORAGE_KEY = 'viewHistory'

export function useViewHistory() {
  const history = ref([])

  const addToHistory = (game) => {
    if (!game || !game.id) return

    const existing = history.value.findIndex(item => item.id === game.id)
    if (existing >= 0) {
      history.value.splice(existing, 1) 
    }

    history.value.unshift(game) 
    history.value = history.value.slice(0, MAX_HISTORY)

    localStorage.setItem(STORAGE_KEY, JSON.stringify(history.value))
  }

  const loadHistory = () => {
    try {
      const saved = localStorage.getItem(STORAGE_KEY)
      history.value = saved ? JSON.parse(saved) : []
    } catch (e) {
      history.value = []
    }
  }

  loadHistory()

  return {
    history,
    addToHistory
  }
}