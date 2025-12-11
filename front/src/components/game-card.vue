<template>
  <!-- Обёртка карточки с router-link -->
  <div :class="['game-card', { 'promo': isPromo, 'placeholder': isPlaceholder, 'add-new': isAddNew }]">

   
    <span v-if="game.tag && !isPlaceholder && !isAddNew" class="tag" :style="tagStyle">{{ game.tag }}</span>

  
    <img v-if="!isPlaceholder && !isAddNew" :src="game.imageUrl" :alt="game.title" class="game-img" />

   


    <div v-else-if="isAddNew" class="placeholder">
      <span class="add-icon">+</span>
    
    </div>

 
    <button v-if="!isPlaceholder && !isAddNew" class="wishlist-btn" @click.stop="toggleWishlist"  @click="addToWishlist" :disabled="wishlistLoading"  :class="{ 'pulse-added': isAdded }">
      <i :class="isInWishlist ? 'fas fa-heart' : 'far fa-heart'"></i>
    </button>

    
    <div v-if="!isPlaceholder && !isAddNew" class="game-info">
      <router-link :to="{ 
        name: 'GameDetail',
        params: { id: game.id } }" 
       class="game-card-link"
        @click.stop>
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

   
    <div v-else-if="isPlaceholder" class="placeholder-button">
      <button @click="router.push('/catalog')" @click.stop="$emit('loadMore')">Показать ещё</button>
    </div>

 
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
const authStore = useAuthStore()
const router = useRouter() 
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
const isAdded = ref(false);

const addToWishlist = () => {
 if(authStore.isAuthenticated){
   isAdded.value = true;

  

  setTimeout(() => {
    isAdded.value = false;
  }, 1000);
 }
 else{
  notificationStore.showWarning('Войдите в аккаунт, чтобы добавить в корзину', 3000)    
 }
 
};
const cartStore = useCartStore()
const wishlistStore = useWishlistStore()
const wishlistLoading = ref(false)
const notificationStore = useNotificationStore() 

const isInWishlist = computed(() => {
  if ((props.isPlaceholder || props.isAddNew) && !authStore.isAuthenticated) return false
  return wishlistStore.isGameInWishlist(props.game.id)
})

const toggleWishlist = async (e) => {
  if(authStore.isAuthenticated){
e.stopPropagation()
  wishlistLoading.value = true
  try {
    await wishlistStore.toggleWishlist(props.game)
  } catch (err) {
    console.error('Не удалось обновить избранное:', err)
  } finally {
    wishlistLoading.value = false
  }
  }
  else{
     notificationStore.showWarning('Войдите в аккаунт, чтобы добавить в корзину', 3000)
  }
  
}

const buttonText = computed(() => {
  if (props.isPlaceholder || props.isAddNew) return ''
  if (props.game.price === 0) return 'Играть'
  if (props.game.tag?.includes('окт') || props.game.tag === 'Скоро') return 'Предзаказ'
  return 'В корзину'
})

const addToCart = (e) => {
  if(authStore.isAuthenticated){
      e.stopPropagation() 
  cartStore.addToCart(props.game)
  
  }
  else{
     notificationStore.showWarning('Войдите в аккаунт, чтобы добавить в корзину', 3000)
  }

}

const tagStyle = computed(() => {
  if (props.isPlaceholder || props.isAddNew) return {}
  if (props.game.tag === 'Бесплатно') {
    return { background: '#00D98D', color: '#000' }
  }
  if (props.game.tag?.includes('окт') || props.game.tag === 'Новинка') {
    return { background: '#FF6B35', color: '#000' }
  }
  return {}
})
</script>

<style lang="scss" scoped src="@/assets/style/components/cart/main.scss"></style>

