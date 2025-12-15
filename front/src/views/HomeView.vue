<template>
  <div class="home-page">
    <Header />
    <Hero />

    <div class="container">
      <GenreCategories @genre-selected="handleGenreSelect" />

      <!-- Топ продаж -->
      <GamesCarousel
        v-if="topGames.length"
        title="Топ продаж"
        :games="topGames"
        @add-to-cart="handleAddToCart"
        @see-more="() => goToCategory('top')"
        @game-click="openGameDetail"
      />

      <!-- Скоро выйдет -->
      <GamesCarousel
        v-if="upcomingGames.length"
        title="Скоро выйдет"
        :games="upcomingGames"
        @add-to-cart="handleAddToCart"
        @see-more="() => goToCategory('upcoming')"
        @game-click="openGameDetail"
      />

      <!-- Бесплатные игры -->
      <GamesCarousel
        v-if="freeGames.length"
        title="Бесплатные игры"
        :games="freeGames"
        @add-to-cart="handleAddToCart"
        @see-more="() => goToCategory('free')"
        @game-click="openGameDetail"
      />

      <!-- Загрузка -->
      <div v-if="isLoading" class="loading-section">
        <div class="spinner"></div>
      </div>

      <!-- Если ничего нет -->
      <div v-else-if="!hasAnyGames" class="empty-home">
        <p>Игры скоро появятся. Следите за обновлениями!</p>
      </div>
    </div>

    <Footer />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/cart'
import { api } from '@/services/api'


import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue'
import Hero from '@/components/Hero.vue'
import GenreCategories from '@/components/GenreCategories.vue'
import GamesCarousel from '@/components/GamesCarousel.vue'

const router = useRouter()
const cartStore = useCartStore()

const topGames = ref([])
const upcomingGames = ref([])
const freeGames = ref([])
const isLoading = ref(true)


const isTopGame = (game) => {
  const tag = (game.tag || '').toLowerCase()
  const category = (game.category || '').toLowerCase()
  return tag === 'top' || category === 'top'
}


const isUpcomingGame = (game) => {
  return game.tag === 'Скоро' || game.category === 'Скоро'
}


onMounted(async () => {
  try {
   
    const response = await api.games.getAll({ pageSize: 100 })
    const allGames = Array.isArray(response.items) ? response.items : []

 
    topGames.value = allGames.filter(isTopGame).slice(0, 10)
    upcomingGames.value = allGames.filter(isUpcomingGame).slice(0, 10)
    freeGames.value = allGames.filter(g => g.price === 0).slice(0, 10)
  } catch (error) {
    console.error('Ошибка загрузки главной:', error)
  } finally {
    isLoading.value = false
  }
})

const hasAnyGames = computed(() => {
  return (
    topGames.value.length > 0 ||
    upcomingGames.value.length > 0 ||
    freeGames.value.length > 0
  )
})

const handleGenreSelect = (genre) => {
  console.log('Выбран жанр:', genre)
}

const goToCategory = (category) => {
  router.push({ name: 'Catalog', query: { category } })
}

const openGameDetail = (game) => {
  if (game?.id) {
    router.push({ name: 'GameDetail', params: { id: game.id } })
  }
}

const handleAddToCart = (game) => {
  cartStore.addToCart(game)
}
</script>

<style scoped>
.home-page {
  background-color: var(--bg-primary, #0f0f13);
  color: var(--text-primary, #fff);
  min-height: 100vh;
}

.container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 1.5rem 3rem;
}

.loading-section,
.empty-home {
  text-align: center;
  padding: 3rem 1rem;
  color: var(--text-secondary, #aaa);
}

.spinner {
  width: 32px;
  height: 32px;
  border: 3px solid rgba(255, 255, 255, 0.3);
  border-top: 3px solid var(--color-primary, #e53e3e);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>