<template>
  <div :class="['game-card', { 'promo': isPromoComputed, 'placeholder': isPlaceholder, 'add-new': isAddNew }]">
    <!-- Тег (скидка, новинка, скоро и т.д.) -->
    <span v-if="game.tag && !isPlaceholder && !isAddNew" class="tag" :style="tagStyle">{{ game.tag }}</span>

    <!-- Изображение игры -->
    <img
      v-if="!isPlaceholder && !isAddNew"
      :src="game.imageUrl"
      :alt="game.title"
      class="game-img"
      @error="handleImageError"
    />

    <!-- Плейсхолдер для "Создать игру" -->
    <div v-else-if="isAddNew" class="placeholder">
      <span class="add-icon">+</span>
    </div>

    <!-- Кнопка "В избранное" -->
    <button
      v-if="!isPlaceholder && !isAddNew"
      class="wishlist-btn"
      @click.stop="toggleWishlist"
      :disabled="wishlistLoading"
      :class="{ 'pulse-added': isAdded }"
    >
      <i :class="isInWishlist ? 'fas fa-heart' : 'far fa-heart'"></i>
    </button>

    <!-- Основная информация и кнопка -->
    <div v-if="!isPlaceholder && !isAddNew" class="game-info">
      <router-link
        :to="{ name: 'GameDetail', params: { id: game.id } }"
        class="game-card-link"
        @click.stop
      >
        <div>
          <h3>{{ game.title }}</h3>
          <div v-if="game.oldPrice" class="price-old">{{ game.oldPrice }} ₽</div>
          <div class="price">
            {{ game.price === 0 ? 'Бесплатно' : `${game.price} ₽` }}
          </div>
        </div>
      </router-link>
      <button class="add-to-cart" @click.stop="addToCart">
        {{ buttonText }}
      </button>
    </div>

    <!-- Плейсхолдер "Показать ещё" -->
    <div v-else-if="isPlaceholder" class="placeholder-button">
      <button @click.stop="$emit('loadMore')">Показать ещё</button>
    </div>

    <!-- Плейсхолдер "Создать игру" -->
    <div v-else-if="isAddNew" class="placeholder-button">
      <button @click.stop="$emit('createNewGame')">Создать новую игру</button>
    </div>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue'
import { useWishlistStore } from '@/stores/wishlistStore'
import { useCartStore } from '@/stores/cart'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useNotificationStore } from '@/stores/NotificationStore'

// --- Props ---
const props = defineProps({
  game: {
    type: Object,
    required: true,
    validator(value) {
      return value.title || value.isPlaceholder || value.isAddNew
    }
  },
  isPromo: { type: Boolean, default: false },
  isPlaceholder: { type: Boolean, default: false },
  isAddNew: { type: Boolean, default: false }
})

// --- Emits ---
defineEmits(['loadMore', 'createNewGame'])

// --- Stores ---
const authStore = useAuthStore()
const cartStore = useCartStore()
const wishlistStore = useWishlistStore()
const notificationStore = useNotificationStore()
const router = useRouter()

// --- Reactive state ---
const isAdded = ref(false)
const wishlistLoading = ref(false)

// --- Computed: isPromo (авто + ручной override) ---
const isPromoComputed = computed(() => {
  if (props.isPromo) return true // ручное управление приоритетнее

  const { game } = props
  if (!game) return false

  const tag = game.tag?.trim()

  // 1. "Скоро" или дата (например, "25 окт")
  if (tag === 'Скоро' || (tag && /\d+\s+(янв|фев|мар|апр|май|июн|июл|авг|сен|окт|ноя|дек)/i.test(tag))) {
    return true
  }

  // 2. "Новинка"
  if (tag === 'Новинка') {
    return true
  }

  // 3. Скидка > 60%
  if (game.oldPrice != null && game.price != null && game.oldPrice > 0) {
    const discount = ((game.oldPrice - game.price) / game.oldPrice) * 100
    if (discount > 60) {
      return true
    }
  }

  return false
})

// --- Computed: isInWishlist ---
const isInWishlist = computed(() => {
  if (props.isPlaceholder || props.isAddNew || !authStore.isAuthenticated) return false
  return wishlistStore.isGameInWishlist(props.game.id)
})

// --- Methods ---
const toggleWishlist = async (e) => {
  if (!authStore.isAuthenticated) {
    notificationStore.showWarning('Войдите в аккаунт, чтобы добавить в избранное', 3000)
    return
  }
  e.stopPropagation()
  wishlistLoading.value = true
  try {
    await wishlistStore.toggleWishlist(props.game)
    isAdded.value = true
    setTimeout(() => isAdded.value = false, 1000)
  } catch (err) {
    console.error('Не удалось обновить избранное:', err)
  } finally {
    wishlistLoading.value = false
  }
}

const addToCart = (e) => {
  if (!authStore.isAuthenticated) {
    notificationStore.showWarning('Войдите в аккаунт, чтобы добавить в корзину', 3000)
    return
  }
  e.stopPropagation()
  cartStore.addToCart(props.game)
}

// --- Computed: buttonText ---
const buttonText = computed(() => {
  if (props.isPlaceholder || props.isAddNew) return ''
  if (props.game.price === 0) return 'Играть'
  if (props.game.tag?.includes('окт') || props.game.tag === 'Скоро') return 'Предзаказ'
  return 'В корзину'
})

// --- Computed: tagStyle ---
const tagStyle = computed(() => {
  if (props.isPlaceholder || props.isAddNew) return {}

  const tag = props.game.tag?.trim()
  if (!tag) return {}

  if (tag === 'Бесплатно') {
    return { background: '#00D98D', color: '#000' }
  }
  if (tag === 'Новинка') {
    return { background: '#FF6B35', color: '#000' }
  }
  if (tag === 'Скоро' || /\d+\s+(янв|фев|мар|апр|май|июн|июл|авг|сен|окт|ноя|дек)/i.test(tag)) {
    return {
      background: 'linear-gradient(90deg, #6A00FF, #FF00FF)',
      color: '#fff',
      fontWeight: '600'
    }
  }
  return {}
})


</script>

<style lang="scss" scoped src="@/assets/style/components/cart/main.scss"></style>