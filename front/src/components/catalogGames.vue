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

defineExpose({ 
  loadMore: () => {
    if (hasMore.value) {
      currentPage.value++
    }
  } 
})
</script>

<style scoped lang="scss">
.games-infinite-list {
  max-width: 1200px;
  justify-content: center;
  padding: 0 1rem; 
}

.section-title {
  margin: 2rem 0 1.25rem;
  color: var(--text-primary, #fff);
  font-size: 1.5rem;
  text-align: left;
}

.games-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 1.5rem;
  justify-content: center; 
  padding: 0.5rem 0;
  align-items: start; 
}


@media (max-width: 768px) {
  .games-grid {
    grid-template-columns: repeat(auto-fill, minmax(150px, 1fr)); 
    gap: 1rem;
    justify-items: center;
    max-width: 250px;
    
  }

  .section-title {
    font-size: 1.3rem;
    margin: 1.5rem 0 1rem;
  }
}


@media (max-width: 480px) {
  .games-grid {
    grid-template-columns: repeat(auto-fill, minmax(130px, 1fr)); // ← ещё меньше
    gap: 0.8rem;
     max-width: 250px;
  }

  .games-infinite-list {
    padding: 0 0.8rem;
  }
}

.end-message {
  text-align: center;
  margin-top: 1.5rem;
  color: var(--text-secondary, #aaa);
  padding: 0 1rem;
}
</style>