
<template>
  <Teleport to="body">
    <div v-if="notificationStore.isVisible" class="notification-overlay">
      <div class="notification-modal" :class="`notification-modal--${notificationStore.type}`">
        <div class="notification-icon">
          <i v-if="notificationStore.type === 'success'" class="fas fa-check-circle"></i>
          <i v-else-if="notificationStore.type === 'error'" class="fas fa-exclamation-circle"></i>
          <i v-else-if="notificationStore.type === 'warning'" class="fas fa-exclamation-triangle"></i>
          <i v-else class="fas fa-info-circle"></i>
        </div>
        <div class="notification-message">{{ notificationStore.message }}</div>
        <button class="notification-close" @click="notificationStore.hide">
          <i class="fas fa-times"></i>
        </button>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import { useNotificationStore } from '@/stores/NotificationStore'
const notificationStore = useNotificationStore()
</script>

<style scoped>
.notification-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  pointer-events: none;
  z-index: 10000;
}

.notification-modal {
  position: fixed;
  top: 20px;
  right: 20px;
  max-width: 350px;
  padding: 16px 20px;
  border-radius: 12px;
  background: var(--color-card);
  border: 1px solid rgba(255, 255, 255, 0.1);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.4);
  display: flex;
  align-items: center;
  gap: 12px;
  pointer-events: auto;
  animation: slideIn 0.3s ease forwards;
}

.notification-modal--success {
  border-left: 4px solid #4caf50;
}
.notification-modal--error {
  border-left: 4px solid #f44336;
}
.notification-modal--warning {
  border-left: 4px solid #ff9800;
}
.notification-modal--info {
  border-left: 4px solid #2196f3;
}

.notification-icon i {
  font-size: 1.4rem;
}
.notification-modal--success .notification-icon i { color: #4caf50; }
.notification-modal--error .notification-icon i { color: #f44336; }
.notification-modal--warning .notification-icon i { color: #ff9800; }
.notification-modal--info .notification-icon i { color: #2196f3; }

.notification-message {
  flex: 1;
  color: var(--color-text);
  font-size: 0.95rem;
  line-height: 1.4;
}

.notification-close {
  background: none;
  border: none;
  color: var(--color-text-secondary);
  font-size: 1.2rem;
  cursor: pointer;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: background 0.2s;
}

.notification-close:hover {
  background: rgba(255, 255, 255, 0.1);
  color: var(--color-text);
}

@keyframes slideIn {
  from {
    transform: translateX(100%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}
</style>