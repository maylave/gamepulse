<template>
  <div class="wishlist-page">
    <Header />

    <main class="container">
      <h1 class="page-title">Избранное</h1>

      <div v-if="isLoading" class="loading-indicator">
        <div class="spinner"></div>
      </div>

      <div v-else-if="wishlist.length === 0" class="empty-state">
        <p>Список избранного пуст.</p>
        <p>Добавляйте игры с помощью ❤️ на карточках.</p>
      </div>

      <div v-else class="game-grid">
        <GameCard
          v-for="item in wishlist"
          :key="item.id"
          :game="item.game"
          @add-to-cart="handleAddToCart"
          @game-click="handleGameClick"
          @remove-from-wishlist="() => removeFromWishlist(item.id)"
        />
      </div>
    </main>

    <Footer />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'
import GameCard from '@/components/Game-card.vue'
import { api } from '@/services/api'
import { useCartStore } from '@/stores/cart'

const cartStore = useCartStore()

const wishlist = ref([])
const isLoading = ref(false)

const loadWishlist = async () => {
  isLoading.value = true
  try {
    const response = await api.wishlist.get()
    // Ожидаем формат: { items: [ { id, game } ] } или массив напрямую
    wishlist.value = Array.isArray(response)
      ? response
      : (response?.items || [])
  } catch (error) {
    console.error('Ошибка загрузки избранного:', error)
    wishlist.value = []
  } finally {
    isLoading.value = false
  }
}

const removeFromWishlist = async (wishlistId) => {
  try {
    await api.wishlist.remove(wishlistId)
    // Удаляем из UI без перезагрузки
    wishlist.value = wishlist.value.filter(item => item.id !== wishlistId)
  } catch (error) {
    console.error('Ошибка удаления из избранного:', error)
  }
}

const handleAddToCart = (game) => {
  cartStore.addToCart(game)
}

const handleGameClick = (game) => {
  // router.push(`/game/${game.id}`)
  console.log('Просмотр игры:', game.id)
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

.container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 20px;
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

.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: var(--text-secondary, #aaa);
  font-size: 1.1rem;
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