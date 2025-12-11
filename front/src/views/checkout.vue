<template>
  <div class="checkout-page">
    <Header />

    <main class="checkout-main">
      <div class="container">
        <div class="checkout-header">
          <h1>Оформление</h1>
          <router-link to="/cart" class="back-link">
            ← Вернуться в корзину
          </router-link>
        </div>

        <!-- Защита от пустой корзины -->
        <div v-if="cartStore.cartItems.length === 0" class="empty-cart">
          <p>Ваша корзина пуста</p>
          <router-link to="/catalog" class="btn-primary">Перейти в каталог</router-link>
        </div>

        <!-- Основной контент -->
        <div v-else class="checkout-content">
          <!-- Список товаров -->
          <div class="order-items">
            <div
              v-for="item in cartStore.cartItems"
              :key="item.id"
              class="order-item"
            >
              <img :src="item.image" :alt="item.title" class="item-image" />
              <div class="item-info">
                <div class="item-title">{{ item.title }}</div>
                <div class="item-price">₽{{ item.price * item.quantity }}</div>
              </div>
              <div class="item-quantity">×{{ item.quantity }}</div>
            </div>
          </div>

          <!-- Способы оплаты: сохранённые карты + добавление -->
          <div class="payment-methods">
            <div
              v-for="(card, index) in savedCards"
              :key="index"
              class="payment-option"
              :class="{ selected: selectedCardIndex === index }"
              @click="selectCard(index)"
            >
              <div class="payment-icon">
                <i class="fas fa-credit-card"></i>
              </div>
              <div class="payment-text">
                <div class="payment-name">Карта</div>
                <div class="payment-number">{{ card.last4 }}</div>
              </div>
            </div>

            <!-- Кнопка добавления новой карты -->
            <div class="payment-option add-card" @click="openAddCardModal">
              <div class="payment-icon">
                <i class="fas fa-plus"></i>
              </div>
              <div class="payment-text">
                <div class="payment-name">Добавить карту</div>
              </div>
            </div>
          </div>

          <!-- Промокод -->
          <div class="promo-code">
            <input
              type="text"
              v-model="promoCode"
              placeholder="Промокод"
              class="promo-input"
            />
            <button class="apply-btn">Применить</button>
          </div>

          <!-- Итоговая сумма -->
          <div class="total-row">
            <span>Итого:</span>
            <strong>₽{{ cartStore.total }}</strong>
          </div>

         
          <button
            class="pay-button"
            @click="submitOrder"
            :disabled="selectedCardIndex === null"
          >
            Оплатить
          </button>
        </div>
      </div>
    </main>

 
    <div v-if="isModalOpen" class="modal-overlay" @click="closeModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>Добавить новую карту</h2>
          <button class="close-btn" @click="closeModal">&times;</button>
        </div>
        <form @submit.prevent="addNewCard" class="card-form">
          <div class="form-group">
            <label>Номер карты</label>
            <input
              v-model="newCard.number"
              type="text"
              inputmode="numeric"
              placeholder="0000 0000 0000 0000"
              maxlength="19"
              @input="formatCardNumber"
              required
            />
          </div>
          <div class="form-row">
            <div class="form-group">
              <label>Срок действия</label>
              <input
                v-model="newCard.expiry"
                type="text"
                inputmode="numeric"
                placeholder="ММ/ГГ"
                maxlength="5"
                @input="formatExpiry"
                required
              />
            </div>
            <div class="form-group">
              <label>CVV</label>
              <input
                v-model="newCard.cvv"
                type="text"
                inputmode="numeric"
                placeholder="123"
                maxlength="3"
                required
              />
            </div>
          </div>
          <button type="submit" class="add-card-btn">Добавить карту</button>
        </form>
      </div>
    </div>

    <Footer />
  </div>
</template>

<script setup>

import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/cart'
import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue'
import { api } from '@/services/api'

const cartStore = useCartStore()
const router = useRouter()


if (cartStore.cartItems.length === 0) {
  router.push('/cart')
}


const promoCode = ref('')
const isModalOpen = ref(false)
const selectedCardIndex = ref(null)


const savedCards = ref([
  { last4: '**699' },
  { last4: '**123' }
])


const newCard = reactive({
  number: '',
  expiry: '',
  cvv: ''
})


const selectCard = (index) => {
  selectedCardIndex.value = index
}

const openAddCardModal = () => {
  newCard.number = ''
  newCard.expiry = ''
  newCard.cvv = ''
  isModalOpen.value = true
}

const closeModal = () => {
  isModalOpen.value = false
}

const formatCardNumber = (e) => {
  let value = e.target.value.replace(/\D/g, '')
  if (value.length > 16) value = value.slice(0, 16)
  value = value.replace(/(.{4})/g, '$1 ').trim()
  newCard.number = value
}

const formatExpiry = (e) => {
  let value = e.target.value.replace(/\D/g, '')
  if (value.length > 4) value = value.slice(0, 4)
  if (value.length >= 2) value = value.slice(0, 2) + '/' + value.slice(2)
  newCard.expiry = value
}

const addNewCard = () => {
  const cleanNumber = newCard.number.replace(/\D/g, '')
  if (cleanNumber.length !== 16) {
    alert('Неверный номер карты (должно быть 16 цифр)')
    return
  }
  if (!/^\d{2}\/\d{2}$/.test(newCard.expiry)) {
    alert('Неверный срок действия (формат: ММ/ГГ)')
    return
  }
  if (newCard.cvv.length !== 3) {
    alert('Неверный CVV (3 цифры)')
    return
  }

 
  const last4 = cleanNumber.slice(-4)
  savedCards.value.push({ last4: `**${last4}` })
  selectedCardIndex.value = savedCards.value.length - 1

  closeModal()
  alert('Карта успешно добавлена!')
}

const submitOrder = async () => {
  if (selectedCardIndex.value === null) {
    alert('Пожалуйста, выберите способ оплаты')
    return
  }

  try {
    const items = cartStore.cartItems.map(item => ({
      gameId: item.gameId,
      quantity: item.quantity,
      price: item.price 
    }))

    const result = await api.purchases.bulkPurchase(items)

  
    cartStore.clearCart()
    router.push('/activation')

    alert(`Заказ оформлен! Итого: ₽${result.totalAmount}`)
  } catch (error) {
    console.error('Purchase error:', error)
    alert('Ошибка при оформлении заказа: ' + (error.message || 'Попробуйте позже'))
  }
}
</script>

<style scoped>

.checkout-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background: var(--color-bg);
  color: var(--color-text);
}

.checkout-main {
  flex: 1;
  padding: 2rem 0;
}

.container {
  max-width: 600px;
  margin: 0 auto;
  padding: 0 1.5rem;
}

.checkout-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
  gap: 1rem;
}

.checkout-header h1 {
  font-size: 1.8rem;
  font-weight: 700;
  margin: 0;
}

.back-link {
  color: var(--color-primary);
  text-decoration: none;
  font-weight: 600;
  transition: opacity 0.2s;
}

.back-link:hover {
  opacity: 0.8;
}

.empty-cart {
  text-align: center;
  padding: 3rem 2rem;
  background: var(--color-card);
  border-radius: 16px;
}

.empty-cart p {
  font-size: 1.2rem;
  margin-bottom: 1.5rem;
  color: var(--color-text-secondary);
}

/* Товары в заказе */
.order-items {
  background: var(--color-card);
  border-radius: 16px;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
}

.order-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.8rem 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

.order-item:last-child {
  border-bottom: none;
}

.item-image {
  width: 50px;
  height: 50px;
  object-fit: cover;
  border-radius: 8px;
}

.item-info {
  flex: 1;
}

.item-title {
  font-size: 1rem;
  font-weight: 600;
  margin-bottom: 0.2rem;
}

.item-price {
  font-weight: 600;
  color: var(--color-primary);
  font-size: 0.95rem;
}

.item-quantity {
  font-weight: 600;
  color: var(--color-text-secondary);
  font-size: 0.95rem;
}

/* Способы оплаты */
.payment-methods {
  background: var(--color-card);
  border-radius: 16px;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
  gap: 0.8rem;
}

.payment-option {
  padding: 1rem;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
}

.payment-option.selected {
  background: #fff;
  color: #000;
  border-color: var(--color-primary);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.payment-icon {
  font-size: 1.5rem;
  margin-bottom: 0.5rem;
  color: var(--color-primary);
}

.payment-text {
  font-size: 0.85rem;
  line-height: 1.2;
}

.payment-name {
  font-weight: 600;
  margin-bottom: 0.2rem;
}

.payment-number {
  color: var(--color-text-secondary);
  font-size: 0.8rem;
}

.add-card {
  opacity: 0.7;
  border-style: dashed;
}

.add-card:hover {
  opacity: 1;
}

/* Промокод */
.promo-code {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
}

.promo-input {
  flex: 1;
  padding: 0.7rem;
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(255, 255, 255, 0.15);
  border-radius: 8px;
  color: var(--color-text);
  font-size: 0.95rem;
}

.apply-btn {
  padding: 0.7rem 1rem;
  background: var(--color-primary);
  color: #000;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: opacity 0.2s;
}

.apply-btn:hover {
  opacity: 0.9;
}

/* Итого */
.total-row {
  display: flex;
  justify-content: space-between;
  font-size: 1.2rem;
  font-weight: 700;
  color: var(--color-primary);
  padding: 1rem 0;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

/* Кнопка оплаты */
.pay-button {
  width: 100%;
  padding: 1.2rem;
  background: linear-gradient(90deg, var(--color-secondary), var(--color-primary));
  color: #000;
  border: none;
  border-radius: 16px;
  font-weight: 700;
  font-size: 1.2rem;
  cursor: pointer;
  transition: opacity 0.2s;
  margin-top: 1rem;
}

.pay-button:hover:not(:disabled) {
  opacity: 0.9;
}

.pay-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Модальное окно */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background: var(--color-card);
  border-radius: 16px;
  width: 90%;
  max-width: 400px;
  padding: 1.5rem;
  color: var(--color-text);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.modal-header h2 {
  margin: 0;
  font-size: 1.4rem;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.8rem;
  color: var(--color-text-secondary);
  cursor: pointer;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.close-btn:hover {
  color: var(--color-primary);
}

/* Форма добавления карты */
.card-form .form-group {
  margin-bottom: 1rem;
}

.card-form label {
  display: block;
  margin-bottom: 0.4rem;
  font-weight: 600;
  font-size: 0.9rem;
}

.card-form input {
  width: 100%;
  padding: 0.7rem;
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(255, 255, 255, 0.15);
  border-radius: 8px;
  color: var(--color-text);
  font-size: 1rem;
}

.form-row {
  display: flex;
  gap: 0.8rem;
}

.form-row .form-group {
  flex: 1;
}

.add-card-btn {
  width: 100%;
  padding: 0.9rem;
  background: linear-gradient(90deg, var(--color-secondary), var(--color-primary));
  color: #000;
  border: none;
  border-radius: 10px;
  font-weight: 700;
  font-size: 1rem;
  cursor: pointer;
  transition: opacity 0.2s;
  margin-top: 1rem;
}

.add-card-btn:hover {
  opacity: 0.9;
}

/* Адаптивность */
@media (max-width: 480px) {
  .container {
    padding: 0 1rem;
  }

  .checkout-header h1 {
    font-size: 1.5rem;
  }

  .payment-methods {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>