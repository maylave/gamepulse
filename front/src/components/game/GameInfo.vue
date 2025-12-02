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
      <button
        ref="btnRef"
        class="btn"
        :class="{ active: isActive }"
        @click="handleAddToCart"
        aria-label="Добавить в корзину"
      >
        <i class="btn__icon btn__icon-cart fas fa-cart-shopping"></i>
        <i class="btn__icon btn__icon-game fas fa-gamepad"></i>
        <span class="btn__text">добавить в корзину</span>
      </button>

      <UButton class="btn-secondary" @click="onWishlist">
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
import { ref, computed } from 'vue'

 import { useCartStore } from '@/stores/cart'
const cartStore = useCartStore()
const props = defineProps({
  game: { type: Object, required: true },
  isInWishlist: { type: Boolean, default: false }
})

const emit = defineEmits(['action', 'wishlist'])


const isActive = ref(false)
const btnRef = ref(null)


const buttonText = computed(() => {
  if (props.game.price === 0) return 'Играть'
  if (props.game.isPreorder) return 'Предзаказ'
  return 'В корзину'
})


const handleAddToCart = () => {
  if (isActive.value) return

  isActive.value = true

  // Добавляем в корзину сразу (без задержки)

  cartStore.addToCart(props.game)
  console.log('Добавлено в корзину:', props.game.title)
  // Сбрасываем анимацию через 1 сек
  setTimeout(() => {
    
    isActive.value = false
  }, 1000)
}

const onWishlist = () => {
  emit('wishlist', props.game)
}


const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('ru-RU', {
    day: '2-digit',
    month: 'long',
    year: 'numeric'
  })
}
</script>

<style scoped lang="scss">
@use '@/assets/style/global/_variables' as *;



.info-section {
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.game-header {
  margin-bottom: 2rem;
}

.game-header h1 {
  font-size: 2.5rem;
  margin-bottom: 1rem;
}

.tags {
  display: flex;
  gap: 0.8rem;
  margin-bottom: 1.5rem;
}

.tag,
.age-rating {
  color: #000;
  padding: 0.3rem 0.8rem;
  border-radius: 20px;
  font-weight: bold;
}

.tag {
  position: relative; 
  background: $color-success;
}

.age-rating {
  background-color: $color-primary;
}

.price-section {
  margin-bottom: 2rem;
}

.price-old {
  text-decoration: line-through;
  color: #777;
}

.price {
  font-size: 2.5rem;
  font-weight: bold;
}

.actions {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 2rem;
}

.btn {
  height: 4em;
  background-color: $color-primary;
  border: none;
  border-radius: 1.5em;
  box-shadow: 0 0 1em rgba(0, 0, 0, 0.2);
  outline: none;
  cursor: pointer;
  position: relative;
  display: flex;
  justify-content: center;
  align-items: center;
  overflow: hidden;
  color: #fff;
  font-weight: 600;
  transition: transform 0.2s cubic-bezier(0.34, 1.56, 0.64, 1),
              background-color 0.2s ease;
}
.btn:active{
    transform: scale(.9);
    -webkit-transform: scale(.9);
    -moz-transform: scale(.9);
    -ms-transform: scale(.9);
    -o-transform: scale(.9);
}
.btn__text{
    font-size: 1rem;
    font-weight: 600;
  color: aliceblue;
}
.btn__icon{
    font-size: 1.5em;
   color: aliceblue;
    position: absolute;
}
.btn__icon-cart{
    left: -20%;
}
.btn__icon-game{
    top: -50%;
}
.btn.active{
 animation: back 1s forwards cubic-bezier(0.25, 0.46, 0.45, 0.94);
 -webkit-animation: back 1s forwards cubic-bezier(0.25, 0.46, 0.45, 0.94);
}

.btn.active .btn__text{
    animation: text 1s forwards cubic-bezier(0.25, 0.46, 0.45, 0.94);
    -webkit-animation: text 1s forwards cubic-bezier(0.25, 0.46, 0.45, 0.94);
}


.btn.active .btn__icon-cart{
     animation: cart .85s forwards cubic-bezier(0.25, 0.46, 0.45, 0.94);
     -webkit-animation: cart .85s forwards cubic-bezier(0.25, 0.46, 0.45, 0.94);
}
.btn.active .btn__icon-game{
     animation: game .7s forwards cubic-bezier(0.25, 0.46, 0.45, 0.94);
     -webkit-animation: game .7s forwards cubic-bezier(0.25, 0.46, 0.45, 0.94);
}





@keyframes text{
    100%{
        opacity: 0;
    }
}

@keyframes cart{
    10%{
         left: 5%;
         transform: rotate(-20deg) scale(1.1);
         -webkit-transform: rotate(-20deg) scale(1.1);
         -moz-transform: rotate(-20deg) scale(1.1);
         -ms-transform: rotate(-20deg) scale(1.1);
         -o-transform: rotate(-20deg) scale(1.1);
}


    50%, 60%{
        left:42% ;
        transform:  scale(1.2);
        -webkit-transform:  scale(1.2);
        -moz-transform:  scale(1.2);
        -ms-transform:  scale(1.2);
        -o-transform:  scale(1.2);
}

    80%,90%{
         left: 80%;
         transform: rotate(15deg);
         -webkit-transform: rotate(15deg);
         -moz-transform: rotate(15deg);
         -ms-transform: rotate(15deg);
         -o-transform: rotate(15deg);
}


    100%{
        left: 110%;
        transform: scale(.6);
        -webkit-transform: scale(.6);
        -moz-transform: scale(.6);
        -ms-transform: scale(.6);
        -o-transform: scale(.6);
}
}
@keyframes game{
   

    70%{
        top: 10%;
        opacity: .8;
        transform: scale(.75);
        -webkit-transform: scale(.75);
        -moz-transform: scale(.75);
        -ms-transform: scale(.75);
        -o-transform: scale(.75);
}

    90%, 100%{
        top: 15%;
        opacity: 0;
    }
}


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

  i {
    color: $color-primary;
    width: 24px;
    text-align: center;
  }
}
</style>