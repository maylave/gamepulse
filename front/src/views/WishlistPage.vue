<template>
  <div class="wishlist-page">
    <Header />

    <main class="container">
      <h1 class="page-title">Избранное</h1>

      
     
    </main>

    <Footer />
  </div>
</template>

<script setup>

import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue'

</script>

<style scoped>
.wishlist-page {
  background-color: var(--bg-primary, #0f0f13);
  color: var(--text-primary, #fff);
  min-height: 100vh;
}

.container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 20px;
}

.page-title {
  font-size: 2rem;
  font-weight: 600;
  margin: 2.5rem 0 2rem;
  color: var(--text-primary, #fff);
}

.game-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

@media (min-width: 1400px) {
  .game-grid {
    grid-template-columns: repeat(6, 1fr);
  }
}

@media (max-width: 768px) {
  .game-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
  }
}

.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: var(--text-secondary, #aaa);
  font-size: 1.1rem;
}

.loading-indicator {
  text-align: center;
  padding: 3rem;
}

.spinner {
  width: 24px;
  height: 24px;
  border: 3px solid rgba(255, 255, 255, 0.3);
  border-top: 3px solid var(--color-primary, #e53e3e);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>