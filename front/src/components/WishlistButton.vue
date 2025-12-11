<template>
  <div 
    class="wishlist-heart" 
    @click="goToWishlist"
    :class="{ 'is-animating': isAnimating }"
  >
    <svg class="heart-icon" viewBox="0 0 24 24" width="24" height="24">
   
      <path
        v-if="wishlistStore.itemCount > 0 && !isAnimating && authStore.isAuthenticated"
        class="heart-fill"
        d="M12 21.35l-1.45-1.32C5.4 15.36 2 12.28 2 8.5 2 5.42 4.42 3 7.5 3c1.74 0 3.41.81 4.5 2.09C13.09 3.81 14.76 3 16.5 3 19.58 3 22 5.42 22 8.5c0 3.78-3.4 6.86-8.55 11.54L12 21.35z"
        fill="#ffffff"
      />
    
      <path
        class="heart-stroke"
        d="M12 21.35l-1.45-1.32C5.4 15.36 2 12.28 2 8.5 2 5.42 4.42 3 7.5 3c1.74 0 3.41.81 4.5 2.09C13.09 3.81 14.76 3 16.5 3 19.58 3 22 5.42 22 8.5c0 3.78-3.4 6.86-8.55 11.54L12 21.35z"
        fill="none"
        stroke="currentColor"
        stroke-width="2"
        vector-effect="non-scaling-stroke"
      />
    </svg>
  </div>
</template>


<script setup>
import { ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useWishlistStore } from '@/stores/wishlistStore'
import { useNotificationStore } from '@/stores/NotificationStore'
import { useAuthStore } from '@/stores/auth'
const wishlistStore = useWishlistStore()
const authStore = useAuthStore()
const router = useRouter()
const notificationStore = useNotificationStore() 
const isAnimating = ref(false)

watch(
  () => wishlistStore.itemCount,
  (newCount, oldCount) => {
    if (newCount > oldCount && authStore.isAuthenticated) {
      isAnimating.value = true
      setTimeout(() => {
        isAnimating.value = false
      }, 1000)
    }
  }
)

const goToWishlist = () => {
  if (authStore.isAuthenticated) {
    router.push('/wishlist')
  } else {
   
    notificationStore.showWarning('Войдите в аккаунт, чтобы использовать избранное', 3000)
  }
}
</script>

<style scoped lang="scss">
@use '@/assets/style/global/_variables' as *;

.wishlist-heart {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  cursor: pointer;
  color: #ffffff; 
  border-radius: 50%;
  transition: transform 0.2s;
}

.wishlist-heart:active {
  transform: scale(0.95);
}

.heart-icon {
  width: 24px;
  height: 24px;
  pointer-events: none;
}

.heart-stroke {
  stroke-dasharray: 76;
  stroke-dashoffset: 0;
  transition: stroke 0.3s ease;
}


.wishlist-heart:not(.is-animating) .heart-stroke {
  stroke: #ffffff;
}


.wishlist-heart.is-animating .heart-stroke {
  stroke: $color-primary;
  stroke-dashoffset: 76;
  animation: drawHeart 1s ease-out forwards;
}

@keyframes drawHeart {
  to {
    stroke-dashoffset: 0;
  }
}
</style>