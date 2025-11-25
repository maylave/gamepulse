<!-- src/views/AdminView.vue -->
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

const store = useAdminStore()
const activeEntity = ref('users')
const isModalOpen = ref(false)
const editingItem = ref(null)
const saving = ref(false)
const loading = computed(() => store.loading)

// Загружаем жанры один раз для использования в multiselect
const allGenres = ref([])

onMounted(async () => {
  try {
    await store.fetchGenres() // Убедись, что в store есть genres
    allGenres.value = store.genres
  } catch (err) {
    console.error('Не удалось загрузить жанры:', err)
  }
})

// Динамический конфиг — используем функцию или computed
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
            options: ['User', 'Admin', 'Moderator']
          }
        ],
        fetch: () => store.fetchUsers(),
        create: (data) => store.createUser(data),
        update: (id, data) => store.updateUser(id, data),
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
          {
            key: 'genreIds',
            label: 'Жанры'
          }
        ],
        fields: [
          { key: 'title', label: 'Название', type: 'text', required: true },
          { key: 'price', label: 'Цена', type: 'number', step: '0.01', required: true },
          { key: 'description', label: 'Описание', type: 'textarea' },
          { key: 'oldPrice', label: 'Старая цена', type: 'number', step: '0.01' },
          { key: 'tag', label: 'Тег', type: 'text' },
          { key: 'category', label: 'Категория', type: 'text' },
          { key: 'developer', label: 'Разработчик', type: 'text' },
          { key: 'publisher', label: 'Издатель', type: 'text' },
          { key: 'ageRating', label: 'Возрастной рейтинг', type: 'number' },
          { key: 'isPreorder', label: 'Предзаказ', type: 'checkbox' },
          { key: 'imageUrl', label: 'URL изображения', type: 'text' },
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
          }
        ],
        fetch: () => store.fetchGames(),
        create: (data) => store.createGame(data),
        update: (id, data) => store.updateGame(id, data),
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

    // Форматируем дату для отображения
    if (activeEntity.value === 'games' && item.releaseDate) {
      newItem.releaseDate = new Date(item.releaseDate).toLocaleDateString('ru-RU')
    }

    // Форматируем жанры как названия
    if (activeEntity.value === 'games' && Array.isArray(item.genreIds)) {
      const genreNames = item.genreIds
        .map(id => allGenres.value.find(g => g.id === id)?.name)
        .filter(Boolean)
        .join(', ')
      newItem.genreIds = genreNames || '—'
    }

    // Форматируем роли у пользователей
    if (activeEntity.value === 'users' && Array.isArray(item.roles)) {
      newItem.roles = item.roles.join(', ')
    }

    return newItem
  })
})

watch(activeEntity, () => {
  entityConfig.value?.fetch()
}, { immediate: true })

function openModal(item) {
  editingItem.value = item ? { ...item } : null
  isModalOpen.value = true
}

function closeModal() {
  isModalOpen.value = false
  editingItem.value = null
}

async function saveItem(data) {
  // Для даты: преобразуем в ISO-строку, если это Date
  if (data.releaseDate && typeof data.releaseDate === 'string') {
    // Если компонент возвращает строку 'YYYY-MM-DD', оставляем как есть
    // Иначе можно: new Date(data.releaseDate).toISOString().split('T')[0]
  }

  saving.value = true
  try {
    if (editingItem.value) {
      await entityConfig.value.update(editingItem.value.id, data)
    } else {
      await entityConfig.value.create(data)
    }
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