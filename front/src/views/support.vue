<template>
  <div class="support-page">
    <Header />

    <main class="container">
      <div class="support-header">
        <h1>Поддержка</h1>
        <p>Наша команда всегда готова помочь вам с любыми вопросами</p>
      </div>

      <div class="support-layout">
        <!-- Чат -->
        <div class="chat-box">
          <div class="chat-messages" ref="chatMessages">
            <div
              v-for="(msg, index) in messages"
              :key="index"
              :class="['message', msg.sender]"
            >
              <div class="message-text">{{ msg.text }}</div>
              <div class="message-time">{{ msg.time }}</div>
            </div>
            <div v-if="loading" class="message loading">
              Загрузка истории чата...
            </div>
            <div v-if="error" class="message error">
              {{ error }}
            </div>
          </div>

          <div class="chat-input">
            <input
              v-model="newMessage"
              @keyup.enter="sendMessage"
              type="text"
              placeholder="Напишите сообщение..."
              class="input-field"
              :disabled="sending"
            />
            <button
              @click="sendMessage"
              class="send-btn"
              :disabled="!newMessage.trim() || sending"
            >
              <i class="fas fa-paper-plane"></i>
            </button>
          </div>
        </div>

        <!-- Информация и контакты -->
        <div class="support-info">
          <div class="info-card">
            <h3>Как мы можем помочь?</h3>
            <ul>
              <li>Помощь с заказом</li>
              <li>Возврат и обмен</li>
              <li>Технические вопросы</li>
              <li>Предложения и жалобы</li>
            </ul>
          </div>

          <div class="info-card">
            <h3>Другие способы связи</h3>
            <p>
              <i class="fas fa-envelope"></i>
              <a href="mailto:support@gamepulse.ru">support@gamepulse.ru</a>
            </p>
            <p>
              <i class="fas fa-phone"></i>
              <a href="tel:+78005553535">+7 (800) 555-35-35</a>
            </p>
            <p>
              <i class="fab fa-telegram"></i>
              <a href="https://t.me/gamepulse_help" target="_blank">@gamepulse_help</a>
            </p>
          </div>

          <div class="info-card hours">
            <h3>Время работы поддержки</h3>
            <p>Пн–Вс: 9:00 – 22:00 (МСК)</p>
          </div>
        </div>
      </div>
    </main>

    <Footer />
  </div>
</template>

<script setup>
import { ref, nextTick, onMounted } from 'vue'
import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue' // Убедитесь, что имя файла footer.vue — lowercase!
import { api } from '@/services/api'

const newMessage = ref('')
const messages = ref([])
const chatMessages = ref(null)
const loading = ref(false)
const sending = ref(false)
const error = ref(null)

onMounted(async () => {
  await loadChat()
})

async function loadChat() {
  loading.value = true
  error.value = null
  try {
    const chat = await api.support.getOrCreateChat()
    messages.value = chat.messages || []
    nextTick(() => scrollToBottom())
  } catch (err) {
    console.error('Ошибка загрузки чата:', err)
    error.value = 'Не удалось загрузить чат. Попробуйте позже.'
  } finally {
    loading.value = false
  }
}

function formatTime(date) {
  return date.getHours() + ':' + String(date.getMinutes()).padStart(2, '0')
}

async function sendMessage() {
  const text = newMessage.value.trim()
  if (!text) return

  // Добавляем локально для мгновенного отклика
  const now = new Date()
  messages.value.push({
    sender: 'user',
    text,
    time: formatTime(now)
  })
  newMessage.value = ''
  nextTick(() => scrollToBottom())

  sending.value = true
  try {
    await api.support.sendMessage(text)
    // После отправки — можно обновить чат, чтобы увидеть возможный ответ
    // await loadChat() // опционально: раскомментируйте, если хотите обновлять сразу
  } catch (err) {
    console.error('Ошибка отправки:', err)
    error.value = 'Не удалось отправить сообщение. Попробуйте ещё раз.'
    // Можно удалить последнее сообщение или оставить — как UX-решение
  } finally {
    sending.value = false
  }
}

function scrollToBottom() {
  if (chatMessages.value) {
    chatMessages.value.scrollTop = chatMessages.value.scrollHeight
  }
}
</script>

<style scoped lang="scss">
@use '@/assets/style/global/_variables' as *;

.support-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.container {
  padding: 2rem 0;
}

.support-header {
  text-align: center;
  margin-bottom: 2.5rem;
}

.support-header h1 {
  font-size: 2.5rem;
  margin-bottom: 0.8rem;
  color: var(--color-text);
}

.support-header p {
  font-size: 1.1rem;
  color: var(--color-text-secondary);
}

.support-layout {
  display: flex;
  gap: 2rem;
}

/* Чат */
.chat-box {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: var(--color-card);
  border-radius: 16px;
  overflow: hidden;
  height: 500px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
}

.chat-messages {
  flex: 1;
  padding: 1.2rem;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.message {
  max-width: 80%;
  padding: 0.8rem 1rem;
  border-radius: 12px;
  position: relative;
  line-height: 1.4;
  word-break: break-word;
}

.message.support {
  align-self: flex-start;
  background: rgba(255, 255, 255, 0.08);
  border-top-left-radius: 4px;
}

.message.user {
  align-self: flex-end;
  background: #0276b07a; // ваш цвет
  color: #fff;
  border-top-right-radius: 4px;
}

.message.loading,
.message.error {
  align-self: center;
  background: none;
  color: var(--color-text-secondary);
  font-style: italic;
  max-width: 100%;
}

.message-time {
  font-size: 0.75rem;
  opacity: 0.7;
  margin-top: 0.3rem;
  text-align: right;
}

.chat-input {
  display: flex;
  padding: 1rem;
  background: rgba(0, 0, 0, 0.2);
}

.input-field {
  flex: 1;
  padding: 0.8rem 1rem;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid #333;
  border-radius: 12px;
  color: white;
  font-family: var(--font-main);
}

.input-field:focus {
  outline: none;
  border-color: var(--color-primary);
}

.input-field:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.send-btn {
  width: 44px;
  height: 44px;
  margin-left: 0.8rem;
  color: rgb(82, 80, 178);
  border: none;
  border-radius: 12px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: color 0.2s;
  background: transparent;
}

.send-btn:hover:not(:disabled) {
  color: $color-primary;
}

.send-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Информация */
.support-info {
  width: 300px;
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.info-card {
  background: var(--color-card);
  padding: 1.5rem;
  border-radius: 16px;
  color: var(--color-text);
}

.info-card h3 {
  margin-bottom: 1rem;
  font-size: 1.2rem;
}

.info-card ul {
  padding-left: 1.2rem;
  list-style-type: circle;
  color: var(--color-text-secondary);
}

.info-card p,
.info-card a {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  margin: 0.6rem 0;
  color: var(--color-text-secondary);
  text-decoration: none;
}

.info-card a:hover {
  color: $color-primary;
}

.info-card i {
  width: 20px;
  text-align: center;
  color: $color-primary;
}

@media (max-width: 900px) {
  .support-layout {
    flex-direction: column;
  }

  .support-info {
    width: 100%;
  }

  .chat-box {
    height: 450px;
  }
}
</style>