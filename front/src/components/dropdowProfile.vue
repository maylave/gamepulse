<!-- src/components/dropdowProfile.vue -->
<template>
  <transition name="menu-fade">
    <div v-show="isVisible" class="user-menu-wrapper" @click.stop>
      <div class="user-menu">
        <button class="menu-item" @click="goToProfile">
          <i class="fas fa-user"></i> Профиль
        </button>
        <button class="menu-item" @click="goToOrders">
          <i class="fas fa-receipt"></i> Мои заказы
        </button>
        <button class="menu-item" @click="goToSettings">
          <i class="fas fa-cog"></i> Настройки
        </button>
        <button class="menu-item" @click="goToWishlist">
          <i class="fas fa-heart"></i> Избранное
        </button>
        <div class="menu-divider"></div>
        <button class="menu-item logout" @click="handleLogout">
          <i class="fas fa-sign-out-alt"></i> Выйти
        </button>
      </div>
    </div>
  </transition>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const props = defineProps({
  isVisible: { type: Boolean, default: false }
})

const emit = defineEmits(['go-to-profile', 'go-to-orders', 'go-to-settings', 'go-to-wishlist', 'logout'])

const goToProfile = () => emit('go-to-profile')
const goToOrders = () => emit('go-to-orders')
const goToSettings = () => emit('go-to-settings')
const goToWishlist = () => emit('go-to-wishlist')
const handleLogout = () => emit('logout')
</script>

<style scoped lang="scss">
@use '@/assets/style/global/_variables' as *;

.user-menu-wrapper {
  position: relative;
  z-index: 1000;
}

.user-menu {
  position: absolute;
  top: calc(100% + 8px);
  right: 0;
  background: $color-card;
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
  padding: 8px 0;
  min-width: 200px;
  border: 1px solid $color-border;
     &.animated {
    opacity: 0;
    transform: translateY(-10px);
    transition: opacity 0.2s ease, transform 0.2s ease;
  }

  &.animated.enter {
    opacity: 1;
    transform: translateY(0);
  }
  
  @media (max-width: 768px) {
    position: fixed;
    top: auto;
    bottom: 70px;
    right: 16px; 
    left: auto;
    width: calc(100% - 32px);
    max-width: 320px;
    min-width: auto;
    border-radius: 16px;
    box-shadow: 0 -4px 20px rgba(0, 0, 0, 0.2);
    padding: 16px 0;
    background: #0f0c14; 
  }
}
@media (min-width: 768px) {
    .menu-fade-enter-active,
.menu-fade-leave-active {
  transition: opacity 0.2s ease, transform 0.2s ease;
}

.menu-fade-enter-from {
  opacity: 0;
  transform: translateY(-5px);
}

.menu-fade-leave-to {
  opacity: 0;
  transform: translateY(-5px);
}

}
.menu-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 16px;
  width: 100%;
  text-align: left;
  font-weight: 500;
  color: $color-text;
  border-radius: 8px;
  margin: 0 8px 4px;
  background: transparent;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;


  &:active {
    background-color: $color-bg-hover;
  }

  &.logout {
    color: $color-primary;
    &:active {
      background-color: rgba($color-primary, 0.1);
    }
  }

  @media (max-width: 768px) {
    background: transparent;
    color: white;
    padding: 12px 20px;
    margin: 8px 16px;
    border-radius: 0;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);

    &:active {
      background: rgba(255, 255, 255, 0.05);
      border-bottom: 1px solid rgba(255, 255, 255, 0.2);
    }

    i {
      color: white;
    }

    &.logout {
      color: white;
      border-bottom: 1px solid rgba(255, 255, 255, 0.1);

      &:active {
        background: rgba(255, 0, 0, 0.05);
        border-bottom: 1px solid rgba(255, 0, 0, 0.2);
      }
    }
  }
}


.menu-divider {
  height: 1px;
  background-color: $color-border;
  margin: 8px 0;

  @media (max-width: 768px) {
    margin: 12px 16px;
    background-color: rgba(255, 255, 255, 0.1);
  }
}
</style>