<template>
  <div class="genre-section">
    <h2 class="section-title">Жанры</h2>

    <UniversalCarousel
      class="categories"
      :items="genres"
      :per-view="2"
      :breakpoints="{
        768: { perView: 2 },
        480: { perView: 1 }
      }"
      :show-controls="true"
      @change="onCarouselChange"
    >
      <template #default="{ item }">
        <div
          class="category-card"
          role="button"
          tabindex="0"
          @click="selectGenre(item)"
          @keyup.enter="selectGenre(item)"
        >
          <div class="category-icon" v-html="item.icon"></div>
          <div class="category-name">{{ item.name }}</div>
        </div>
      </template>
    </UniversalCarousel>
  </div>
</template>

<script setup>
import { defineEmits } from 'vue'


const genres = [
  { id: 1, name: 'Экшен', icon: '<i class="fas fa-fist-raised"></i>' },
  { id: 2, name: 'RPG', icon: '<i class="fas fa-dragon"></i>' },
  { id: 3, name: 'Стратегии', icon: '<i class="fas fa-chess-rook"></i>' },
  { id: 4, name: 'Гонки', icon: '<i class="fas fa-car"></i>' },
  { id: 5, name: 'Спорт', icon: '<i class="fas fa-basketball-ball"></i>' },
  { id: 6, name: 'Хоррор', icon: '<i class="fas fa-ghost"></i>' }
]


const emit = defineEmits(['genre-selected'])

const selectGenre = (genre) => {
  emit('genre-selected', genre)
}
</script>

<style lang="scss" scoped src="@/assets/style/components/GenreCategories/main.scss"></style>


<style scoped>
.categories {
 
  gap: 1.5rem;
  margin-bottom: 3.5rem;
}


</style>