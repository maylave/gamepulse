<!-- src/views/Profile/components/ProfileSidebar.vue -->
<template>
  <aside class="sidebar">
    <div
      v-for="tab in tabs"
      :key="tab.id"
      :class="['sidebar-item', { 'active': activeTab === tab.id }]"
      @click="setActiveTab(tab.id)"
    >
      <i :class="tab.icon"></i> {{ tab.label }}
    </div>
  </aside>
</template>

<script setup>
import { defineEmits, defineProps } from 'vue'

const emit = defineEmits(['update:activeTab'])

const props = defineProps({
  activeTab: { type: String, required: true }
})

const tabs = [
  { id: 'profile', label: 'Профиль', icon: 'fas fa-user' },
  { id: 'orders', label: 'Мои покупки', icon: 'fas fa-receipt' },
  { id: 'support', label: 'Поддержка', icon: 'fas fa-headset' }
]

const setActiveTab = (id) => {
  emit('update:activeTab', id)
}
</script>
<style scoped lang="scss" src="@/assets/style/views/profile/main.scss" ></style>


