<template>
  <div class="game-grid">
    <h2 v-if="title" class="game-grid__title">{{ title }}</h2>

    <div class="game-grid-container">
      <GameCard
        v-for="(game, index) in displayedGames"
        :key="game.id || `_game_${index}`"
        :game="game"
        :isPlaceholder="!!game.isPlaceholder"
        :isAddNew="!!game.isAddNew"
        @add-to-cart="$emit('add-to-cart', $event)"
        @game-click="$emit('game-click', game)"
        @createNewGame="openCreateGameModal"
        @loadMore="loadMoreGames"
        class="game-grid__item"
      />
    </div>


    <div v-if="isLoading && !hasLoadedAll" class="game-grid__loader">
      <div class="spinner"></div>
    </div>


    <div v-else-if="displayedGames.length === 0 && !isLoading" class="game-grid__empty">
      Игры в категории "{{ category || 'все' }}" не найдены.
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import GameCard from '@/components/game-card.vue'

const props = defineProps({
  games: { type: Array, default: () => [] },
  title: { type: String, default: '' },
  category: { type: String, default: '' }, 
  showAddNew: { type: Boolean, default: true },
  initialLimit: { type: Number, default: 12 } 
})

const emit = defineEmits(['add-to-cart', 'game-click', 'load-more'])


const isLoading = ref(false)
const hasLoadedAll = ref(false)
const displayedCount = ref(props.initialLimit)


const displayedGames = computed(() => {
  let items = props.games.slice(0, displayedCount.value)

  if (props.showAddNew) {
    items.push({ isAddNew: true })
  }

  return items
})


const handleScroll = () => {
  if (isLoading.value || hasLoadedAll.value) return

  const scrollBottom = window.innerHeight + window.scrollY
  const bodyHeight = document.body.offsetHeight

 
  if (scrollBottom >= bodyHeight - 300) {
    loadMoreGames()
  }
}

const loadMoreGames = () => {
  if (isLoading.value || hasLoadedAll.value) return

  isLoading.value = true

 
  emit('load-more', {
    category: props.category,
    skip: displayedGames.value.length,
    limit: 12
  })

 
  setTimeout(() => {
    const newCount = displayedCount.value + 12
    if (newCount >= props.games.length) {
      hasLoadedAll.value = true
    }
    displayedCount.value = newCount
    isLoading.value = false
  }, 600)
}

const openCreateGameModal = () => {

  console.log('Открыть модал создания игры')
}

// Подписка на скролл
onMounted(() => {
  window.addEventListener('scroll', handleScroll)
})

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
})
</script>

<style scoped>
.game-grid {
  margin-bottom: 3.5rem;
 
}

.game-grid__title {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--text-primary, #fff);
  margin: 2.5rem 0 1.5rem;
}

.game-grid-container {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 1.5rem;
  padding: 0 20px;
}

@media (min-width: 1400px) {
  .game-grid-container {
    grid-template-columns: repeat(6, 1fr); 
  }
}

@media (max-width: 768px) {
  .game-grid__container {
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
    padding: 0 12px;
  }
}

.game-grid__loader,
.game-grid__empty {
  text-align: center;
  padding: 2rem;
  color: var(--text-secondary, #aaa);
}

.spinner {
  width: 24px;
  height: 24px;
  border: 3px solid rgba(255, 255, 255, 0.3);
  border-top: 3px solid var(--color-primary, #e53e3e);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>