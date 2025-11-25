<!-- GamesInfiniteList.vue (локальная пагинация) -->
<template>
  <div class="games-infinite-list">
    <h2 v-if="title" class="section-title">{{ title }}</h2>

    <div class="games-grid">
      <GameCard
        v-for="game in displayedGames"
        :key="game.id"
        :game="game"
        @add-to-cart="$emit('add-to-cart', $event)"
        @click.native="$emit('game-click', game)"
      />
    </div>

    <div v-if="!hasMore" class="end-message">Показаны все игры</div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

import GameCard from '@/components/game-card.vue'

const props = defineProps({
  title: { type: String, default: '' },
  perPage: { type: Number, default: 10 }
})

const emit = defineEmits(['add-to-cart', 'game-click'])


const currentPage = ref(1)

defineExpose({ loadMore: () => {
  if (hasMore.value) {
    currentPage.value++
  }
} })
</script>

<style scoped lang="scss">
.games-infinite-list {
  max-width: 1200px;
  margin: 0 auto;
}

.section-title {
  margin: 2rem 0 1.25rem;
  color: var(--text-primary, #fff);
  font-size: 1.5rem;
}

.games-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 1.5rem;
}

.end-message {
  text-align: center;
  margin-top: 1.5rem;
  color: var(--text-secondary, #aaa);
}
</style>