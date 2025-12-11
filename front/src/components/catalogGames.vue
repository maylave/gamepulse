<template>
  <div class="games-infinite-list">
    <h2 v-if="title" class="section-title">{{ title }}</h2>

    <div v-if="loading && currentPage === 1" class="loading">Загрузка...</div>
    <div v-else-if="error" class="error">Ошибка загрузки: {{ error }}</div>
    <div v-else class="games-grid">
      <GameCard
        v-for="game in displayedGames"
        :key="game.id"
        :game="game"
        @add-to-cart="$emit('add-to-cart', $event)"
        @click.native="$emit('game-click', game)"
      />
    </div>

    <div v-if="!hasMore && displayedGames.length > 0" class="end-message">Показаны все игры</div>
    <button
      v-else-if="hasMore"
      @click="loadMore"
      class="load-more-btn"
      :disabled="loading"
    >
      {{ loading ? 'Загрузка...' : 'Загрузить ещё' }}
    </button>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import GameCard from '@/components/game-card.vue'

const props = defineProps({
  title: { type: String, default: '' },
  perPage: { type: Number, default: 10 },
  search: { type: String, default: null },
  category: { type: String, default: null },
  genreIds: { type: Array, default: () => [] },
  onSale: { type: Boolean, default: null },
  sortBy: { type: String, default: 'id' },
  ascending: { type: Boolean, default: true }
})

const emit = defineEmits(['add-to-cart', 'game-click'])

const games = ref([])
const currentPage = ref(1)
const total = ref(0)
const loading = ref(false)
const error = ref(null)

const hasMore = computed(() => {
  return currentPage.value * props.perPage < total.value
})

const displayedGames = computed(() => {
  return games.value
})

async function fetchGames(page) {
  if (loading.value) return

  loading.value = true
  error.value = null

  try {
    const params = new URLSearchParams({
      page: page,
      pageSize: props.perPage,
      sortBy: props.sortBy,
      ascending: props.ascending.toString()
    })

  
    if (props.search) params.append('search', props.search)
    if (props.category) params.append('category', props.category)
    if (props.onSale !== null) params.append('onSale', props.onSale.toString())
    if (props.genreIds?.length) {
      props.genreIds.forEach(id => params.append('genreIds', id))
    }

    const response = await fetch(`/api/games?${params.toString()}`)
    if (!response.ok) throw new Error('Не удалось загрузить игры')

    const data = await response.json()
    total.value = data.total

    if (page === 1) {
      games.value = data.items
    } else {
      games.value = [...games.value, ...data.items]
    }
  } catch (err) {
    error.value = err.message || 'Неизвестная ошибка'
    console.error('Ошибка загрузки игр:', err)
  } finally {
    loading.value = false
  }
}

async function loadMore() {
  if (hasMore.value) {
    currentPage.value++
    await fetchGames(currentPage.value)
  }
}

// Перезагружать данные при изменении фильтров (если нужно — раскомментировать)
// watch(() => [props.search, props.category, props.genreIds], () => {
//   currentPage.value = 1
//   fetchGames(1)
// }, { deep: true })


onMounted(() => {
  fetchGames(1)
})


defineExpose({
  loadMore,
  refresh: () => {
    currentPage.value = 1
    fetchGames(1)
  }
})
</script>

<style scoped lang="scss">
.games-infinite-list {
  max-width: 1200px;

  padding: 0 1rem;
}

.section-title {
  margin: 2rem 0 1.25rem;
  color: var(--text-primary, #fff);
  font-size: 1.5rem;
  text-align: left;
}

.games-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, min(max(250px, 20%), 1fr));
  gap: 1.5rem;
  justify-content: center;
  padding: 0.5rem 0;
  align-items: start;
}

// УБРАЛ max-width — он ломал сетку на мобильных!
@media (max-width: 768px) {
  .games-grid {
    grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
    gap: 1rem;
    // justify-items: center; ← НЕ НУЖНО, иначе карточки не растягиваются
  }

  .section-title {
    font-size: 1.3rem;
    margin: 1.5rem 0 1rem;
  }
}

@media (max-width: 480px) {
  .games-grid {
    grid-template-columns: repeat(auto-fill, minmax(130px, 1fr));
    gap: 0.8rem;
  }

  .games-infinite-list {
    padding: 0 0.8rem;
  }
}

.loading,
.error,
.end-message {
  text-align: center;
  padding: 1.5rem;
  color: var(--text-secondary, #aaa);
}

.error {
  color: #f44336;
}

.load-more-btn {
  display: block;
  margin: 1.5rem auto;
  padding: 0.75rem 2rem;
  background: var(--color-primary, #d32f2f);
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: opacity 0.2s;

  &:disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }

  &:not(:disabled):hover {
    opacity: 0.9;
  }
}
</style>