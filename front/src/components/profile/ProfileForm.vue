<template>
  <div class="tab-content">
    <h2 class="section-title">Профиль</h2>

    <!-- Аватар -->
    <div class="avatar-section">
      <div class="avatar-preview">
        <img
          :src="previewAvatarUrl"
          :alt="localName || 'Аватар'"
          @error="handleAvatarError"
        />
      </div>
      <div class="avatar-controls">
        <input
          v-model="localAvatarUrl"
          type="url"
          class="form-control avatar-input"
          placeholder="https://example.com/avatar.jpg"
          :disabled="loading"
        />
        <!-- КНОПКА ЗАГРУЗКИ ФАЙЛА -->
        <label class="btn btn-secondary avatar-upload">
          Загрузить
          <input
            type="file"
            accept="image/*"
            @change="handleFileUpload"
            class="visually-hidden"
          />
        </label>
        <button
          type="button"
          class="btn btn-secondary avatar-reset"
          @click="resetAvatar"
          :disabled="loading"
        >
          Сбросить
        </button>
      </div>
    </div>

    <!-- Остальное содержимое профиля -->
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
import { api } from '@/services/api'
import { useAuthStore } from '@/stores/auth'

const DEFAULT_AVATAR = '/images/defaults/avatar.png'

// Глобальное хранилище (для обновления токена/ролей, если нужно)
const authStore = useAuthStore()

const loading = ref(false)
const isEditing = ref(false)
const newPassword = ref('')
const localName = ref('')
const email = ref('')
const localAvatarUrl = ref(DEFAULT_AVATAR)

const previewAvatarUrl = computed(() => {
  return localAvatarUrl.value || DEFAULT_AVATAR
})

const isNameValid = computed(() => localName.value.trim().length >= 2)

// 🔑 ОСНОВНОЙ МЕТОД: получение профиля с сервера
const getProfile = async () => {
  try {
    const profile = await api.profile.get()
    localName.value = profile.name
    email.value = profile.email
    localAvatarUrl.value = profile.avatarUrl || DEFAULT_AVATAR

    // Опционально: обновить глобальное состояние (например, роли)
    authStore.updateUser({
      id: profile.id,
      name: profile.name,
      email: profile.email,
      avatarUrl: profile.avatarUrl || null,
      roles: profile.roles
    })
  } catch (error) {
    console.error('Ошибка загрузки профиля:', error)
    alert('Не удалось загрузить данные профиля.')
  }
}

const toggleEdit = () => {
  if (isEditing.value) {
    saveProfile()
  } else {
    isEditing.value = true
  }
}

const cancelEdit = () => {
  getProfile() // возвращаемся к серверным данным
  isEditing.value = false
}

const resetAvatar = () => {
  localAvatarUrl.value = DEFAULT_AVATAR
}

const handleAvatarError = () => {
  localAvatarUrl.value = DEFAULT_AVATAR
}

const handleFileUpload = async (event) => {
  const file = event.target.files?.[0]
  if (!file) return

  if (!file.type.startsWith('image/')) {
    alert('Пожалуйста, выберите изображение (JPEG, PNG и т.д.)')
    return
  }

  if (file.size > 5 * 1024 * 1024) {
    alert('Файл слишком большой. Максимум — 5 МБ.')
    return
  }

  loading.value = true
  try {
    const formData = new FormData()
    formData.append('avatar', file)

    await api.profile.uploadAvatar(formData)

    // 🔁 Загружаем актуальные данные с сервера
    await getProfile()
  } catch (error) {
    console.error('Ошибка загрузки аватара:', error)
    alert('Не удалось загрузить аватар. Попробуйте позже.')
  } finally {
    loading.value = false
    event.target.value = ''
  }
}

const saveProfile = async () => {
  if (isEditing.value && !isNameValid.value) {
    alert('Имя должно содержать минимум 2 символа')
    return
  }

  loading.value = true
  try {
    const payload = {
      name: localName.value.trim(),
      password: newPassword.value || undefined,
      avatarUrl: localAvatarUrl.value === DEFAULT_AVATAR ? null : localAvatarUrl.value
    }

    await api.profile.update(payload)

 
    await getProfile()

    alert(`Профиль обновлён!\nПривет, ${localName.value}!`)
  } catch (error) {
    console.error('Ошибка сохранения профиля:', error)
    alert('Не удалось сохранить изменения. Проверьте соединение.')
  } finally {
    loading.value = false
    isEditing.value = false
  }
}

// Загружаем профиль при монтировании
onMounted(() => {
  getProfile()
})
</script>

<style scoped lang="scss">
@use '@/assets/style/global/_variables' as *;

/* Только минимальные стили для нового элемента — без цветов и переписывания твоих */
.avatar-upload {
  position: relative;
  overflow: hidden;
  cursor: pointer;
  display: inline-block;
}

.visually-hidden {
  position: absolute !important;
  width: 1px !important;
  height: 1px !important;
  padding: 0 !important;
  margin: -1px !important;
  overflow: hidden !important;
  clip: rect(0, 0, 0, 0) !important;
  white-space: nowrap !important;
  border: 0 !important;
}
</style>

<style scoped lang="scss" src="@/assets/style/views/profile/main.scss"></style>

<style scoped lang="scss">
@use '@/assets/style/global/_variables' as *;

.avatar-section {
  display: flex;
  gap: 1.5rem;
  margin-bottom: 2rem;
  align-items: flex-start;
}

.avatar-preview {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  overflow: hidden;
  border: 2px solid $color-border;

  img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }
}

.avatar-controls {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;

  .avatar-input {
    flex: 1;
  }

  .avatar-reset {
    padding: 0.5rem;
    font-size: 0.9rem;
  }
}

@media (max-width: 768px) {
  .avatar-section {
    flex-direction: column;
  }
}
</style>