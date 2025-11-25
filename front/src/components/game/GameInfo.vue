<!-- src/components/game/GameInfo.vue -->
<template>
  <div class="info-section">
    <div class="game-header">
      <h1>{{ game.title }}</h1>
      <div class="tags">
        <span v-if="game.tag" class="tag">{{ game.tag }}</span>
        <span class="age-rating">{{ game.ageRating }}+</span>
      </div>
    </div>

    <div class="price-section">
      <div v-if="game.oldPrice" class="price-old">{{ game.oldPrice }} ₽</div>
      <div class="price">{{ game.price === 0 ? 'Бесплатно' : `${game.price} ₽` }}</div>
    </div>

    <div class="actions">
        <UButton  @click="onAction">
             {{ buttonText }}
        </UButton>
     <UButton class=" btn-secondary" @click="onWishlist">
          <i :class="isInWishlist ? 'fas fa-heart' : 'far fa-heart'"></i>
        В избранное
     </UButton>
  
    </div>

    <div class="game-details">
      <div class="detail-item">
        <i class="fas fa-calendar"></i>
        <span>Дата выхода: {{ formatDate(game.releaseDate) }}</span>
      </div>
      <div class="detail-item">
        <i class="fas fa-gamepad"></i>
        <span>Жанр: {{ game.genre }}</span>
      </div>
      <div class="detail-item">
        <i class="fas fa-users"></i>
        <span>Разработчик: {{ game.developer }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  game: { type: Object, required: true },
  isInWishlist: { type: Boolean, default: false }
})

const emit = defineEmits(['action', 'wishlist'])

const buttonText = computed(() => {
  if (props.game.price === 0) return 'Играть'
  if (props.game.isPreorder) return 'Предзаказ'
  return 'В корзину'
})

const onAction = () => emit('action')
const onWishlist = () => emit('wishlist')

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('ru-RU', {
    day: '2-digit',
    month: 'long',
    year: 'numeric'
  })
}
const addToCart = (e) => {
  e.stopPropagation() 
  cartStore.addToCart(props.game)
  console.log('Добавлено в корзину:', props.game.title)
}
</script>

<style scoped lang="scss">
@use '@/assets/style/global/_variables' as *;
.tag {
    position: absolute;
    top: 12px;
    left: 12px;
    background: $color-success;
    color: #000;
    font-size: 0.75rem;
    font-weight: bold;
    padding: 0.25rem 0.6rem;
    border-radius: 4px;
    z-index: 2;
}
.age-rating{background-color: $color-primary;}
.info-section { display: flex; flex-direction: column; justify-content: center; }
.game-header { margin-bottom: 2rem; }
.game-header h1 { font-size: 2.5rem; margin-bottom: 1rem; }
.tags { display: flex; gap: 0.8rem; margin-bottom: 1.5rem; }
.tag, .age-rating {

  color: #000;
  padding: 0.3rem 0.8rem;
  border-radius: 20px;
  font-weight: bold;
}
.price-section { margin-bottom: 2rem; }
.price-old { text-decoration: line-through; color: #777; }
.price { font-size: 2.5rem; font-weight: bold; }
.actions { display: flex; flex-direction: column; gap: 1rem; margin-bottom: 2rem; }

.btn-secondary {
  background: rgba(255, 255, 255, 0.05);
  color: #fff;
  border: 1px solid #444;
}
.game-details {
  background: rgba(255, 255, 255, 0.03);
  border-radius: 16px;
  padding: 1.5rem;
}
.detail-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1rem;
  color: #aaa;
  i { color: $color-primary; width: 24px; text-align: center; }
}
</style>