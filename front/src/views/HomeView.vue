<template>
  <div class="app">
    <Header />
    <Hero />

    <div class="container">
      <GenreCategories @genre-selected="handleGenreSelect" />

      <GamesCarousel
        title="Топ продаж"
        :games="topGames"
        @add-to-cart="handleAddToCart"
        @see-more="() => goToCategory('top')"
        @game-click="openGame"
      />

      <GamesCarousel
        title="Скоро выйдет"
        :games="upcomingGames"
        @add-to-cart="handleAddToCart"
        @see-more="() => goToCategory('upcoming')"
      />

      <GamesCarousel
        title="Бесплатные игры"
        :games="freeGames"
        @add-to-cart="handleAddToCart"
        @see-more="() => goToCategory('free')"
      />
    </div>

    <Footer />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/cart'
import { api } from '@/services/api'

import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue'
import Hero from '@/components/hero.vue'
import GenreCategories from '@/components/GenreCategories.vue'
import GamesCarousel from '@/components/GamesCarousel.vue'

const router = useRouter()
const cartStore = useCartStore()
const games = ref([])

onMounted(async () => {
  try {
    const data = await api.games.getAll();
    games.value = Array.isArray(data.items) ? data.items : []; // предполагаем, что данные в поле items

   
  } catch (error) {
    console.error('Ошибка загрузки игр:', error);
    games.value = [];
  }
});

const topGames = computed(() => games.value.filter(g => g.category === 'top'))
const upcomingGames = computed(() => games.value.filter(g => g.category === 'upcoming'))
const freeGames = computed(() => games.value.filter(g => g.category === 'free'))

const handleGenreSelect = (genre) => {
 
  console.log('Выбран жанр:', genre)
}

const goToCategory = (category) => {
  router.push({ name: 'Catalog', query: { category } })
}

const openGame = (game) => {
  router.push({ name: 'Game', params: { id: game.id } })
}

const handleAddToCart = (game) => {
  cartStore.addToCart(game)
}
</script>

<style scoped>
.container {


  padding: 0 1.5rem 2rem;
}

.section-title {
  text-align: left;
  margin: 2.5rem 0 1.25rem;
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--text-primary, #fff);
}
</style> 