<template>
  <div v-if="isAdmin" class="admin-controls">
    <div class="admin-controls-inner">
      <h2 class="admin-controls-title">Админка</h2>
      <div class="admin-controls-buttons">
        <nav>
         <router-link to="table">
         таблица
         </router-link>
          <router-link to="404">
        модерация
         </router-link>
       
        </nav>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed} from 'vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

const isAdmin = computed(() => authStore.user?.roles?.includes('Admin'))






</script>

<style scoped lang="scss">
@use '@/assets/style/global/_variables' as *;
nav {
  display: flex;
  gap: 1.5rem;

  a {
    color: $color-text;
    text-decoration: none;
    font-weight: 500;
    padding: 0.4rem 0;
    border-bottom: 2px solid transparent;
    transition: border-color 0.2s;
    border:1px solid ;
    border-radius: 10px;
    padding: 0.6rem;

    &.router-link-active,
    &:hover {
      border-color: $color-primary;
      color: $color-primary;
    }
  }
}
.admin-controls {
    margin: 20px 30px 0 30px;

  padding: 12px 20px;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}
:deep(.bt-admin) {
  background: rgba($color-primary, $alpha: 0.8) !important;
  border-radius: 5px !important;
  color: rgb(0, 0, 0) !important;
  margin-left: 20px;
}
.admin-controls-inner {
  display: flex;
  justify-content: space-between;

  align-items: center;
  gap: 20px;
}

.admin-controls__title {
  margin: 0;
  font-size: 18px;
  color: #333;
}


@media (max-width: 768px) {
  .admin-controls {
    margin: 16px;
    padding: 16px;
  }

  .admin-controls-title {
    font-size: 1.125rem;
    text-align: center;
    flex-basis: 100%;
    order: 1;
  }

  .admin-nav {
    justify-content: center;
    width: 100%;
    order: 2;
    gap: 10px;
  }

  .admin-nav__link {
    flex: 1;
    min-width: 120px;
    text-align: center;
    padding: 0.75rem;
    font-size: 0.95rem;
  }
}

@media (max-width: 480px) {
  .admin-nav__link {
    min-width: 100px;
    padding: 0.65rem 0.5rem;
    font-size: 0.9rem;
  }
}
</style>