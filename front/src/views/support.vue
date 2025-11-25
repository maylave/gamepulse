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
          </div>

          <div class="chat-input">
            <input
              v-model="newMessage"
              @keyup.enter="sendMessage"
              type="text"
              placeholder="Напишите сообщение..."
              class="input-field"
            />
            <button @click="sendMessage" class="send-btn" :disabled="!newMessage.trim()">
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
              <a href="mailto:support@pirogiryzani.ru">support@pirogiryzani.ru</a>
            </p>
            <p>
              <i class="fas fa-phone"></i>
              <a href="tel:+78005553535">+7 (800) 555-35-35</a>
            </p>
            <p>
              <i class="fab fa-telegram"></i>
              <a href="https://t.me/pirogiryzani_help" target="_blank">@pirogiryzani_help</a>
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
import { ref, nextTick } from 'vue'
import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue'

const newMessage = ref('')
const messages = ref([
  {
    sender: 'support',
    text: 'Здравствуйте! Чем могу помочь?',
    time: 'Только что'
  }
])

const chatMessages = ref(null)

function formatTime() {
  const now = new Date()
  return now.getHours() + ':' + String(now.getMinutes()).padStart(2, '0')
}

function sendMessage() {
  if (!newMessage.value.trim()) return

  // Сообщение от пользователя
  messages.value.push({
    sender: 'user',
    text: newMessage.value,
    time: formatTime()
  })

  newMessage.value = ''

  // Имитация ответа поддержки (в реальном проекте — запрос к API)
  setTimeout(() => {
    messages.value.push({
      sender: 'support',
      text: 'Спасибо за обращение! Оператор ответит вам в ближайшее время.',
      time: formatTime()
    })
    scrollToBottom()
  }, 1000)

  nextTick(() => {
    scrollToBottom()
  })
}

function scrollToBottom() {
  if (chatMessages.value) {
    chatMessages.value.scrollTop = chatMessages.value.scrollHeight
  }
}
</script>

<style scoped>
.support-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.container {
  width: 90%;
  max-width: 1400px;
  margin: 0 auto;
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
}

.message.support {
  align-self: flex-start;
  background: rgba(255, 255, 255, 0.08);
  border-top-left-radius: 4px;
}

.message.user {
  align-self: flex-end;
  background: linear-gradient(90deg, var(--color-secondary), var(--color-primary));
  color: #000;
  border-top-right-radius: 4px;
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

.send-btn {
  width: 44px;
  height: 44px;
  margin-left: 0.8rem;
  background: #FF6B35;
  color: white;
  border: none;
  border-radius: 12px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.2s;
}

.send-btn:hover:not(:disabled) {
  background: #e05a2a;
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
  color: #FF6B35;
}

.info-card i {
  width: 20px;
  text-align: center;
  color: #FF6B35;
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