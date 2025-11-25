<template>
  <div class="promo-page">
    <Header />
    
    <main class="container">
     

      <div class="game-grid">
        <GameCard
          v-for="(game, index) in games"
          :key="game.id || `game-${index}`"
          :game="game"
          class="game"
          :isAddNew="false"
          @add-to-cart="handleAddToCart"
          @game-click="handleGameClick"
        />

        
       
      </div>

      <div v-if="isLoading && hasMore" class="loading-indicator">
        <div class="spinner"></div>
      </div>

      <div v-else-if="games.length === 0 && !isLoading" class="empty-state">
        Пока нет бесплатных игр. Создайте первую!
      </div>
    </main>

    <Footer />
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue'
import GameCard from '@/components/game-card.vue'
import { useCartStore } from '@/stores/cart'
import { api } from '@/services/api'

const cartStore = useCartStore()

const games = ref([])
const page = ref(1)
const totalPages = ref(0)
const isLoading = ref(false)
const hasMore = ref(true)

const loadGames = async (append = false) => {
  if (isLoading.value || !hasMore.value) return

  isLoading.value = true

  try {
    const response = await api.games.getAll({
      category: 'free',
      page: page.value,
      limit: 24
    })

    const newGames = Array.isArray(response.items) ? response.items : []

    if (append) {
      games.value.push(...newGames)
    } else {
      games.value = newGames
      page.value = 1 
    }

    totalPages.value = response.totalPages || 0
    hasMore.value = page.value < totalPages.value

    if (hasMore.value) {
      page.value += 1
    }
  } catch (error) {
    console.error('Ошибка загрузки бесплатных игр:', error)
    hasMore.value = false
  } finally {
    isLoading.value = false
  }
}

const handleScroll = () => {
  if (isLoading.value || !hasMore.value) return

  const scrollBottom = window.innerHeight + window.scrollY
  const bodyHeight = document.body.offsetHeight

  if (scrollBottom >= bodyHeight - 500) {
    loadGames(true)
  }
}

onMounted(() => {
  loadGames(false)
  window.addEventListener('scroll', handleScroll)
})

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
})

const handleAddToCart = (game) => {
  cartStore.addToCart(game)
}

const handleGameClick = (game) => {
  console.log('Клик по игре:', game)
}

const openCreateGameModal = () => {
  console.log('Открыть создание игры')
}
</script>


<style scoped>
.promo-page {
  background-color: var(--bg-primary, #0f0f13);
  color: var(--text-primary, #fff);
  min-height: 100vh;
}
.game{
    width: 250px;
}
.container {
  max-width: 100%;
  width: 100%;
  margin: 40px 40px;
  padding: 0 20px;
}

.promo-title {
  font-size: 2rem;
  font-weight: 600;
  margin: 2.5rem 0 2rem;
  color: var(--text-primary, #fff);
}

.game-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}
@media (min-width: 1400px) {
  .game-grid {
    grid-template-columns: repeat(6, 1fr);
  }
}

@media (max-width: 768px) {
  .game-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
  }
}

.loading-indicator,
.empty-state {
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