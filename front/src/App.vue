<template>
  <div id="app" class="app-layout">
    <LoadingSpinner
      :is-loading="uiStore.isLoading"
      :text="uiStore.loadingMessage"
    />
    <router-view />
     <NotificationModal />
  </div>
</template>

<script setup>
import { useUiStore } from '@/stores/ui'
import LoadingSpinner from './components/global/LoadingSpinner.vue'
import NotificationModal from '@/components/NotificationModal.vue'
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
#app{
   min-height: 100vh;
}

</style>