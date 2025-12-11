<template>
  <Header />
  <div class="admin-page">
    <AdminSidebar v-model:active="activeEntity" />

    <main class="admin-content">
      <div class="admin-header">
        <h1>{{ entityConfig.title }}</h1>
        <UButton @click="openModal(null)" class="btn-add">+ Добавить</UButton>
      </div>

      <AdminTable
        :headers="entityConfig.headers"
        :rows="formattedItems"
        @edit="openModal"
        @delete="handleDelete"
        :loading="loading"
      />

      <AdminModal
        v-if="isModalOpen"
        :item="editingItem"
        :fields="entityConfig.fields"
        :title="editingItem ? 'Редактировать' : 'Добавить'"
        @save="saveItem"
        @close="closeModal"
        :saving="saving"
      />
    </main>
  </div>
  <Footer />
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useAdminStore } from '@/stores/admin'
import AdminSidebar from '@/components/admin/AdminSidebar.vue'
import AdminTable from '@/components/admin/AdminTable.vue'
import AdminModal from '@/components/admin/AdminModal.vue'
import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue'

// === КОНСТАНТЫ: категории и теги ===
const CATEGORIES = [
  { value: 'action', label: 'Экшен' },
  { value: 'rpg', label: 'RPG' },
  { value: 'strategy', label: 'Стратегия' },
  { value: 'adventure', label: 'Приключения' },
  { value: 'simulation', label: 'Симуляторы' },
  { value: 'sports', label: 'Спорт' },
  { value: 'other', label: 'Другое' }
]

const TAGS = [
  { value: 'Новинка', label: 'Новинка' },
  { value: 'Бесплатно', label: 'Бесплатно' },
  { value: 'Скоро', label: 'Скоро' },
  { value: 'Хит', label: 'Хит' },
  { value: '', label: 'Без тега' }
]

const store = useAdminStore()
const activeEntity = ref('users')
const isModalOpen = ref(false)
const editingItem = ref(null)
const saving = ref(false)
const loading = computed(() => store.loading)

const allGenres = ref([])

onMounted(async () => {
  try {
    await store.fetchGenres()
    allGenres.value = store.genres
  } catch (err) {
    console.error('Не удалось загрузить жанры:', err)
  }
})

const refreshGenresIfNeeded = async () => {
  if (activeEntity.value === 'games') {
    await store.fetchGenres()
    allGenres.value = store.genres
  }
}

const getEntityConfig = (entity) => {
  switch (entity) {
    case 'users':
      return {
        title: 'Пользователи',
        headers: [
          { key: 'id', label: 'ID' },
          { key: 'name', label: 'Имя' },
          { key: 'email', label: 'Email' },
          { key: 'roles', label: 'Роли' }
        ],
        fields: [
          { key: 'name', label: 'Имя', type: 'text' },
          { key: 'email', label: 'Email', type: 'email' },
          {
            key: 'roles',
            label: 'Роли',
            type: 'multiselect',
            options: ['User', 'Admin', 'Moderator', 'SuperUser', 'Support']
          }
        ],
        fetch: () => store.fetchUsers(),
        create: (data) => store.createUser(data),
        updateRoles: (id, roles) => store.updateUserRoles(id, roles),
        delete: (id) => store.deleteUser(id)
      }

    case 'games':
      return {
        title: 'Игры',
        headers: [
          { key: 'id', label: 'ID' },
          { key: 'title', label: 'Название' },
          { key: 'price', label: 'Цена' },
          { key: 'category', label: 'Категория' },
          { key: 'developer', label: 'Разработчик' },
          { key: 'releaseDate', label: 'Дата релиза' },
          { key: 'genreNames', label: 'Жанры' }
        ],
        fields: [
          { key: 'title', label: 'Название', type: 'text', required: true },
          { key: 'price', label: 'Цена', type: 'number', step: '0.01', required: true },
          { key: 'description', label: 'Описание', type: 'textarea' },
          { key: 'oldPrice', label: 'Старая цена', type: 'number', step: '0.01' },
          {
            key: 'tag',
            label: 'Тег',
            type: 'select',
            options: TAGS
          },
          {
            key: 'category',
            label: 'Категория',
            type: 'select',
            options: TAGS
          },
          { key: 'developer', label: 'Разработчик', type: 'text' },
          { key: 'publisher', label: 'Издатель', type: 'text' },
          { key: 'ageRating', label: 'Возрастной рейтинг', type: 'number' },
          { key: 'isPreorder', label: 'Предзаказ', type: 'checkbox' },
          { key: 'imageUrl', label: 'Главное изображение (URL)', type: 'text' },
          {
            key: 'externalUrl',
            label: 'Внешняя ссылка (Steam, Epic и т.д.)',
            type: 'text'
          },
          {
            key: 'releaseDate',
            label: 'Дата релиза',
            type: 'date'
          },
          {
            key: 'genreIds',
            label: 'Жанры',
            type: 'multiselect',
            options: allGenres.value.map(g => ({ label: g.name, value: g.id }))
          },
          {
            key: 'mediaUrls',
            label: 'Дополнительные медиа (URL, по одному на строку)',
            type: 'textarea',
            placeholder: 'https://example.com/screen1.jpg\nhttps://example.com/video.mp4'
          }
        ],
        fetch: () => store.fetchGames(),
        create: (data) => {
          const payload = { ...data }
          if (payload.mediaUrls) {
            payload.media = payload.mediaUrls
              .split('\n')
              .map(url => url.trim())
              .filter(url => url)
              .map(url => ({
                url,
                type: /\.(mp4|webm|mov|avi)$/i.test(url) ? 'video' : 'image'
              }))
            delete payload.mediaUrls
          }
          if (payload.genreIds) {
            payload.genreIds = payload.genreIds.map(id => parseInt(id))
          }
          return store.createGame(payload)
        },
        update: (id, data) => {
          const payload = { ...data }
          if (payload.mediaUrls) {
            payload.media = payload.mediaUrls
              .split('\n')
              .map(url => url.trim())
              .filter(url => url)
              .map(url => ({
                url,
                type: /\.(mp4|webm|mov|avi)$/i.test(url) ? 'video' : 'image'
              }))
            delete payload.mediaUrls
          }
          if (payload.genreIds) {
            payload.genreIds = payload.genreIds.map(id => parseInt(id))
          }
          return store.updateGame(id, payload)
        },
        delete: (id) => store.deleteGame(id)
      }

    case 'genres':
      return {
        title: 'Жанры',
        headers: [
          { key: 'id', label: 'ID' },
          { key: 'name', label: 'Название' }
        ],
        fields: [
          { key: 'name', label: 'Название жанра', type: 'text', required: true }
        ],
        fetch: () => store.fetchGenres(),
        create: (data) => store.createGenre(data),
        update: (id, data) => store.updateGenre(id, data),
        delete: (id) => store.deleteGenre(id)
      }

    default:
      return null
  }
}

const entityConfig = computed(() => getEntityConfig(activeEntity.value))
const items = computed(() => store[activeEntity.value] || [])

const formattedItems = computed(() => {
  return items.value.map(item => {
    let newItem = { ...item }

    // Показываем понятные названия категорий
    if (activeEntity.value === 'games' && item.category) {
      const cat = CATEGORIES.find(c => c.value === item.category)
      newItem.category = cat ? cat.label : item.category
    }

    if (activeEntity.value === 'games' && item.releaseDate) {
      newItem.releaseDate = new Date(item.releaseDate).toLocaleDateString('ru-RU')
    }

    if (activeEntity.value === 'games' && Array.isArray(item.genreIds)) {
      const genreNames = item.genreIds
        .map(id => allGenres.value.find(g => g.id === id)?.name)
        .filter(Boolean)
        .join(', ')
      newItem.genreNames = genreNames || '—'
    }

    if (activeEntity.value === 'users' && Array.isArray(item.roles)) {
      newItem.roles = item.roles.join(', ')
    }

    return newItem
  })
})

watch(activeEntity, async () => {
  await refreshGenresIfNeeded()
  await entityConfig.value?.fetch()
}, { immediate: true })

function openModal(item) {
  if (item && activeEntity.value === 'games') {
    const mediaUrls = item.media?.map(m => m.url).join('\n') || ''
    editingItem.value = { ...item, mediaUrls }
  } else {
    editingItem.value = item ? { ...item } : null
  }
  isModalOpen.value = true
}

function closeModal() {
  isModalOpen.value = false
  editingItem.value = null
}

async function saveItem(data) {
  saving.value = true
  try {
    if (editingItem.value) {
      if (activeEntity.value === 'users') {
        await entityConfig.value.updateRoles(editingItem.value.id, data.roles)
      } else {
        await entityConfig.value.update(editingItem.value.id, data)
      }
    } else {
      await entityConfig.value.create(data)
    }

    await refreshGenresIfNeeded()
    await entityConfig.value.fetch()
    closeModal()
  } catch (err) {
    alert('Ошибка сохранения: ' + (err.message || err))
  } finally {
    saving.value = false
  }
}

async function handleDelete(id) {
  if (confirm('Удалить запись?')) {
    await entityConfig.value.delete(id)
    await refreshGenresIfNeeded()
    await entityConfig.value.fetch()
  }
}
</script>

<style lang="scss" scoped>
@use '@/assets/style/global/_variables' as *;

.admin-page {
  display: flex;
  min-height: 100vh;
}

.admin-content {
  flex: 1;
  padding: 2rem;
}

.admin-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;

  h1 {
    font-size: 1.5rem;
    color: var(--color-text);
    margin: 0;
  }
}

:deep(.btn-add) {
  background: none;
  border: 2px solid #333;
  color: white;
  box-shadow: 0 0 15px rgba(10, 20, 30, 0.4);
  font-size: 0.9em !important;
  border-radius: 10px !important;
  padding: 10px 18px !important;
  font-weight: 600;
  transition: all 0.2s ease;

  &:hover {
    border-color: $color-primary;
    box-shadow: 0 0 20px rgba($color-primary, 0.3);
  }
}
</style>