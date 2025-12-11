<template>
  <div class="history-page">
    <catalogGames
      title="Недавно просмотренные"
      mode="history"
      @game-click="handleGameClick"
    />
    
 
    <div class="actions" v-if="hasHistory">
      <button @click="clearHistory" class="btn btn-danger">Очистить историю</button>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import catalogGames from '@/components/catalogGames.vue'

const router = useRouter()

// Проверяем, есть ли что-то в истории
const hasHistory = computed(() => {
  const raw = localStorage.getItem('viewedGames')
  return raw ? JSON.parse(raw).length > 0 : false
})

// Обработка клика по игре (переход на страницу игры)
function handleGameClick(game) {
  router.push(`/game/${game.id}`)
}

// Очистка всей истории
function clearHistory() {
  if (confirm('Вы уверены, что хотите очистить всю историю?')) {
    localStorage.removeItem('viewedGames')
    // GamesInfiniteList сам обновится при следующем рендере (или можно вызвать refresh через ref)
  }
}
</script>

<style scoped>
/* Ваши стили — но упростим: уберём дублирующие, оставим только обёртку и действия */

.history-page {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.actions {
  text-align: center;
  margin-top: 2rem;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
}

.btn-danger {
  background: #f44336;
  color: white;
}

.btn-danger:hover {
  background: #e53935;
}
</style>