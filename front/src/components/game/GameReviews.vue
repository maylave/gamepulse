<template>
  <section class="reviews-section">
    <h2 class="section-title">Отзывы игроков</h2>

    <!-- Список отзывов -->
    <div v-if="loading" class="loading-reviews">
      Загрузка отзывов...
    </div>
    <div v-else-if="reviews.length === 0" class="no-reviews">
      Пока никто не оставил отзыв. Будьте первым!
    </div>
    <div v-else class="reviews-grid">
      <div class="review-card" v-for="review in reviews" :key="review.id">
        <div class="review-header">
          <div class="review-avatar">
            {{ review.authorName ? review.authorName[0].toUpperCase() : '?' }}
          </div>
          <div>
            <div class="review-author">{{ review.authorName || 'Аноним' }}</div>
            <div class="review-rating">
              <i
                v-for="n in 5"
                :key="n"
                :class="n <= review.rating ? 'fas fa-star' : 'far fa-star'"
              ></i>
            </div>
          </div>
        </div>
        <div class="review-text">{{ review.text }}</div>
        <div class="review-date">{{ formatDate(review.createdAt) }}</div>
      </div>
    </div>

    <!-- Форма отзыва -->
    <div v-if="isAuthenticated" class="review-form">
      <h3>Оставить отзыв</h3>
      <form @submit.prevent="submitReview">
        <div class="rating-input">
          <label>Ваша оценка:</label>
          <div class="stars">
            <button
              type="button"
              v-for="n in 5"
              :key="n"
              class="star-btn"
              :class="{ active: newRating >= n }"
              @click="newRating = n"
              :aria-label="`Оценить ${n} звездами`"
            >
              <i class="fas fa-star"></i>
            </button>
          </div>
        </div>

        <div class="text-input">
          <label for="review-text">Ваш отзыв:</label>
          <textarea
            id="review-text"
            v-model="newReviewText"
            placeholder="Расскажите, что вам понравилось..."
            maxlength="500"
            required
          ></textarea>
          <div class="char-count">{{ newReviewText.length }}/500</div>
        </div>

        <button type="submit" class="submit-btn" :disabled="isSubmitting">
          {{ isSubmitting ? 'Отправка...' : 'Опубликовать' }}
        </button>

        <div v-if="submitError" class="error-message">
          {{ submitError }}
        </div>
      </form>
    </div>

    <div v-else class="review-login-prompt">
      <p>Чтобы оставить отзыв, <router-link to="/auth">войдите в аккаунт</router-link>.</p>
    </div>
  </section>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { api } from '@/services/api'

const props = defineProps({
  game: {
    type: Object,
    required: true
  }
})

const authStore = useAuthStore()
const isAuthenticated = computed(() => authStore.isAuthenticated)

// Отзывы
const reviews = ref([])
const loading = ref(false)

// Форма
const newRating = ref(5)
const newReviewText = ref('')
const isSubmitting = ref(false)
const submitError = ref(null)

// Форматирование даты
const formatDate = (dateString) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('ru-RU', {
    day: '2-digit',
    month: 'long',
    year: 'numeric'
  })
}

// Загрузка отзывов
const fetchReviews = async () => {
  loading.value = true
  submitError.value = null
  try {
    const response = await api.reviews.getByGame(props.game.id)
    // Убедимся, что данные — массив
    reviews.value = Array.isArray(response) ? response : response?.data || []
  } catch (err) {
    console.error('Ошибка загрузки отзывов:', err)
    reviews.value = []
  } finally {
    loading.value = false
  }
}

// Отправка отзыва
const submitReview = async () => {
  const text = newReviewText.value.trim()
  if (!text) return

  isSubmitting.value = true
  submitError.value = null

  try {
    const payload = {
      gameId: props.game.id,
      rating: newRating.value,
      text: text
    }

    const response = await api.reviews.create(payload)

    // 💡 ВАЖНО: добавляем полученный отзыв в начало списка
    reviews.value = [response, ...reviews.value]

    // Сброс формы
    newReviewText.value = ''
    newRating.value = 5

    // Успех — можно показать уведомление (опционально)
  } catch (err) {
    console.error('Ошибка отправки отзыва:', err)
    const msg = err?.response?.data?.message ||
                err?.message ||
                'Не удалось отправить отзыв. Попробуйте позже.'
    submitError.value = msg
  } finally {
    isSubmitting.value = false
  }
}

// Загружаем отзывы при монтировании
onMounted(() => {
  fetchReviews()
})

// 🔁 Обновляем отзывы, если ID игры изменилось (например, переход между играми)
watch(() => props.game.id, () => {
  fetchReviews()
})
</script>

<style scoped lang="scss">
@use '@/assets/style/global/_variables' as *;

.reviews-section {
  margin-top: 3rem;
}

.section-title {
  font-size: 2rem;
  margin: 3rem 0 1.8rem;
  position: relative;
  display: inline-block;

  &::after {
    content: '';
    position: absolute;
    bottom: -8px;
    left: 0;
    width: 50px;
    height: 4px;
    background: $color-primary;
    border-radius: 2px;
  }
}

.loading-reviews,
.no-reviews {
  text-align: center;
  padding: 2rem;
  color: $color-text-secondary;
  font-style: italic;
}

.reviews-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 2rem;
  margin-bottom: 2.5rem;
}

.review-card {
  background: $color-card;
  border-radius: 16px;
  padding: 1.5rem;

  .review-header {
    display: flex;
    align-items: center;
    gap: 1rem;
    margin-bottom: 1rem;

    .review-avatar {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      background: linear-gradient(45deg, $color-primary, $color-secondary);
      display: flex;
      align-items: center;
      justify-content: center;
      color: #000;
      font-weight: bold;
      font-size: 1.1rem;
    }

    .review-author {
      font-weight: 600;
      margin-bottom: 0.3rem;
    }

    .review-rating {
      color: #ffd700;
    }
  }

  .review-text {
    color: $color-text-secondary;
    line-height: 1.6;
    margin-bottom: 1rem;
  }

  .review-date {
    color: $color-text-secondary;
    font-size: 0.9rem;
  }
}

.review-form {
  background: $color-card;
  border-radius: 16px;
  padding: 1.8rem;
  margin-top: 2rem;

  h3 {
    margin-bottom: 1.2rem;
    color: $color-text;
    font-size: 1.4rem;
  }

  .rating-input {
    margin-bottom: 1.2rem;

    label {
      display: block;
      margin-bottom: 0.6rem;
      color: $color-text;
    }

    .stars {
      display: flex;
      gap: 0.4rem;
    }

    .star-btn {
      background: none;
      border: none;
      color: #444;
      font-size: 1.4rem;
      cursor: pointer;
      transition: transform 0.2s ease;

      &.active {
        color: #ffd700;
      }

      &:hover {
        transform: scale(1.2);
      }
    }
  }

  .text-input {
    margin-bottom: 1.2rem;

    label {
      display: block;
      margin-bottom: 0.6rem;
      color: $color-text;
    }

    textarea {
      width: 100%;
      min-height: 120px;
      padding: 0.8rem;
      border-radius: 12px;
      border: 1px solid $color-text-secondary;
      background: rgba(255, 255, 255, 0.03);
      color: $color-text;
      font-family: $font-main;
      resize: vertical;

      &:focus {
        outline: 2px solid $color-primary;
        border-color: $color-primary;
      }
    }

    .char-count {
      text-align: right;
      font-size: 0.85rem;
      color: $color-text-secondary;
      margin-top: 0.4rem;
    }
  }

  .submit-btn {
    background: linear-gradient(90deg, $color-secondary, $color-primary);
    color: #000;
    border: none;
    padding: 0.8rem 1.8rem;
    border-radius: 12px;
    font-weight: 600;
    font-size: 1rem;
    cursor: pointer;
    transition: opacity 0.2s ease;

    &:hover:not(:disabled) {
      opacity: 0.9;
    }

    &:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
  }

  .error-message {
    color: #ff6b6b;
    margin-top: 0.8rem;
    font-size: 0.95rem;
  }
}

.review-login-prompt {
  margin-top: 2rem;
  text-align: center;
  color: $color-text-secondary;

  a {
    color: $color-primary;
    text-decoration: none;
    &:hover {
      text-decoration: underline;
    }
  }
}
</style>