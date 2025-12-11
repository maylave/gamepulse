<template>
  <Header />
  <div class="admin-page">
    <main class="admin-content">
      <div class="admin-header">
        <h1>Добавить новую игру</h1>
        <!-- Кнопка удалена — модалка открывается автоматически -->
      </div>

      <!-- Модалка всегда открыта при заходе на страницу -->
      <AdminModal
        :item="null"
        :fields="gameFields"
        title="Добавить игру"
        @save="saveGame"
        @close="closeModal"
        :saving="saving"
      />
    </main>
  </div>
  <Footer />
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { useSuperUserStore } from '@/stores/superUser'
import { useRouter } from 'vue-router' // ← для перехода после закрытия
import AdminModal from '@/components/admin/AdminModal.vue'
import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue'

const store = useSuperUserStore()
const router = useRouter()
const saving = ref(false)
const allGenres = ref([])

const TAGS = [
  { value: 'Новинка', label: 'Новинка' },
  { value: 'Бесплатно', label: 'Бесплатно' },
  { value: 'Скоро', label: 'Скоро' },
  { value: 'Хит', label: 'Хит' },
  { value: 'top', label: 'top' },
  { value: 'free', label: 'free' },
  { value: '', label: 'Без тега' }
]
const isModalOpen = ref(true)

onMounted(async () => {
  try {
    await store.fetchGenres()
    allGenres.value = store.genres
  } catch (err) {
    console.error('Не удалось загрузить жанры:', err)
  }
})

const gameFields = ref([
  
      
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
  
])

watch(
  () => allGenres.value,
  (genres) => {
    const genreField = gameFields.value.find(f => f.key === 'genreIds')
    if (genreField) {
      genreField.options = genres.map(g => ({ label: g.name, value: g.id }))
    }
  },
  { immediate: true }
)

function closeModal() {
   
  router.back()
}

async function saveGame(data) {
  if (data.oldPrice === '' || data.oldPrice === undefined) {
    data.oldPrice = null
  }

  saving.value = true
  try {
    await store.createGame(data)
    alert('Игра успешно добавлена!')
    closeModal()
  } catch (err) {
    alert('Ошибка: ' + (err.message || 'Не удалось создать игру'))
  } finally {
    saving.value = false
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
  margin-bottom: 2rem;

  h1 {
    font-size: 1.6rem;
    color: var(--color-text);
    margin: 0;
  }
}
</style>