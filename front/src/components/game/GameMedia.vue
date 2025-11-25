<!-- src/components/game/GameMedia.vue -->
<template>
  <div class="media-section">
    <!-- Главное изображение -->
    <div class="main-image">
      <img :src="game.imageUrl" :alt="game.title" />
    </div>

    
    <div class="thumbnails">
      <div
        v-for="(img, index) in game.images"
        :key="index"
        class="thumbnail"
        :class="{ active: currentIndex === index }"
        @click="currentIndex = index"
      >
        <img :src="img" :alt="`Скриншот ${index + 1}`" />
      </div>
    </div>

   
    <div v-if="game.trailer" class="trailer-section">
      <h3>Трейлер</h3>
      <div class="video-wrapper">
        <video
          controls
          :poster="mainImage"
          preload="metadata"
        >
          <source :src="game.trailer" type="video/mp4" />
          Ваш браузер не поддерживает видео.
        </video>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  game: {
    type: Object,
    required: true
  }
})

const currentIndex = ref(0)
const mainImage = computed(() => {
  return props.game.imageUrl?.[currentIndex.value] || props.game.imageUrl
})
</script>

<style scoped lang="scss">

@use '@/assets/style/global/_variables' as *;
.media-section {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.main-image img {
  width: 100%;
  height: 500px;
  object-fit: cover;
  border-radius: 16px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.5);
}

.thumbnails {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
  gap: 1rem;
}

.thumbnail {
  border-radius: 12px;
  overflow: hidden;
  cursor: pointer;
  opacity: 0.7;
  transition: all 0.3s ease;

  &:hover,
  &.active {
    opacity: 1;
    transform: scale(1.05);
    box-shadow: 0 5px 15px rgba($color-primary, 0.3);
  }

  img {
    width: 100%;
    height: 80px;
    object-fit: cover;
  }
}

.trailer-section {
  h3 {
    margin-bottom: 1rem;
    color: $color-text;
    font-size: 1.5rem;
  }
}

.video-wrapper {
  position: relative;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.5);
  aspect-ratio: 16 / 9; 

  video {
    width: 100%;
    height: 100%;
    object-fit: contain;
    background-color: $color-bg;
    outline: none;

  
    &::-webkit-media-controls {
      background-color: rgba($color-card, 0.8);
    }

    &::-webkit-media-controls-play-button {
      background-color: $color-primary;
      border-radius: 50%;
    }
  }
}

// Поддержка старых браузеров (если нужно)
@supports not (aspect-ratio: 16 / 9) {
  .video-wrapper {
    position: relative;
    padding-bottom: 56.25%; /* 16:9 */
    height: 0;

    video {
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
    }
  }
}

// Адаптивность
@media (max-width: 768px) {
  .main-image img {
    height: 350px;
  }

  .video-wrapper {
    border-radius: 12px;
  }
}
</style>