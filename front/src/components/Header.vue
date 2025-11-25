<template>
  <header class="header">
    <div class="container header-content">
      <router-link to="/" class="logo">
        <img class="img-logo" :src="logoGamePlus" alt="GamePulse Logo" />
        GamePulse
      </router-link>

      <nav class="desktop-nav">
        <router-link to="/">Главная</router-link>
        <router-link to="/catalog">Каталог</router-link>
        <router-link to="/sales">Акции</router-link>
        <router-link to="/new">Новинки</router-link>
        <router-link to="/support">Поддержка</router-link>
      </nav>

      <div class="search-box">
        <i class="fas fa-search"></i>
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Поиск игр..."
          @keyup.enter="performSearch"
        />
      </div>

      <div class="desktop-actions">
        <div class="cart-icon" @click="openCart" :class="{ jump: shouldJump }">
          <i class="fas fa-shopping-cart"></i>
          <span v-if="cartStore.itemCount > 0" class="cart-count">{{ cartStore.itemCount }}</span>
        </div>

        <div
          v-if="authStore.isAuthenticated"
          ref="desktopMenuRef"
          class="user-avatar-wrapper"
          @click="toggleMenu"
        >
          <div
            class="user-avatar desktop"
            :style="{
              backgroundImage: `linear-gradient(45deg, ${authStore.userColor || 'var(--color-primary, #e53935)'}, white)`
            }"
          >
            {{ authStore.userInitial || 'U' }}
          </div>
          <dropdowProfile
            :is-visible="menuVisible"
            @go-to-profile="() => { closeMenu(); router.push('/profile') }"
            @go-to-orders="() => { closeMenu(); router.push('/orders') }"
            @go-to-settings="() => { closeMenu(); router.push('/settings') }"
            @go-to-wishlist="() => { closeMenu(); router.push('/wishlist') }"
            @logout="() => { authStore.logout(); closeMenu() }"
          />
        </div>

        <UButton v-else @click="goToAuth" size="sm">Войти</UButton>
      </div>
    </div>

    <AdminControls />
  </header>

  <div v-if="isMobile" class="mobile-nav" :class="{ hidden: isScrolledDown && !isScrollingUp }">
    <router-link to="/" class="nav-item" :class="{ active: currentRoute === '/' }">
      <i class="fas fa-home"></i>
      <span>Главная</span>
    </router-link>
    <router-link to="/catalog" class="nav-item" :class="{ active: currentRoute.startsWith('/catalog') }">
      <i class="fas fa-gamepad"></i>
      <span>Каталог</span>
    </router-link>
    <div class="nav-item cart-center" @click="openCart" :class="{ jump: shouldJump }">
      <i class="fas fa-shopping-cart"></i>
      <span>Корзина</span>
      <span v-if="cartStore.itemCount > 0" class="mobile-cart-count">{{ cartStore.itemCount }}</span>
    </div>

    <template v-if="authStore.isAuthenticated">
      <div
        ref="mobileMenuRef"
        class="nav-item user-avatar-mobile"
        @click="toggleMenu"
      >
        <div
          class="user-avatar mobile"
          :style="{
            backgroundImage: `linear-gradient(45deg, ${authStore.userColor || '#e53935'}, white)`
          }"
        >
          {{ authStore.userInitial || 'U' }}
        </div>
        <dropdowProfile
          :is-visible="menuVisible"
          @go-to-profile="() => { closeMenu(); router.push('/profile') }"
          @go-to-orders="() => { closeMenu(); router.push('/orders') }"
          @go-to-settings="() => { closeMenu(); router.push('/settings') }"
          @go-to-wishlist="() => { closeMenu(); router.push('/wishlist') }"
          @logout="() => { authStore.logout(); closeMenu() }"
        />
      </div>
    </template>
    <template v-else>
      <router-link to="/auth" class="nav-item">
        <i class="fas fa-sign-in-alt"></i>
        <span>Войти</span>
      </router-link>
    </template>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'
import AdminControls from './AdminControls.vue'
import logoGamePlus from '@/assets/logoGamePlus.svg'
import dropdowProfile from './dropdowProfile.vue'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()
const cartStore = useCartStore()

const searchQuery = ref('')
const menuVisible = ref(false) // ← единственный источник истины
const shouldJump = ref(false)
const isMobile = ref(window.innerWidth <= 768)

const currentRoute = computed(() => route.path)

const isScrolledDown = ref(false)
const isScrollingUp = ref(false)
let lastScrollY = window.scrollY
let scrollTimeout = null

const handleScroll = () => {
  const currentScrollY = window.scrollY
  if (currentScrollY > lastScrollY && currentScrollY > 100) {
    isScrolledDown.value = true
    isScrollingUp.value = false
  } else if (currentScrollY < lastScrollY) {
    isScrollingUp.value = true
    isScrolledDown.value = false
    clearTimeout(scrollTimeout)
    scrollTimeout = setTimeout(() => {
      isScrollingUp.value = false
    }, 150)
  }
  lastScrollY = currentScrollY
}

const desktopMenuRef = ref(null)
const mobileMenuRef = ref(null)

const toggleMenu = () => {
  menuVisible.value = !menuVisible.value
}

const closeMenu = () => {
  menuVisible.value = false
}

const performSearch = () => {
  if (searchQuery.value.trim()) {
    router.push({ name: 'Catalog', query: { q: searchQuery.value.trim() } })
  } else {
    router.push({ name: 'Catalog' })
  }
}

const openCart = () => {
  closeMenu()
  router.push('/cartGame')
}

const goToAuth = () => {
  closeMenu()
  router.push('/auth')
}

const handleClickOutside = (event) => {
  if (
    desktopMenuRef.value?.contains(event.target) ||
    mobileMenuRef.value?.contains(event.target)
  ) return
  closeMenu()
}

const updateIsMobile = () => {
  isMobile.value = window.innerWidth <= 768
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
  window.addEventListener('resize', updateIsMobile)
  window.addEventListener('scroll', handleScroll, { passive: true })
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
  window.removeEventListener('resize', updateIsMobile)
  window.removeEventListener('scroll', handleScroll)
  if (scrollTimeout) clearTimeout(scrollTimeout)
})

watch(
  () => cartStore.itemCount,
  (newCount, oldCount) => {
    if (newCount > oldCount) {
      shouldJump.value = true
      setTimeout(() => (shouldJump.value = false), 500)
    }
  }
)
</script>

<style lang="scss" scoped src="@/assets/style/components/header/main.scss"></style>