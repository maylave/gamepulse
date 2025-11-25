<template>
  <div id="app">
    <LoadingSpinner
      :is-loading="uiStore.isLoading"
      :text="uiStore.loadingMessage"
    />
    <router-view />
  </div>
</template>

<script setup>
import { useUiStore } from '@/stores/ui'
import LoadingSpinner from './components/global/LoadingSpinner.vue'

const uiStore = useUiStore()
</script>

<style>

.app-wrapper {
  padding-bottom: 80px; 
}

@media (min-width: 769px) {
  .app-wrapper {
    padding-bottom: 0;
  }
}
</style>