<!-- src/views/Profile/components/OrdersList.vue -->
<template>
  <div class="tab-content">
    <h2 class="section-title">Мои покупки</h2>

    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Загрузка заказов...</p>
    </div>

    <div v-else-if="orders.length === 0" class="empty-state">
      <i class="fas fa-receipt"></i>
      <p>У вас пока нет покупок</p>
    </div>

    <table v-else class="orders-table">
      <thead>
        <tr>
          <th>Игра</th>
          <th>Дата</th>
          <th>Статус</th>
          <th>Сумма</th>
          <th>Действие</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="order in orders" :key="order.id">
          <td data-label="Игра">{{ order.gameTitle }}</td>
          <td data-label="Дата">{{ formatDate(order.date) }}</td>
          <td data-label="Статус">
            <span :class="['order-status', getStatusClass(order.status)]">
              {{ getStatusText(order.status) }}
            </span>
          </td>
          <td data-label="Сумма">{{ order.amount }} ₽</td>
          <td data-label="Действие">
            <span
              v-if="order.status === 'completed'"
              class="download-key"
              @click="downloadKey(order.key)"
            >
              Скачать ключ
            </span>
            <span v-else>—</span>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'

const loading = ref(false)
const orders = ref([])

const authStore = useAuthStore()

const loadOrders = async () => {
  loading.value = true
  try {
    const response = await fetch('/api/user/orders', {
      headers: { 'Authorization': `Bearer ${authStore.token}` }
    })
    if (!response.ok) throw new Error('Failed to load orders')
    orders.value = await response.json()
  } catch (error) {
  
  } finally {
    loading.value = false
  }
}

const downloadKey = (key) => {
  navigator.clipboard.writeText(key)
   
}

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('ru-RU', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  })
}

const getStatusText = (status) => status === 'completed' ? 'Активировано' : 'Ожидает релиза'
const getStatusClass = (status) => status === 'completed' ? 'status-completed' : 'status-pending'

onMounted(() => {
  loadOrders()
})
</script>
<style scoped lang="scss" src="@/assets/style/views/profile/main.scss" ></style>