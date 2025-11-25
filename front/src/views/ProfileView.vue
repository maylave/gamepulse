<!-- src/views/Profile/ProfileView.vue -->
<template>
  <div class="app">
    <Header />
    <div class="container">
      <div class="main-layout">
        <ProfileSidebar v-model:active-tab="activeTab" />
        <main class="content">
          <transition name="slide" mode="out-in">
            <ProfileForm
              v-if="activeTab === 'profile'"
              key="profile"
              :initial-profile="profile"
              @update="handleProfileUpdate"
            />
            <OrdersList
              v-else-if="activeTab === 'orders'"
              key="orders"
            />
            <SupportInfo v-else key="support" />
          </transition>
        </main>
      </div>
    </div>
    <Footer />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue'
import ProfileSidebar from '../components/profile/ProfileSidebar.vue'
import ProfileForm from '../components/profile/ProfileForm.vue'
import OrdersList from '../components/profile/OrdersList.vue'
import SupportInfo from '../components/profile/SupportInfo.vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const activeTab = ref('profile')

const profile = ref({
  name: authStore.user?.name || 'Гость',
  email: authStore.user?.email || ''
})
const handleLogout = async () => {
  authStore.logout() 
  await router.push('/') 
}
const handleProfileUpdate = (updatedName) => {
  if (authStore.user) {
    authStore.user.name = updatedName
  }
  profile.value.name = updatedName
}

onMounted(() => {
  profile.value.name = authStore.user?.name || 'Гость'
  profile.value.email = authStore.user?.email || ''
})
</script>

<style lang="scss" scoped src="@/assets/style/views/profile/main.scss"></style>