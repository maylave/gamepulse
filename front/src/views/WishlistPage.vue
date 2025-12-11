<template>
  <div class="wishlist-page">
    <Header />

    <main class="container">
      <h1 class="page-title">Избранное</h1>

      <div v-if="loading" class="loading-indicator">
        <div class="spinner"></div>
      </div>

      <div v-else-if="wishlistGames.length === 0" class="empty-state">
        <p>Ваше избранное пусто</p>
        <router-link to="/catalog" class="btn-link">Перейти в каталог</router-link>
      </div>

      <div v-else class="game-grid">
        <GameCard
          v-for="game in wishlistGames"
          :key="game.id"
          :game="game"
          @add-to-cart="handleAddToCart"
          @remove-from-wishlist="loadWishlist"
        />
      </div>
    </main>

    <Footer />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/cart'
import { api } from '@/services/api'

import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue'
import GameCard from '@/components/game-card.vue'

const loading = ref(false)
const wishlistGames = ref([])
const router = useRouter()
const cartStore = useCartStore()

const loadWishlist = async () => {
  loading.value = true
  try {
    const response = await api.wishlist.get()
    wishlistGames.value = response
  } catch (err) {
    console.error('Не удалось загрузить избранное:', err)
   
  } finally {
    loading.value = false
  }
}

const handleAddToCart = (game) => {
  cartStore.addToCart(game)
}

onMounted(() => {
  loadWishlist()
})
</script>

<style scoped>
.wishlist-page {
  background-color: var(--bg-primary, #0f0f13);
  color: var(--text-primary, #fff);
  min-height: 100vh;
}


.page-title {
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

@media (max-width: 480px) {
  .game-grid {
    grid-template-columns: 1fr;
    gap: 0.8rem;
  }
}

.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: var(--text-secondary, #aaa);
  font-size: 1.1rem;
}

.btn-link {
  display: inline-block;
  margin-top: 1rem;
  color: var(--color-primary, #e53e3e);
  text-decoration: none;
  font-weight: 600;
}

.btn-link:hover {
  opacity: 0.8;
}

.loading-indicator {
  text-align: center;
  padding: 3rem;
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