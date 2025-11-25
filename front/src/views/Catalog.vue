<template>
  <div class="app">
    <Header />

    <main class="container">
      <h1 class="page-title">Каталог игр</h1>

      <!-- Панель управления: поиск + сортировка -->
      <div class="controls">
        <div class="search-box">
          <i class="fas fa-search"></i>
          <input 
            v-model="query" 
            type="text" 
            placeholder="Поиск по названию..." 
          />
        </div>

        <select v-model="sortBy" class="sort-select">
          <option value="popularity">Сначала популярные</option>
          <option value="price-asc">Сначала дешёвые</option>
          <option value="price-desc">Сначала дорогие</option>
          <option value="newest">Новинки</option>
        </select>
      </div>

      
      <div class="games-grid">
        <GameCard
          v-for="game in displayedGames"
          :key="game.id"
          :game="game"
          :is-promo="game.isPromo"
          @add-to-cart="handleAddToCart"
        />
      </div>
    </main>

    <Footer />
  </div>
</template>
<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'
import GameCard from '@/components/Game-card.vue'
import { useCartStore } from '@/stores/cart'
import { api } from '@/services/api'

const route = useRoute()

const games = ref([]) 
const query = ref('')
const sortBy = ref('popularity')
onMounted(async () => {
  try {
    const data = await api.games.getAll();
    games.value = Array.isArray(data.items) ? data.items : []; // предполагаем, что данные в поле items

   
  } catch (error) {
    console.error('Ошибка загрузки игр:', error);
    games.value = [];
  }
});


watch(
  () => route.query.q,
  (newQuery) => {
    query.value = newQuery || ''
  }
)

watch(query, (newQuery) => {
  const newRoute = { name: 'Catalog' }
  if (newQuery.trim()) {
    newRoute.query = { q: newQuery }
  }
})

const displayedGames = computed(() => {
  let results = games.value.filter(game => 
    game.title.toLowerCase().includes(query.value.toLowerCase())
  )

  results = [...results].sort((a, b) => {
    if (sortBy.value === 'price-asc') return a.price - b.price
    if (sortBy.value === 'price-desc') return b.price - a.price
    if (sortBy.value === 'newest') return new Date(b.releaseDate) - new Date(a.releaseDate)
    return 0
  })

  return results
})

</script>

<style scoped lang="scss">

.container {
  width: 90%;

  margin: 0 60px;
  padding: 2rem 0;
}
.page-title {
  font-size: 2.5rem;
  margin-bottom: 2rem;
  text-align: center;
}
.controls {
  display: flex;
  gap: 1.5rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}
.search-box {
  position: relative;
  flex: 1;
  min-width: 300px;
}
.search-box input {
  width: 100%;
  padding: 0.8rem 1rem 0.8rem 2.5rem;
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid #333;
  border-radius: 12px;
  color: white;
  font-size: 1rem;
  font-family: var(--font-main);
}
.search-box i {
  position: absolute;
  left: 12px;
  top: 50%;
  transform: translateY(-50%);
  color: #777;
}
.sort-select {
  padding: 0.8rem 1.2rem;
  background: rgba(0, 0, 0, 0.3);
  color: white;
  border: 1px solid #333;
  border-radius: 12px;
  font-family: var(--font-main);
  min-width: 200px;
  appearance: none; 
 
  background-repeat: no-repeat;
  background-position: right 12px center;
  background-size: 12px;
  cursor: pointer;
  outline: none;

  
  &:focus {
    border-color: #eee;
    background-color: #333;
    
  }

  
  -webkit-appearance: none;
}
.games-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 2.2rem;
  margin-bottom: 3rem;
}
@media (max-width: 768px) {
  .controls {
    flex-direction: column;
    align-items: center;
  }
  .search-box {
    min-width: auto;
  }
  .sort-select {
    min-width: auto;
  }
  .games-grid {
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    justify-content: center; 
  }
}

</style>