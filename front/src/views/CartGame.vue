<!-- src/views/CartView.vue -->
<template>
  <div class="cart-page">
    <Header />
    <div class="container">
      <div class="cart-header">
        <h1>Ваша корзина</h1>
        <router-link to="/catalog" class="continue-shopping">
           Продолжить покупки
        </router-link>
      </div>

      <div v-if="cartStore.cartItems.length" class="cart-content">
        <div class="cart-items">
          <div v-for="item in cartStore.cartItems" :key="item.id" class="cart-item">
            <div class="item-image">
              <img :src="item.image" :alt="item.title" />
            </div>
            <div class="item-info">
              <h3>{{ item.title }}</h3>
              <p class="item-genre">{{ item.genre?.join(', ') }}</p>
            </div>
            <div class="item-price">
              ₽{{ item.price }}
            </div>
            <div class="item-quantity">
              <button
                class="quantity-btn"
                @click="cartStore.updateQuantity({ id: item.id, quantity: item.quantity - 1 })"
                :disabled="item.quantity <= 1"
              >
                –
              </button>
              <span class="quantity-value">{{ item.quantity }}</span>
              <button
                class="quantity-btn"
                @click="cartStore.updateQuantity({ id: item.id, quantity: item.quantity + 1 })"
              >
                +
              </button>
            </div>
            <div class="item-total">
              ₽{{ (item.price * item.quantity).toFixed(0) }}
            </div>
            <button
              class="remove-btn"
              @click="cartStore.removeFromCart(item.id)"
              title="Удалить"
            >
              <i class="fas fa-times"></i>
            </button>
          </div>
        </div>

        <div class="cart-summary">
          <div class="summary-row">
            <span>Товаров:</span>
            <strong>{{ cartStore.itemCount }} шт.</strong>
          </div>
          <div class="summary-row total">
            <span>Итого:</span>
            <strong>₽{{ cartStore.total }}</strong>
          </div>
          <button class="checkout-btn" @click="handleCheckout">
            Оформить заказ
          </button>
        </div>
      </div>

      <div v-else class="cart-empty">
        <div class="empty-icon">
          <i class="fas fa-shopping-cart"></i>
        </div>
        <p>Ваша корзина пуста</p>
        <router-link to="/catalog" class="btn-primary empty-btn">
          Перейти в каталог
        </router-link>
      </div>
    </div>
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
    <Footer />
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/cart'
import { api } from '@/services/api'

import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue'
import GameCard from '@/components/game-card.vue'






const cartStore = useCartStore()
const router = useRouter()

const handleCheckout = () => {
  if (cartStore.cartItems.length === 0) return
}


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
.cart-page {
  min-height: 100vh;
  background: var(--color-bg);
  color: var(--color-text);
}

.container {
  width: 90%;
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem 0;
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


.cart-content {
  display: grid;
  gap: 2rem;
}

.cart-items {
  background: var(--color-card);
  border-radius: 16px;
  padding: 1.5rem;
}

.cart-item {
  display: grid;
  grid-template-columns: 80px 1fr auto 120px auto 40px;
  gap: 1rem;
  align-items: center;
  padding: 1rem 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
}

.cart-item:last-child {
  border-bottom: none;
}

.item-image img {
  width: 80px;
  height: 80px;
  object-fit: cover;
  border-radius: 8px;
}

.item-info h3 {
  margin: 0 0 0.3rem 0;
  font-size: 1.1rem;
}

.item-genre {
  color: var(--color-text-secondary);
  font-size: 0.85rem;
}

.item-price,
.item-total {
  font-weight: 600;
  color: var(--color-primary);
  text-align: center;
}

.item-quantity {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.quantity-btn {
  width: 32px;
  height: 32px;
  background: rgba(255, 255, 255, 0.08);
  color: var(--color-text);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  cursor: pointer;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.quantity-btn:hover:not(:disabled) {
  background: var(--color-primary);
  color: #000;
}

.quantity-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.quantity-value {
  min-width: 30px;
  text-align: center;
  font-weight: 600;
}

.remove-btn {
  width: 32px;
  height: 32px;
  background: var(--color-warning);
  color: #000;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.remove-btn:hover {
  background: #ffcc5c;
  transform: scale(1.05);
}


.cart-summary {
  background: var(--color-card);
  border-radius: 16px;
  padding: 1.8rem;
  display: flex;
  flex-direction: column;
  gap: 1.2rem;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  font-size: 1.1rem;
}

.summary-row.total {
  font-size: 1.4rem;
  font-weight: 700;
  color: var(--color-primary);
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  padding-top: 1rem;
}

.checkout-btn {
  padding: 1rem;
  background: linear-gradient(90deg, var(--color-secondary), var(--color-primary));
  color: #000;
  border: none;
  border-radius: 12px;
  font-weight: 700;
  font-size: 1.1rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.checkout-btn:hover {
  opacity: 0.9;
  transform: translateY(-2px);
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
  transition: all 0.2s;
}

.empty-btn:hover {
  opacity: 0.9;
}


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
  .item-quantity,
  .item-total,
  .remove-btn {
    display: flex;
    align-items: center;
    font-size: 0.9rem;
  }

  .cart-summary {
    padding: 1.5rem;
  }
}

@media (max-width: 480px) {
  .cart-header {
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }

  .quantity-btn {
    width: 28px;
    height: 28px;
    font-size: 0.9rem;
  }

  .quantity-value {
    min-width: 24px;
    font-size: 0.9rem;
  }
}
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
  margin-top: 20px;
  margin-left: 60px;
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 0.6rem;
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