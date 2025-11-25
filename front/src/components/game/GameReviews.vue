<!-- src/components/game/GameReviews.vue -->
<template>
  <section class="reviews-section">
    <h2 class="section-title">Отзывы игроков</h2>
    
    <!-- Список отзывов -->
    <div class="reviews-grid">
      <div class="review-card" v-for="review in game.reviews" :key="review.id">
        <div class="review-header">
          <div class="review-avatar">{{ review.author[0] }}</div>
          <div>
            <div class="review-author">{{ review.author }}</div>
            <div class="review-rating">
              <i v-for="n in 5" :key="n" :class="n <= review.rating ? 'fas fa-star' : 'far fa-star'"></i>
            </div>
          </div>
        </div>
        <div class="review-text">{{ review.text }}</div>
        <div class="review-date">{{ formatDate(review.date) }}</div>
      </div>
    </div>

    <!-- Форма добавления отзыва -->
    <div v-if="isAuthenticated" class="review-form">
      <h3>Оставить отзыв</h3>
      <form @submit.prevent="submitReview">
        <!-- Выбор рейтинга -->
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

        <!-- Текст отзыва -->
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

        <!-- Кнопка -->
        <button type="submit" class="submit-btn" :disabled="isSubmitting">
          {{ isSubmitting ? 'Отправка...' : 'Опубликовать' }}
        </button>
      </form>
    </div>

    <div v-else class="review-login-prompt">
      <p>Чтобы оставить отзыв, <router-link to="/auth">войдите в аккаунт</router-link>.</p>
    </div>
  </section>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'

const props = defineProps({
  game: {
    type: Object,
    required: true
  }
})

const authStore = useAuthStore()
const isAuthenticated = computed(() => authStore.isAuthenticated)

// Данные формы
const newRating = ref(5)
const newReviewText = ref('')
const isSubmitting = ref(false)

const formatDate = (dateString) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('ru-RU', {
    day: '2-digit',
    month: 'long',
    year: 'numeric'
  })
}

// Отправка отзыва (временно в мок-данные)
const submitReview = async () => {
  if (!newReviewText.value.trim()) return

  isSubmitting.value = true

  try {
    // ⚠️ Пока добавляем в мок-данные (в будущем — API)
    const newReview = {
      id: Date.now(),
      author: authStore.user?.name || 'Пользователь',
      rating: newRating.value,
      text: newReviewText.value.trim(),
      date: new Date().toISOString().split('T')[0]
    }

    // Добавляем отзыв в игру (локально)
    props.game.reviews = [...props.game.reviews, newReview]

    // Сброс формы
    newReviewText.value = ''
    newRating.value = 5

    // Уведомление
    alert('✅ Отзыв успешно добавлен!')
  } catch (err) {
    console.error('Ошибка при добавлении отзыва:', err)
    alert('❌ Не удалось добавить отзыв. Попробуйте позже.')
  } finally {
    isSubmitting.value = false
  }
}
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

// Форма отзыва
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