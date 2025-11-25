<template>
  <div class="games-carousel">
    <div class="games-carousel__header">
      <h2 v-if="title" class="games-carousel__title">{{ title }}</h2>

      <!-- Кнопки навигации — справа от заголовка -->
      <div v-if="showArrows" class="games-carousel__nav-controls">
        <button
          class="games-carousel__nav-btn games-carousel__nav-btn--prev"
          :disabled="!canGoPrev"
          @click="goPrev"
          aria-label="Предыдущие игры"
        >
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M15 18l-6-6 6-6" />
          </svg>
        </button>
        <button
          class="games-carousel__nav-btn games-carousel__nav-btn--next"
          :disabled="!canGoNext"
          @click="goNext"
          aria-label="Следующие игры"
        >
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M9 6l6 6-6 6" />
          </svg>
        </button>
      </div>
    </div>

    <div class="games-carousel__wrapper">
      <UniversalCarousel
        ref="carouselRef"
        :items="carouselItems"
        @change="onSlideChange"
        @click-slide="onGameClick"
        class="universalCarousel"
      >
        <template #default="{ item }">
          <GameCard
            :game="item"
            :isPlaceholder="!!item.isPlaceholder"
            :isAddNew="!!item.isAddNew"
            @loadMore="loadMoreGames"
            @add-to-cart="$emit('add-to-cart', $event)"
            @createNewGame="openCreateGameModal"
            class="game"
          />
        </template>
      </UniversalCarousel>
    </div>

   
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import GameCard from '@/components/game-card.vue'
import UButton from './global/UButton.vue'

const props = defineProps({
  games: { type: Array, required: true },
  title: { type: String, default: '' },
  showArrows: { type: Boolean, default: true },
  showSeeMore: { type: Boolean, default: true },
  showAddNew: { type: Boolean, default: false }
})

const emit = defineEmits(['add-to-cart', 'game-click'])

const carouselRef = ref(null)
const currentSlide = ref(0)


watch(() => props.games, () => {
  currentSlide.value = 0
})


const carouselItems = computed(() => {
  let items = [...props.games]

  if (props.showAddNew) {
    items.push({ isAddNew: true })
  }

  if (props.showSeeMore) {
    items.push({ isPlaceholder: true })
  }

  return items
})


const totalSlides = computed(() => carouselItems.value.length)

const isOnLastSlide = computed(() => {
  return totalSlides.value > 0 && currentSlide.value >= totalSlides.value - 1
})

const canGoPrev = computed(() => currentSlide.value > 0)
const canGoNext = computed(() => totalSlides.value > 0 && currentSlide.value < totalSlides.value - 1)

// События карусели
const onSlideChange = (index) => {
  currentSlide.value = index
}

const onGameClick = (game) => {
  emit('game-click', game)
}

// Управление вручную через кнопки (если UniversalCarousel поддерживает .prev() / .next())
const goPrev = () => {
  if (canGoPrev.value && carouselRef.value?.prev) {
    carouselRef.value.prev()
  }
}

const goNext = () => {
  if (canGoNext.value && carouselRef.value?.next) {
  carouselRef.value.next()
  }
}

// Заглушка — если вызывается из GameCard
const loadMoreGames = () => {
  // Можно оставить пустой или эмитить 'see-more'
}

const openCreateGameModal = () => {
  // Например: emit('open-create-modal') или router push
  // Пока оставим как заглушку — ты можешь расширить по нужде
}
</script>

<style scoped>
.games-carousel {
  margin-bottom: 3.5rem;
}

.games-carousel__header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.25rem;
}

.games-carousel__title {
  text-align: left;
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--text-primary, #fff);
}

.games-carousel__nav-controls {
  display: flex;
  gap: 0.5rem;
}

.games-carousel__nav-btn {
  background: rgba(0, 0, 0, 0.5);
  border: none;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  cursor: pointer;
  transition: background 0.2s ease;
  z-index: 2;
}

.games-carousel__nav-btn:hover:not(:disabled) {
  background: rgba(0, 0, 0, 0.8);
}

.games-carousel__nav-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

/* Мобильная версия — скрываем стрелки */
@media (max-width: 768px) {
  .games-carousel__nav-controls {
    display: none;
  }
}

.games-carousel__wrapper {
  position: relative;
  padding: 0;
}

.universalCarousel {
  border-radius: 20px;
  padding-left: 20px;
}

.game {
  z-index: 2;
  margin-top: 30px;
  margin-bottom: 30px;
}

:deep(.carousel-slide) {
  width: 250px !important;
  flex: none !important;
  max-width: none !important;
}

:deep(.carousel-track) {
  gap: 1.5rem;
}
</style>