<template>
  <div class="activation-page">
    <Header />

    <main class="activation-main">
      <div class="container">
        <div class="activation-card">
          <div class="icon">
            <i class="fas fa-check-circle"></i>
          </div>

          <h1>Заказ оформлен!</h1>
          <p class="subtitle">
            Ключ активации игры отправлен на ваш email:<br />
            <strong>{{ authStore.user?.email}}</strong>
          </p>

          <div class="info-box">
            <p>Проверьте папку «Спам», если письмо не пришло.</p>
            <p>Ключ действителен в течение 30 дней.</p>
          </div>

          <div class="timer">
            <p>Вы будете перенаправлены в каталог через {{ countdown }} сек.</p>
          </div>

          <button class="back-btn" @click="goToCatalog">
            Перейти в каталог сейчас
          </button>
        </div>
      </div>
    </main>

    <Footer />
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue'
import { useAuthStore } from '@/stores/auth'
const router = useRouter()


const authStore = useAuthStore()


const countdown = ref(10)
let timer = null

const goToCatalog = () => {
  router.push('/catalog')
}

const startCountdown = () => {
  timer = setInterval(() => {
    if (countdown.value > 0) {
      countdown.value--
    } else {
      goToCatalog()
    }
  }, 1000)
}

onMounted(() => {
  startCountdown()
})

onUnmounted(() => {
  if (timer) clearInterval(timer)
})
</script>

<style scoped>
.activation-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background: var(--color-bg);
  color: var(--color-text);
}

.activation-main {
  flex: 1;
  display: flex;
  align-items: center;
  padding: 2rem 0;
}

.container {
  max-width: 600px;
  margin: 0 auto;
  padding: 0 1.5rem;
}

.activation-card {
  background: var(--color-card);
  border-radius: 20px;
  padding: 2.5rem;
  text-align: center;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
}

.icon {
  font-size: 4rem;
  color: #4ade80;
  margin-bottom: 1.2rem;
}

.icon i {
  display: inline-block;
  filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.2));
}

h1 {
  font-size: 1.8rem;
  font-weight: 700;
  margin: 0 0 1rem;
  color: var(--color-primary);
}

.subtitle {
  font-size: 1.1rem;
  line-height: 1.5;
  color: var(--color-text);
  margin-bottom: 1.5rem;
}

.subtitle strong {
  color: var(--color-primary);
  word-break: break-all;
}

.info-box {
  background: rgba(255, 255, 255, 0.08);
  border-left: 3px solid var(--color-primary);
  padding: 1rem;
  margin: 1.5rem 0;
  text-align: left;
  border-radius: 0 8px 8px 0;
}

.info-box p {
  margin: 0.4rem 0;
  font-size: 0.95rem;
  color: var(--color-text-secondary);
}

.timer {
  margin: 1.5rem 0;
  color: var(--color-text-secondary);
  font-size: 0.95rem;
}

.back-btn {
  width: 100%;
  max-width: 300px;
  padding: 0.9rem;
  background: linear-gradient(90deg, var(--color-secondary), var(--color-primary));
  color: #000;
  border: none;
  border-radius: 12px;
  font-weight: 700;
  font-size: 1rem;
  cursor: pointer;
  transition: opacity 0.2s;
  margin: 0 auto;
}

.back-btn:hover {
  opacity: 0.9;
}


@media (max-width: 480px) {
  .activation-card {
    padding: 1.8rem;
  }

  h1 {
    font-size: 1.5rem;
  }

  .subtitle {
    font-size: 1rem;
  }

  .icon {
    font-size: 3rem;
  }
}
</style>