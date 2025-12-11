<template>
  <div class="purchased-games-page">
    <Header />
    <div class="containerGame">
      <div class="cart-header">
        <h1>Купленные игры</h1>
        <router-link to="/catalog" class="continue-shopping"> Перейти в каталог </router-link>
      </div>

      <div v-if="loading" class="loading">Загрузка...</div>
      <div v-else-if="error" class="error">{{ error }}</div>
      <div v-else-if="purchasedGames.length === 0" class="cart-empty">
        <div class="empty-icon"><i class="fas fa-gamepad"></i></div>
        <p>Вы ещё ничего не покупали</p>
        <router-link to="/catalog" class="btn-primary empty-btn">Перейти в каталог</router-link>
      </div>
      <div v-else class="cart-items">
        <div v-for="game in purchasedGames" :key="game.id" class="cart-item">
        
          <div class="item-image">
            <img
              v-if="game.imageUrl"
              :src="game.imageUrl"
              :alt="game.title"
              @error="onImageError(game)"
            />
            <div v-else class="no-image"></div>
          </div>

      
          <div class="item-info">
            <h3>{{ game.title }}</h3>
            <p class="item-genre" v-if="game.genres?.length">
              {{ game.genres.join(', ') }}
            </p>
            <p class="item-developer" v-if="game.developer">
              {{ game.developer }}
            </p>
          </div>

        
          <div class="item-price">{{ formatPrice(game.price) }} ₽</div>

         
          <div class="item-actions">
            <button
              v-if="game.externalUrl"
              @click="openGame(game.external)"
              class="play-btn"
            >
              Играть
            </button>
            <span v-else class="no-link">Нет ссылки</span>
          </div>
        </div>
      </div>
    </div>
    <Footer />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/services/api'
import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue'

const router = useRouter()
const purchasedGames = ref([])
const loading = ref(false)
const error = ref(null)

function formatPrice(price) {
  return new Intl.NumberFormat('ru-RU').format(Math.round(price))
}

// Обработка ошибки загрузки изображения — скрываем его
function onImageError(game) {
  game.imageUrl = null // или можно использовать отдельное поле в локальном состоянии
}

function openGame(url) {
  window.open(url, '_blank', 'noopener,noreferrer')
}

async function fetchPurchasedGames() {
  loading.value = true
  error.value = null
  try {
    const response = await api.games.getPurchased()
    purchasedGames.value = Array.isArray(response) ? response : []
    // Очистим imageUrl у игр, где он пустой или недоступный
    purchasedGames.value.forEach(game => {
      if (!game.imageUrl || game.imageUrl.trim() === '') {
        game.imageUrl = null
      }
    })
  } catch (err) {
    if (err.response?.status === 401) {
      error.value = 'Требуется авторизация'
    } else {
      error.value = err.message || 'Не удалось загрузить купленные игры'
    }
    console.error('[PurchasedGames] Error:', err)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchPurchasedGames()
})
</script>

<style scoped>
.purchased-games-page {
  min-height: 100vh;
  background: var(--color-bg);
  color: var(--color-text);
}

.containerGame {
  padding: 2rem 2.5rem;
}

.cart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2.5rem;
}

.cart-header h1 {
  font-size: 2.2rem;
  font-weight: 700;
}

.continue-shopping {
  color: var(--color-primary);
  text-decoration: none;
  font-weight: 600;
  transition: opacity 0.2s;
}

.continue-shopping:hover {
  opacity: 0.8;
}

.cart-items {
  background: var(--color-card);
  border-radius: 16px;
  padding: 1.5rem;
}

.cart-item {
  display: grid;
  grid-template-columns: 80px 1fr auto 120px;
  gap: 1rem;
  align-items: center;
  padding: 1rem 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
}

.cart-item:last-child {
  border-bottom: none;
}

.item-image {
  width: 80px;
  height: 80px;
  border-radius: 8px;
  overflow: hidden;
  background: rgba(0, 0, 0, 0.1);
  display: flex;
  align-items: center;
  justify-content: center;
}

.item-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.no-image {
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.05);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--color-text-secondary);
  font-size: 0.75rem;
  text-align: center;
}

.item-info h3 {
  margin: 0 0 0.3rem 0;
  font-size: 1.1rem;
}

.item-genre,
.item-developer {
  color: var(--color-text-secondary);
  font-size: 0.85rem;
  margin: 0;
}

.item-price {
  font-weight: 600;
  color: var(--color-primary);
  text-align: center;
}

.item-actions {
  display: flex;
  justify-content: center;
}

.play-btn {
  padding: 0.5rem 1rem;
  background: linear-gradient(90deg, var(--color-secondary), var(--color-primary));
  color: #000;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: opacity 0.2s;
}

.play-btn:hover {
  opacity: 0.9;
}

.no-link {
  color: var(--color-text-secondary);
  font-size: 0.9rem;
  font-style: italic;
}

.cart-empty {
  text-align: center;
  padding: 4rem 2rem;
  background: var(--color-card);
  border-radius: 16px;
}

.empty-icon {
  font-size: 4rem;
  color: var(--color-primary);
  margin-bottom: 1.5rem;
}

.cart-empty p {
  font-size: 1.3rem;
  margin-bottom: 2rem;
  color: var(--color-text-secondary);
}

.empty-btn {
  padding: 0.8rem 2rem;
  background: linear-gradient(90deg, var(--color-secondary), var(--color-primary));
  color: #000;
  border: none;
  border-radius: 30px;
  font-weight: 700;
  text-decoration: none;
  display: inline-block;
  transition: opacity 0.2s;
}

.empty-btn:hover {
  opacity: 0.9;
}

.loading,
.error {
  text-align: center;
  padding: 2rem;
  color: var(--color-text-secondary);
  background: var(--color-card);
  border-radius: 16px;
}

/* Адаптив */
@media (max-width: 768px) {
  .cart-item {
    grid-template-columns: 70px 1fr;
    grid-template-rows: auto auto;
    gap: 0.8rem;
    padding: 1rem 0;
  }

  .cart-item > *:not(.item-image):not(.item-info) {
    justify-self: end;
  }

  .item-info,
  .item-image {
    grid-column: 1 / -1;
    display: flex;
    align-items: center;
    gap: 1rem;
  }

  .item-price,
  .item-actions {
    font-size: 0.9rem;
  }

  .play-btn {
    padding: 0.4rem 0.8rem;
    font-size: 0.85rem;
  }
}

@media (max-width: 480px) {
  .cart-header {
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }

  .containerGame {
    padding: 1.5rem;
  }
}
</style>