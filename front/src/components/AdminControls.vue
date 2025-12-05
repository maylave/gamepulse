<template>
  <div v-if="hasAdminAccess" class="admin-controls">
    <div class="admin-controls-inner">
      <h2 class="admin-controls-title">Панель управления</h2>
      <nav class="admin-nav">
        <!-- Полная админка — только для Admin -->
        <router-link 
          v-if="isAdmin" 
          to="/table"
          class="admin-nav__link"
        >
          Таблица
        </router-link>

        <!-- Поддержка — для Support и Admin -->
        <router-link 
          v-if="canAccessSupport" 
          to="/supportAdmin"
          class="admin-nav__link"
        >
          Поддержка
        </router-link>

        <!-- Модерация — для Moderator и Admin -->
        <router-link 
          v-if="canAccessModeration" 
          to="/moderation"
          class="admin-nav__link"
        >
          Модерация
        </router-link>
      </nav>
    </div>
  </div>

  <!-- Кнопка "Добавить игру" — для SuperUser и Admin -->
  <div v-if="canAddGames" class="add-game-button-container">
    <router-link to="/add-game" class="add-game-btn">
      + Добавить игру
    </router-link>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

// Показываем панель, если роль выше User
const hasAdminAccess = computed(() => 
  authStore.isAdmin || 
  authStore.isSuperUser || 
  authStore.isSupport || 
  authStore.isModerator
)

// Доступ к конкретным разделам
const isAdmin = computed(() => authStore.isAdmin)
const canAccessSupport = computed(() => authStore.isSupport || authStore.isAdmin)
const canAccessModeration = computed(() => authStore.isModerator || authStore.isAdmin)
const canAddGames = computed(() => authStore.isSuperUser || authStore.isAdmin)
</script>

<style scoped lang="scss">
@use '@/assets/style/global/_variables' as *;

/* === Основная панель управления === */
.admin-controls {
  margin: 20px 30px 0 30px;
  padding: 12px 20px;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  background: rgba(30, 30, 50, 0.3);
  border-radius: 12px;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(80, 80, 120, 0.3);
}

.admin-controls-inner {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 20px;
  flex-wrap: wrap;
}

.admin-controls-title {
  margin: 0;
  font-size: 18px;
  color: var(--color-text);
  white-space: nowrap;
}

.admin-nav {
  display: flex;
  gap: 1.2rem;
  flex-wrap: wrap;
}

.admin-nav__link {
  color: var(--color-text);
  text-decoration: none;
  font-weight: 500;
  padding: 0.6rem 1rem;
  border: 1px solid rgba(100, 100, 150, 0.4);
  border-radius: 8px;
  transition: all 0.2s ease;
  background: rgba(0, 0, 0, 0.1);
  white-space: nowrap;

  &:hover,
  &.router-link-active {
    border-color: $color-primary;
    color: $color-primary;
    background: rgba($color-primary, 0.08);
    transform: translateY(-1px);
  }
}

/* === Кнопка "Добавить игру" === */
.add-game-button-container {
  position: fixed;
  bottom: 2rem;
  right: 2rem;
  z-index: 1000;
}

.add-game-btn {
  display: inline-block;
  background: none;
  border: 2px solid #333;
  color: white;
  box-shadow: 0 0 15px rgba(10, 20, 30, 0.4);
  font-size: 0.9em;
  border-radius: 10px;
  padding: 12px 20px;
  font-weight: 600;
  text-decoration: none;
  transition: all 0.2s ease;

  &:hover {
    border-color: $color-primary;
    box-shadow: 0 0 20px rgba($color-primary, 0.3);
    color: white;
  }
}

/* === Адаптивность === */
@media (max-width: 768px) {
  .admin-controls {
    margin: 16px;
    padding: 16px;
  }

  .admin-controls-inner {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }

  .admin-controls-title {
    font-size: 1.125rem;
    margin-bottom: 0;
  }

  .admin-nav {
    width: 100%;
    justify-content: flex-start;
    gap: 10px;
  }

  .admin-nav__link {
    flex: 1;
    min-width: 100px;
    text-align: center;
    padding: 0.75rem;
    font-size: 0.9rem;
  }

  .add-game-button-container {
    bottom: 1rem;
    right: 1rem;
    
    .add-game-btn {
      padding: 10px 16px;
      font-size: 0.85em;
    }
  }
}
</style>