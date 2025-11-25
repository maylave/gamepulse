<!-- src/views/GameView.vue -->
<template>
  <div class="app">
    <Header />
    <main class="container">
      <!-- Загрузка -->
      <div v-if="gameStore.loading" class="loading">
        <LoadingSpinner></LoadingSpinner>
      </div>

      <!-- Ошибка: игра не найдена -->
      <div v-else-if="gameStore.error" class="error-section">
        <div class="error-content">
          <h2>Игра не найдена</h2>
          <p>К сожалению, игра с ID "{{ route.params.id }}" не существует.</p>
          <div class="error-actions">
            <button class="btn btn-primary" @click="goBack">
              Вернуться назад
            </button>
            <router-link to="/catalog" class="btn btn-secondary">
              В каталог
            </router-link>
          </div>
        </div>
      </div>


      <div v-else-if="game" class="game-page">
        <button class="back-btn" @click="goBack" aria-label="Вернуться назад">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M19 12H5M12 19l-7-7 7-7" />
          </svg>
          Назад
        </button>

        <div class="breadcrumbs">
          <router-link to="/">Главная</router-link> / 
          <router-link to="/catalog">Каталог</router-link> / 
          <span>{{ game.title }}</span>
        </div>

        <div class="game-layout">
          <GameMedia :game="game" />
          <GameInfo 
            :game="game"
            :is-in-wishlist="false"
            @action="handleAction"
            @wishlist="handleWishlist"
          />
        
        </div>

        <section class="description">
          <h2>Описание</h2>
          <div v-html="game.description"></div>
        </section>
           <GameReviews  :game="game"
           ></GameReviews>
       
      </div>
    </main>
    <Footer />
  </div>
</template>

<script setup>
import { onMounted, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useGameStore } from '@/stores/gameStore'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'
import GameMedia from '@/components/game/GameMedia.vue'
import GameInfo from '@/components/game/GameInfo.vue'
import GameReviews from '../components/game/GameReviews.vue'
const route = useRoute()
const router = useRouter()
const gameStore = useGameStore()
const game = computed(() => gameStore.game)

onMounted(() => {
  const id = Number(route.params.id)
  gameStore.fetchGame(id)
})

const goBack = () => {
  if (window.history.length > 2) {
    router.go(-1)
  } else {
    router.push('/catalog')
  }
}
watch(
  () => route.params.id,
  async (newId) => {
    const id = Number(newId)
    if (isNaN(id)) {
      gameStore.error = 'Некорректный ID игры'
      return
    }

    
    gameStore.error = null
    await gameStore.fetchGame(id)

   
  },
  { immediate: true } 
)
const handleAction = () => {
  alert(`Действие с "${game.value.title}"`)
}
const handleWishlist = () => {
  alert('Добавлено в избранное')
}
</script>

<style scoped lang="scss">
@use '@/assets/style/global/_variables' as *;
.app {
  min-height: 100vh;
  background: $color-bg;
  color: $color-text;
}

.container {
  width: 90%;
  max-width: 1400px;
  margin: 0 auto;
  padding: 2rem 0;
}


// Ошибка
.error-section {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 60vh;
}

.error-content {
  text-align: center;
  max-width: 600px;
  padding: 2rem;
  background: $color-card;
  border-radius: 16px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);

  h2 {
    color: $color-text;
    font-size: 2rem;
    margin-bottom: 1rem;
  }

  p {
    color: $color-text-secondary;
    margin-bottom: 2rem;
    line-height: 1.6;
  }
}

.error-actions {
  display: flex;
  gap: 1rem;
  justify-content: center;
  flex-wrap: wrap;
}

// Кнопки ошибки
.btn {
  padding: 0.75rem 1.5rem;
  border-radius: 12px;
  font-weight: 600;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  text-decoration: none;

  &.btn-primary {
    background: linear-gradient(90deg, $color-secondary, $color-primary);
    color: #000;
    border: none;
  }

  &.btn-secondary {
    background: rgba(255, 255, 255, 0.05);
    color: $color-text;
    border: 1px solid $color-text-secondary;
  }

  &:hover {
    opacity: 0.9;
    transform: translateY(-2px);
  }
}

// Успешная страница — остальное без изменений
.back-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: transparent;
  color: $color-primary;
  border: 1px solid $color-primary;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  margin-bottom: 1.5rem;
  transition: all 0.2s ease;

  &:hover {
    background: rgba($color-primary, 0.1);
    transform: translateY(-2px);
  }

  svg {
    width: 18px;
    height: 18px;
  }
}

.breadcrumbs {
  margin-bottom: 2rem;
  color: $color-text-secondary;
  a {
    color: $color-primary;
    text-decoration: none;
    &:hover { text-decoration: underline; }
  }
}

.game-layout {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 3rem;
  margin-bottom: 3rem;
}

.description {
  h2 {
    font-size: 2rem;
    margin-bottom: 1.5rem;
    color: $color-text;
  }
  p {
    line-height: 1.7;
    margin-bottom: 1rem;
    color: $color-text-secondary;
  }
}

@media (max-width: 992px) {
  .game-layout { grid-template-columns: 1fr; }
  .error-actions { flex-direction: column; }
}
</style>