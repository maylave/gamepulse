<template>
  <div class="app">
    <Header />

    <main class="container containerCatalog">
      <h1 class="page-title">Каталог игр</h1>

      <!-- Поиск + Фильтры -->
      <div class="search-filters-bar">
        <div class="search-wrapper">
          <div class="search-box">
            <i class="fas fa-search"></i>
            <input 
              v-model="localQuery" 
              type="text" 
              placeholder="Поиск по названию..." 
              @input="debouncedSearch"
            />
          </div>
        </div>

        <button class="filters-toggle" @click="toggleFilters">
          <i class="fas fa-filter"></i> Фильтры {{ showFilters ? '▲' : '▼' }}
        </button>
      </div>

      <!-- Фильтры (свёрнуты по умолчанию) -->
      <div v-show="showFilters" class="filters-content">
        <div class="filter-row">
          <div class="filter-group" v-if="genres.length">
            <h4><i class="fas fa-gamepad"></i> Жанры</h4>
            <label v-for="genre in genres" :key="genre.id" class="checkbox-label">
              <input
                type="checkbox"
                :value="genre.id"
                v-model="selectedGenreIds"
                @change="applyFilters"
              />
              {{ genre.name }}
            </label>
          </div>

          <div class="filter-group">
            <h4><i class="fas fa-tag"></i> Цена</h4>
            <div class="price-range">
              <input
                type="number"
                v-model.number="minPrice"
                placeholder="От"
                min="0"
                @change="applyFilters"
              />
              <span>–</span>
              <input
                type="number"
                v-model.number="maxPrice"
                placeholder="До"
                min="0"
                @change="applyFilters"
              />
            </div>
          </div>

          <div class="filter-group">
            <h4><i class="fas fa-birthday-cake"></i> Возраст</h4>
            <div class="age-range">
              <input
                type="number"
                v-model.number="minAge"
                placeholder="От"
                min="0"
                max="100"
                @change="applyFilters"
              />
              <span>–</span>
              <input
                type="number"
                v-model.number="maxAge"
                placeholder="До"
                min="0"
                max="100"
                @change="applyFilters"
              />
            </div>
          </div>

          <div class="filter-group">
            <label class="checkbox-label">
              <input
                type="checkbox"
                v-model="onSaleOnly"
                @change="applyFilters"
              />
              <i class="fas fa-gift"></i> Только со скидкой
            </label>
          </div>
        </div>

        <button @click="resetFilters" class="reset-btn"><i class="fas fa-undo"></i> Сбросить</button>
      </div>

      <!-- Информация -->
      <div class="results-info" v-if="!loading && games.length">
        Найдено: {{ total }} игр
      </div>
      <div v-else-if="!loading && games.length === 0" class="empty-state">
        Ничего не найдено 😕
      </div>

      <!-- Игры -->
      <div class="games-grid">
        <GameCard
          v-for="game in games"
          :key="game.id"
          :game="game"
          @add-to-cart="handleAddToCart"
        />
      </div>

      <!-- Пагинация -->
      <div v-if="totalPages > 1" class="pagination">
        <button :disabled="page === 1" @click="changePage(page - 1)">Назад</button>
        <span>Стр. {{ page }} из {{ totalPages }}</span>
        <button :disabled="page >= totalPages" @click="changePage(page + 1)">Вперёд</button>
      </div>
    </main>

    <Footer />
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue'
import GameCard from '@/components/game-card.vue'
import { useCartStore } from '@/stores/cart'
import { api } from '@/services/api'

const route = useRoute()
const router = useRouter()
const cartStore = useCartStore()

// Аккордеон
const showFilters = ref(false)

// Данные
const games = ref([])
const total = ref(0)
const loading = ref(false)
const error = ref(null)
const totalPages = ref(1)
const page = ref(1)

// Фильтры
const localQuery = ref('')
const selectedGenreIds = ref([])
const minPrice = ref(null)
const maxPrice = ref(null)
const minAge = ref(null)
const maxAge = ref(null)
const onSaleOnly = ref(false)
const localSortBy = ref('id')
const genres = ref([])

// Функции
function toggleFilters() {
  showFilters.value = !showFilters.value
}

async function loadGenres() {
  try {
    const data = await api.games.getGenres()
    genres.value = Array.isArray(data) ? data : []
  } catch (err) {
    console.error('Ошибка загрузки жанров:', err)
  }
}

let searchTimeout = null
function debouncedSearch() {
  clearTimeout(searchTimeout)
  searchTimeout = setTimeout(() => {
    page.value = 1
    applyFilters()
  }, 300)
}

async function fetchGames() {
  loading.value = true
  error.value = null

  try {
    const { q, category, sort, page: urlPage, genres: genreParam, minPrice: minP, maxPrice: maxP, minAge: minA, maxAge: maxA, onSale } = route.query

    localQuery.value = q || ''
    localSortBy.value = sort || 'id'
    page.value = Number(urlPage) || 1
    selectedGenreIds.value = genreParam ? (Array.isArray(genreParam) ? genreParam.map(Number) : [Number(genreParam)]) : []
    minPrice.value = minP ? Number(minP) : null
    maxPrice.value = maxP ? Number(maxP) : null
    minAge.value = minA ? Number(minA) : null
    maxAge.value = maxA ? Number(maxA) : null
    onSaleOnly.value = onSale === 'true'

    const params = {
      search: q || undefined,
      category: category || undefined,
      sortBy: sort || 'id',
      page: page.value,
      pageSize: 24,
      genreIds: selectedGenreIds.value.length ? selectedGenreIds.value : undefined,
      minPrice: minPrice.value ?? undefined,
      maxPrice: maxPrice.value ?? undefined,
      minAge: minAge.value ?? undefined,
      maxAge: maxAge.value ?? undefined,
      onSale: onSaleOnly.value || undefined
    }

    const cleanParams = Object.fromEntries(
      Object.entries(params).filter(([_, v]) => v !== undefined && v !== null)
    )

    const response = await api.games.getAll(cleanParams)
    games.value = Array.isArray(response.items) ? response.items.filter(g => g && g.id) : []
    total.value = response.total || 0
    totalPages.value = response.totalPages || 1

  } catch (err) {
    error.value = err.message || 'Ошибка загрузки'
    games.value = []
  } finally {
    loading.value = false
  }
}

function applyFilters() {
  const query = {}
  if (localQuery.value.trim()) query.q = localQuery.value.trim()
  if (localSortBy.value !== 'id') query.sort = localSortBy.value
  if (selectedGenreIds.value.length) query.genres = selectedGenreIds.value
  if (minPrice.value !== null) query.minPrice = minPrice.value
  if (maxPrice.value !== null) query.maxPrice = maxPrice.value
  if (minAge.value !== null) query.minAge = minAge.value
  if (maxAge.value !== null) query.maxAge = maxAge.value
  if (onSaleOnly.value) query.onSale = 'true'
  if (page.value > 1) query.page = page.value
  router.push({ query })
}

function resetFilters() {
  localQuery.value = ''
  selectedGenreIds.value = []
  minPrice.value = null
  maxPrice.value = null
  minAge.value = null
  maxAge.value = null
  onSaleOnly.value = false
  localSortBy.value = 'id'
  page.value = 1
  router.push({ path: '/catalog' })
}

function changePage(newPage) {
  const query = { ...route.query, page: newPage }
  router.push({ query })
}

function handleAddToCart(game) {
  cartStore.addToCart(game)
}

// Инициализация
onMounted(() => {
  loadGenres()
  fetchGames()
})

watch(() => route.query, () => {
  fetchGames()
})
</script>

<style scoped lang="scss">
.containerCatalog {
  padding: 1.5rem 0;
}

.page-title {
  font-size: 2.2rem;
  margin-bottom: 1.5rem;
  text-align: center;
  color: var(--color-text);
}

.search-filters-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
}

.search-wrapper {
  flex: 1;
  min-width: 300px;
}

.search-box {
  position: relative;
  width: 100%;
}

.search-box input {
  width: 100%;
  padding: 0.7rem 1rem 0.7rem 2.2rem;
  background: var(--color-input-bg);
  border: 1px solid var(--color-border);
  border-radius: 8px;
  color: var(--color-text);
  font-size: 1rem;
  font-family: var(--font-main);
}

.search-box i {
  position: absolute;
  left: 10px;
  top: 50%;
  transform: translateY(-50%);
  color: var(--color-text-secondary);
}

.filters-toggle {
  padding: 0.6rem 1rem;
  background: var(--color-primary);
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.filters-content {
  background: var(--color-card);
  padding: 1.2rem;
  border-radius: 12px;
  margin-bottom: 1.5rem;
}

.filter-row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  margin-bottom: 1rem;
}

.filter-group h4 {
  margin-bottom: 0.6rem;
  font-size: 1rem;
  color: var(--color-text);
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin: 0.4rem 0;
  cursor: pointer;
}

.price-range,
.age-range {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.price-range input,
.age-range input {
  width: 70px;
  padding: 0.35rem;
  background: var(--color-input-bg);
  border: 1px solid var(--color-border);
  border-radius: 6px;
  color: var(--color-text);
  font-size: 0.95rem;
}

.reset-btn {
  width: 100%;
  padding: 0.6rem;
  background: #555;
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-size: 0.95rem;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.results-info,
.empty-state {
  text-align: center;
  margin-bottom: 1.5rem;
  color: var(--color-text-secondary);
}

.games-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(230px, 1fr));
  gap: 1.8rem;
  margin-bottom: 2rem;
  justify-content: center; 
}

.pagination {
  display: flex;
  justify-content: center;
  gap: 1rem;
  margin-top: 1.5rem;
}

.pagination button {
  padding: 0.4rem 0.8rem;
  background: var(--color-primary);
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.pagination button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

@media (max-width: 768px) {
  .search-filters-bar {
    flex-direction: column;
    align-items: stretch;
  }

  .filter-row {
    grid-template-columns: 1fr;
  }

  .games-grid {
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  }
}
</style>