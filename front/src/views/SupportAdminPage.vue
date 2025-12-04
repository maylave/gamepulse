<template>
  <div class="support-admin-page">
    <Header />

    <main class="container">
      <div class="support-header">
        <h1>Панель поддержки</h1>
        <p>Управляйте обращениями клиентов и отвечайте на вопросы</p>
      </div>

      <div class="support-layout">
        <!-- Список чатов -->
        <div class="chats-list">
          <div
            v-for="chat in chats"
            :key="chat.id"
            :class="['chat-item', { active: chat.id === activeChatId }]"
            @click="setActiveChat(chat.id)"
          >
            <div class="chat-user">{{ chat.clientName }}</div>
            <div class="chat-preview">{{ chat.lastMessage }}</div>
            <div class="chat-time">{{ chat.lastMessageTime }}</div>
          </div>
          <div v-if="loadingChats" class="chat-item">Загрузка чатов...</div>
          <div v-if="errorChats" class="chat-item error">{{ errorChats }}</div>
        </div>

        
        <div class="chat-box" v-if="activeChat">
          <div class="chat-header">
            <h3>Чат с {{ activeChat.clientName }}</h3>
            <span class="status online">● Онлайн</span>
          </div>

          <div class="chat-messages" ref="chatMessages">
            <div
              v-for="(msg, index) in activeChat.messages"
              :key="index"
              :class="['message', msg.sender]"
            >
              <div class="message-text">{{ msg.text }}</div>
              <div class="message-time">{{ msg.time }}</div>
            </div>
            <div v-if="loadingMessages" class="message loading">Загрузка сообщений...</div>
            <div v-if="errorMessages" class="message error">{{ errorMessages }}</div>
          </div>

          <div class="chat-input">
            <input
              v-model="replyMessage"
              @keyup.enter="sendReply"
              type="text"
              placeholder="Напишите ответ..."
              class="input-field"
              :disabled="sending"
            />
            <button
              @click="sendReply"
              class="send-btn"
              :disabled="!replyMessage.trim() || sending"
            >
              <i class="fas fa-paper-plane"></i>
            </button>
          </div>
        </div>

        <!-- Заглушка, если чат не выбран -->
        <div v-else class="chat-placeholder">
          <p>Выберите чат из списка, чтобы начать ответ</p>
        </div>
      </div>
    </main>

    <Footer />
  </div>
</template>

<script setup>
import { ref, nextTick, computed, onMounted } from 'vue'
import Header from '@/components/Header.vue'
import Footer from '@/components/footer.vue' 
import { api } from '@/services/api'

const chats = ref([])
const activeChatId = ref(null)
const replyMessage = ref('')
const chatMessages = ref(null)

const loadingChats = ref(false)
const errorChats = ref(null)
const loadingMessages = ref(false)
const errorMessages = ref(null)
const sending = ref(false)

const activeChat = computed(() => {
  return chats.value.find(chat => chat.id === activeChatId.value)
})

onMounted(async () => {
  await loadChats()
})

async function loadChats() {
  loadingChats.value = true
  errorChats.value = null
  try {
    const data = await api.supportAdmin.getChats()
    chats.value = data || []
    if (chats.value.length > 0 && !activeChatId.value) {
      setActiveChat(chats.value[0].id)
    }
  } catch (err) {
    console.error('Ошибка загрузки чатов:', err)
    errorChats.value = 'Не удалось загрузить чаты'
  } finally {
    loadingChats.value = false
  }
}

async function loadMessages(chatId) {
  loadingMessages.value = true
  errorMessages.value = null
  try {
    const messages = await api.supportAdmin.getMessages(chatId)
    const chat = chats.value.find(c => c.id === chatId)
    if (chat) {
      chat.messages = messages || []
      nextTick(() => scrollToBottom())
    }
  } catch (err) {
    console.error('Ошибка загрузки сообщений:', err)
    errorMessages.value = 'Не удалось загрузить сообщения'
  } finally {
    loadingMessages.value = false
  }
}

function setActiveChat(id) {
  activeChatId.value = id
  loadMessages(id)
}

function formatTime() {
  const now = new Date()
  return now.getHours() + ':' + String(now.getMinutes()).padStart(2, '0')
}

async function sendReply() {
  if (!replyMessage.value.trim() || !activeChatId.value) return

  const text = replyMessage.value.trim()
  const chat = activeChat.value
  if (!chat) return

  
  chat.messages.push({
    sender: 'support',
    text,
    time: formatTime()
  })
  replyMessage.value = ''
  nextTick(() => scrollToBottom())

  sending.value = true
  try {
    await api.supportAdmin.sendReply(activeChatId.value, text)

    // Обновляем lastMessage в списке
    chat.lastMessage = text
    chat.lastMessageTime = formatTime()
  } catch (err) {
    console.error('Ошибка отправки:', err)
    // Можно показать ошибку или откатить сообщение
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

.support-admin-page {
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


.chats-list {
  width: 280px;
  background: var(--color-card);
  border-radius: 16px;
  padding: 1rem;
  max-height: 600px;
  overflow-y: auto;
}

.chat-item {
  padding: 1rem;
  border-radius: 12px;
  margin-bottom: 0.8rem;
  cursor: pointer;
  background: rgba(255, 255, 255, 0.03);
  transition: background 0.2s;
  color: var(--color-text);
}

.chat-item:hover {
  background: rgba(255, 255, 255, 0.06);
}

.chat-item.active {
  background: rgba($color-primary, 0.2);
  border-left: 3px solid $color-primary;
}

.chat-item.error {
  color: #ff6b6b;
  cursor: default;
}

.chat-user {
  font-weight: 600;
  margin-bottom: 0.2rem;
}

.chat-preview {
  font-size: 0.9rem;
  color: var(--color-text-secondary);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.chat-time {
  font-size: 0.75rem;
  color: var(--color-text-tertiary);
  text-align: right;
  margin-top: 0.3rem;
}


.chat-box {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: var(--color-card);
  border-radius: 16px;
  overflow: hidden;
  height: 600px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
}

.chat-header {
  padding: 1rem 1.2rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: rgba(0, 0, 0, 0.15);
}

.chat-header h3 {
  font-size: 1.25rem;
  color: var(--color-text);
}

.status {
  font-size: 0.85rem;
}

.status.online {
  color: #4caf50;
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
  background: #0276b07a;
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

.chat-placeholder {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-card);
  border-radius: 16px;
  color: var(--color-text-secondary);
  font-style: italic;
}


@media (max-width: 900px) {
  .support-layout {
    flex-direction: column;
  }

  .chats-list {
    width: 100%;
    max-height: 200px;
  }

  .chat-box,
  .chat-placeholder {
    height: 400px;
  }
}
</style>