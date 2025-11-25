<!-- src/views/Profile/components/ProfileForm.vue -->
<template>
  <div class="tab-content">
    <h2 class="section-title">Профиль</h2>
    <div class="profile-info">
      <div class="name-field">
        <label>Полное имя</label>
        <div class="name-input-wrapper">
          <input
            v-model="localName"
            type="text"
            class="form-control name-input"
            :disabled="!isEditing || loading"
            placeholder="Введите ваше имя"
          />
          <button
            type="button"
            class="edit-btn"
            @click="toggleEdit"
            :disabled="loading"
            aria-label="Редактировать имя"
          >
            <i class="fas fa-pencil-alt"></i>
          </button>
        </div>
      </div>

      <div class="form-group">
        <label for="email">Email</label>
        <input
          id="email"
          :value="email"
          type="email"
          class="form-control"
          disabled
        />
      </div>

      <div class="form-group">
        <label for="newPassword">Новый пароль</label>
        <input
          id="newPassword"
          v-model="newPassword"
          type="password"
          class="form-control"
          placeholder="••••••••"
          :disabled="loading"
        />
      </div>

      <div class="profile-actions">
        <button
          v-if="isEditing"
          type="button"
          class="btn btn-secondary"
          @click="cancelEdit"
          :disabled="loading"
        >
          Отмена
        </button>
        <button
          type="button"
          class="btn"
          :disabled="loading || (isEditing && !isNameValid)"
          @click="saveProfile"
        >
          {{ loading ? 'Сохранение...' : isEditing ? 'Сохранить имя' : 'Сохранить изменения' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { api } from '@/services/api' // ✅ ЕДИНЫЙ API-КЛИЕНТ

const props = defineProps({
  initialProfile: {
    type: Object,
    required: true,
    validator: (val) => 'name' in val && 'email' in val
  }
})

const emit = defineEmits(['update'])

const loading = ref(false)
const isEditing = ref(false)
const newPassword = ref('')
const originalName = ref(props.initialProfile.name)
const localName = ref(props.initialProfile.name)
const email = ref(props.initialProfile.email)

const isNameValid = computed(() => localName.value.trim().length >= 2)

const toggleEdit = () => {
  if (isEditing.value) {
    saveProfile()
  } else {
    originalName.value = localName.value
    isEditing.value = true
  }
}

const cancelEdit = () => {
  localName.value = originalName.value
  isEditing.value = false
}

const saveProfile = async () => {
  if (isEditing.value && !isNameValid.value) {
    alert('Имя должно содержать минимум 2 символа')
    return
  }

  loading.value = true
  try {
    
    const updatedUser = await api.profile.update({
      name: localName.value,
      password: newPassword.value || undefined
    })

    emit('update', updatedUser.name)
    newPassword.value = ''
    isEditing.value = false
    alert(` Профиль обновлён!\nПривет, ${updatedUser.name}!`)
  } catch (error) {
    console.error('Ошибка сохранения профиля:', error)
    alert('Не удалось сохранить изменения. Проверьте соединение.')
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  localName.value = props.initialProfile.name
  email.value = props.initialProfile.email
})
</script>

<style scoped lang="scss" src="@/assets/style/views/profile/main.scss"></style>