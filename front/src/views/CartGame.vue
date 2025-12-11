<template>
  <div class="cart-page">
    <Header />
    <main class="container">
      <div class="containerGame">
        <div class="cart-header">
          <h1>Ваша корзина</h1>
          <router-link to="/catalog" class="continue-shopping">
            Продолжить покупки
          </router-link>
        </div>

        <div v-if="cartStore.cartItems.length" class="cart-content">
          <div class="cart-items">
            <div v-for="item in cartStore.cartItems" :key="item.id" class="cart-item">
              <div class="item-left">
                <div class="item-image">
                  <img :src="item.image" :alt="item.title" />
                </div>
                <div class="item-info">
                  <h3>{{ item.title }}</h3>
                  <p class="item-genre">{{ item.genre?.join(', ') }}</p>
                </div>
              </div>

              <div class="item-right">
                <div class="item-price">{{ item.price }}₽</div>
                <div class="item-quantity">
                  <DragCounter
                    :model-value="item.quantity"
                    :min="1"
                    @update:model-value="
                      (qty) => cartStore.updateQuantity({ id: item.id, quantity: qty })
                    "
                  />
                </div>
                <div class="item-total">{{ item.quantity * item.price }}₽</div>
                <button
                  class="remove-btn"
                  @click="cartStore.removeFromCart(item.id)"
                  title="Удалить"
                >
                  <i class="fas fa-times"></i>
                </button>
              </div>
            </div>
          </div>

          <div class="cart-summary">
            <div class="summary-row">
              <span>Товаров:</span>
              <strong>{{ cartStore.itemCount }} шт.</strong>
            </div>
            <div class="summary-row total">
              <span>Итого:</span>
              <strong>{{ cartStore.total }}₽</strong>
            </div>
            <button class="checkout-btn" @click="handleCheckout">Оформить заказ</button>
          </div>
        </div>

        <div v-else class="cart-empty">
          <div class="empty-icon"><i class="fas fa-shopping-cart"></i></div>
          <p>Ваша корзина пуста</p>
          <router-link to="/catalog" class="btn-primary empty-btn">Перейти в каталог</router-link>
        </div>
      </div>

      <!-- История просмотров -->
      <div v-if="viewHistory.length" class="history-section">
        <h2>Вы недавно смотрели</h2>
        <div class="game-grid fixed-cards">
          <GameCard
            v-for="game in viewHistory"
            :key="`history-${game.id}`"
            :game="game"
            @add-to-cart="handleAddToCart"
          />
        </div>
      </div>

      <!-- Рекомендуемые игры -->
      <div class="recommended-section">
        <h2>Вам может понравиться</h2>
        <div class="game-grid fixed-cards">
          <GameCard
            v-for="(game, index) in games"
            :key="game.id || `game-${index}`"
            :game="game"
            @add-to-cart="handleAddToCart"
          />
        </div>
      </div>
    </main>
    <Footer />
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/cart'
import { useViewHistory } from '@/stores/useViewHistory'
import { api } from '@/services/api'

import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue'
import GameCard from '@/components/game-card.vue'
import DragCounter from '@/components/DragCounter.vue'

const cartStore = useCartStore()
const { history: viewHistory } = useViewHistory()
const router = useRouter()

const games = ref([])
const page = ref(1)
const hasMore = ref(true)
const isLoading = ref(false)

onMounted(() => {
  cartStore.fetchCart()
  loadGames(false)
  window.addEventListener('scroll', handleScroll)
})

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
})

const handleCheckout = () => {
  if (cartStore.cartItems.length) {
    router.push('/checkout')
  }
}

const loadGames = async (append = false) => {
  if (isLoading.value || !hasMore.value) return

  isLoading.value = true

  try {
    const response = await api.games.getAll({
      page: page.value,
      pageSize: 12
    })

    const newGames = response.items || []

    if (append) {
      games.value.push(...newGames)
    } else {
      games.value = newGames
      page.value = 1
    }

    hasMore.value = page.value < (response.totalPages || 1)
    if (hasMore.value) page.value++

  } finally {
    isLoading.value = false
  }
}

const handleScroll = () => {
  if (isLoading.value || !hasMore.value) return

  const scrollBottom = window.innerHeight + window.scrollY
  const bodyHeight = document.body.offsetHeight

  if (scrollBottom >= bodyHeight - 300) {
    loadGames(true)
  }
}

const handleAddToCart = (game) => {
  cartStore.addToCart(game)
 
 
 
    cartStore.fetchCart()
  
  
}
</script>

<style scoped lang="scss">
.cart-page {
  min-height: 100vh;
  background: var(--color-bg);
  color: var(--color-text);
}

.container {
  max-width: 1200px;
  width: 100%;
  margin: 0 auto;
  padding: 0 20px;
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
  margin: 0;
}

.continue-shopping {
  color: var(--color-primary);
  text-decoration: none;
  font-weight: 600;
  transition: opacity 0.2s;
  white-space: nowrap;
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

// === Основной элемент корзины — ПЕРЕДЕЛАНО НА FLEX ===
.cart-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  gap: 1rem;

  .item-left {
    display: flex;
    align-items: center;
    gap: 1rem;
    flex: 1;
    min-width: 0; // предотвращает переполнение
  }

  .item-image img {
    width: 80px;
    height: 80px;
    object-fit: cover;
    border-radius: 8px;
    flex-shrink: 0;
  }

  .item-info {
    display: flex;
    flex-direction: column;
    gap: 0.3rem;
    flex: 1;
    min-width: 0;
  }

  .item-info h3 {
    margin: 0;
    font-size: 1.1rem;
    font-weight: 600;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .item-genre {
    color: var(--color-text-secondary);
    font-size: 0.85rem;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .item-right {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    white-space: nowrap;
    flex-shrink: 0;
  }

  .item-price,
  .item-total {
    font-weight: 600;
    color: var(--color-primary);
    font-size: 0.95rem;
  }

  .item-quantity {
    min-width: 80px;
    max-width: 80px;
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
    flex-shrink: 0;
  }

  .remove-btn:hover {
    background: #ffcc5c;
    transform: scale(1.05);
  }
}

.cart-item:last-child {
  border-bottom: none;
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
  width: 100%;
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

/* === Адаптивность === */

@media (max-width: 768px) {
  .containerGame {
    padding: 1.5rem 1.5rem;
  }

  .cart-header {
    flex-direction: column;
    align-items: stretch;
    gap: 1rem;
    text-align: center;
  }

  .cart-item {
    flex-direction: column;
    align-items: stretch;
    gap: 1rem;
    padding: 1rem 0;
  }

  .item-left {
    flex-direction: row;
    align-items: flex-start;
    gap: 1rem;
  }

  .item-right {
    justify-content: flex-end;
    gap: 0.75rem;
    width: 100%;
    flex-wrap: wrap;
  }

  .item-quantity {
    min-width: 70px;
    max-width: 70px;
  }

  .item-price,
  .item-total {
    font-size: 0.9rem;
  }

  .remove-btn {
    width: 30px;
    height: 30px;
  }

  .checkout-btn {
    font-size: 1rem;
    padding: 0.9rem;
  }
}

@media (max-width: 480px) {
  .containerGame {
    padding: 1rem;
  }

  .cart-header h1 {
    font-size: 1.8rem;
  }

  .item-image img {
    width: 64px;
    height: 64px;
  }

  .item-info h3 {
    font-size: 1rem;
  }

  .item-genre {
    font-size: 0.8rem;
  }

  .item-right {
    gap: 0.5rem;
    justify-content: flex-end;
  }

  .item-quantity {
    min-width: 60px;
    max-width: 60px;
  }

  .item-price,
  .item-total {
    font-size: 0.85rem;
  }

  .remove-btn {
    width: 28px;
    height: 28px;
  }

  .checkout-btn {
    font-size: 1rem;
    padding: 0.85rem;
  }
}

/* === Карточки (история / рекомендации) === */

.game-grid.fixed-cards {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  margin-top: 1.5rem;
  margin-bottom: 2rem;
  padding: 0 1rem;
  justify-content: center;
}

.game-grid.fixed-cards .game-card {
  width: 250px;
  min-width: 250px;
  max-width: 250px;
}

.history-section,
.recommended-section {
  padding: 2rem 1rem 1rem;
}

.history-section h2,
.recommended-section h2 {
  text-align: center;
  margin-bottom: 1.5rem;
  font-size: 1.5rem;
  color: var(--color-text);
}

@media (max-width: 768px) {
  .game-grid.fixed-cards .game-card {
    width: calc(50% - 0.5rem);
    min-width: auto;
    max-width: none;
  }
}

@media (max-width: 480px) {
  .game-grid.fixed-cards {
    justify-content: center;
    flex-direction: column;
    align-items: center;
  }

  .game-grid.fixed-cards .game-card {
    width: 90vw;
    min-width: auto;
    max-width: 90vw;
  }

  .history-section,
  .recommended-section {
    padding: 2rem 0.5rem 1.5rem;
  }

  .history-section h2,
  .recommended-section h2 {
    font-size: 1.25rem;
    margin-bottom: 1.2rem;
  }
}
</style>