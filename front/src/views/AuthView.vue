<template>
  <main>
    <div class="auth-container">
      <div class="auth-content">
        <div class="logo">
          <i class="fas fa-gamepad"></i> GamePulse
        </div>

        <!-- Вкладки -->
        <div class="tabs">
          <div 
            v-if="!isConfirming"
            class="tab" 
            :class="{ active: currentForm === 'login' }"
            @click="currentForm = 'login'"
          >
            Войти
          </div>
          <div 
            v-if="!isConfirming"
            class="tab" 
            :class="{ active: currentForm === 'register' }"
            @click="currentForm = 'register'"
          >
            Регистрация
          </div>
          <div v-else class="confirm-form">
            Подтверждение
          </div>
        </div>

        <!-- Форма входа -->
        <form v-if="currentForm === 'login' && !isConfirming" @submit.prevent="login" class="auth-form">
          <div class="form-group">
            <label for="loginEmail">Email</label>
            <input 
              id="loginEmail" 
              v-model="loginForm.email" 
              type="email" 
              class="form-control" 
              required
            >
            <div v-if="errors.loginEmail" class="error">{{ errors.loginEmail }}</div>
          </div>
          <div class="form-group">
            <label for="loginPassword">Пароль</label>
            <div class="password-wrapper">
              <input 
                id="loginPassword" 
                v-model="loginForm.password" 
                :type="showLoginPassword ? 'text' : 'password'"
                class="form-control" 
                minlength="6" 
                required
              >
              <button type="button" class="toggle-password" @click="toggleLoginPassword" aria-label="Показать пароль">
                <i v-if="showLoginPassword" class="fas fa-eye-slash"></i>
                <i v-else class="fas fa-eye"></i>
              </button>
            </div>
            <div v-if="errors.loginPassword" class="error">{{ errors.loginPassword }}</div>
          </div>
          <div v-if="serverError.login" class="error server-error">{{ serverError.login }}</div>
          <button type="submit" class="btn">Войти</button>
        </form>

        <!-- Форма регистрации -->
        <form v-if="currentForm === 'register' && !isConfirming" @submit.prevent="register" class="auth-form">
          <div class="form-group">
            <label for="regName">Имя</label>
            <input 
              id="regName" 
              v-model="registerForm.name" 
              type="text" 
              class="form-control" 
              required
            >
            <div v-if="errors.regName" class="error">{{ errors.regName }}</div>
          </div>
          <div class="form-group">
            <label for="regEmail">Email</label>
            <input 
              id="regEmail" 
              v-model="registerForm.email" 
              type="email" 
              class="form-control" 
              required
            >
            <div v-if="errors.regEmail" class="error">{{ errors.regEmail }}</div>
          </div>
          <div class="form-group">
            <label for="regPassword">Пароль</label>
            <div class="password-wrapper">
              <input 
                id="regPassword" 
                v-model="registerForm.password" 
                :type="showRegisterPassword ? 'text' : 'password'"
                class="form-control" 
                minlength="6" 
                required
              >
              <button type="button" class="toggle-password" @click="toggleRegisterPassword" aria-label="Показать пароль">
                <i v-if="showRegisterPassword" class="fas fa-eye-slash"></i>
                <i v-else class="fas fa-eye"></i>
              </button>

              <div class="line"></div>
            </div>
            <div v-if="errors.regPassword" class="error">{{ errors.regPassword }}</div>
          </div>
          <div class="form-group">
            <label for="regConfirmPassword">Подтверждение пароля</label>
            <div class="password-wrapper">
              <input 
                id="regConfirmPassword" 
                v-model="registerForm.confirmPassword" 
                :type="showConfirmPassword ? 'text' : 'password'"
                class="form-control" 
                minlength="6" 
                required
              >
              <button type="button" class="toggle-password" @click="toggleConfirmPassword" aria-label="Показать пароль">
                <i v-if="showConfirmPassword" class="fas fa-eye-slash"></i>
                <i v-else class="fas fa-eye"></i>
              </button>
            </div>
            <div v-if="errors.regConfirmPassword" class="error">{{ errors.regConfirmPassword }}</div>
          </div>
          <div class="form-group consent">
            <label class="consent-label">
              <input 
                v-model="registerForm.consent" 
                type="checkbox" 
                required
              >
              <span>Согласен на <a href="#" @click.prevent>обработку персональных данных</a></span>
            </label>
            <div v-if="errors.regConsent" class="error">{{ errors.regConsent }}</div>
          </div>
          <div v-if="serverError.register" class="error server-error">{{ serverError.register }}</div>
          <button type="submit" class="btn">Зарегистрироваться</button>
        </form>

      
        <form v-if="isConfirming" @submit.prevent="confirmEmail" class="auth-form-confirm">
          <p class="confirm-instruction">
            Мы отправили код подтверждения на <strong>{{ registerForm.email }}</strong>
          </p>
          <div class="form-group">
            <label for="confirmCode">Код подтверждения</label>
            <input 
              id="confirmCode" 
              v-model="confirmCode" 
              type="text" 
              class="form-control" 
              placeholder="Введите 6-значный код"
              maxlength="6"
              inputmode="numeric"
              required
            >
            <div v-if="errors.confirmCode" class="error">{{ errors.confirmCode }}</div>
          </div>
          <div v-if="serverError.confirm" class="error server-error">{{ serverError.confirm }}</div>
          <button type="submit" class="btn">Подтвердить email</button>
          <button type="button" class="btn btn-secondary" @click="cancelConfirmation">
            Назад
          </button>
        </form>

        <div v-if="!isConfirming" class="footer-link">
          Нет аккаунта? 
          <span @click="currentForm = 'register'" style="color: var(--color-primary); font-weight: 600; cursor: pointer;">
            Создать
          </span>
        </div>
      </div>
    </div>
  </main>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const router = useRouter()
const currentForm = ref('login')
const isConfirming = ref(false)
const confirmCode = ref('')

const loginForm = reactive({
  email: '',
  password: ''
})

const registerForm = reactive({
  name: '',
  email: '',
  password: '',
  confirmPassword: '',
  consent: false
})

const errors = reactive({
  loginEmail: '',
  loginPassword: '',
  regName: '',
  regEmail: '',
  regPassword: '',
  regConfirmPassword: '',
  regConsent: '',
  confirmCode: ''
})

const serverError = reactive({
  login: '',
  register: '',
  confirm: ''
})

const showLoginPassword = ref(false)
const showRegisterPassword = ref(false)
const showConfirmPassword = ref(false)

const toggleLoginPassword = () => {
  showLoginPassword.value = !showLoginPassword.value
}

const toggleRegisterPassword = () => {
  showRegisterPassword.value = !showRegisterPassword.value
}

const toggleConfirmPassword = () => {
  showConfirmPassword.value = !showConfirmPassword.value
}

const validateEmail = (email) => {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)
}

const validateLogin = () => {
  errors.loginEmail = ''
  errors.loginPassword = ''
  serverError.login = ''
  
  if (!loginForm.email) {
    errors.loginEmail = 'Email обязателен'
    return false
  }
  
  if (!validateEmail(loginForm.email)) {
    errors.loginEmail = 'Неверный формат email'
    return false
  }
  
  if (!loginForm.password) {
    errors.loginPassword = 'Пароль обязателен'
    return false
  }
  
  return true
}

const validateRegister = () => {
  errors.regName = ''
  errors.regEmail = ''
  errors.regPassword = ''
  errors.regConfirmPassword = ''
  errors.regConsent = ''
  serverError.register = ''
  
  if (!registerForm.name) {
    errors.regName = 'Имя обязательно'
    return false
  }
  
  if (!registerForm.email) {
    errors.regEmail = 'Email обязателен'
    return false
  }
  
  if (!validateEmail(registerForm.email)) {
    errors.regEmail = 'Неверный формат email'
    return false
  }
  
  if (!registerForm.password) {
    errors.regPassword = 'Пароль обязателен'
    return false
  }
  
  if (registerForm.password.length < 6) {
    errors.regPassword = 'Пароль должен содержать минимум 6 символов'
    return false
  }
  
  if (!registerForm.confirmPassword) {
    errors.regConfirmPassword = 'Подтверждение пароля обязательно'
    return false
  }
  
  if (registerForm.password !== registerForm.confirmPassword) {
    errors.regConfirmPassword = 'Пароли не совпадают'
    return false
  }
  
  if (!registerForm.consent) {
    errors.regConsent = 'Необходимо согласие на обработку персональных данных'
    return false
  }
  
  return true
}

const extractErrorMessage = (message) => {
  if (typeof message === 'string' && message.includes(': ')) {
    return message.split(': ').slice(1).join(': ')
  }
  return message
}

const login = async () => {
  const isValid = validateLogin()
  if (!isValid) return

  try {
    await authStore.login({
      email: loginForm.email,
      password: loginForm.password
    })
    router.push('/')
  } catch (error) {
    let message = 'Ошибка при входе'
    if (error.response?.data?.error) {
      message = extractErrorMessage(error.response.data.error)
    } else if (error.response?.data) {
      message = extractErrorMessage(error.response.data)
    }
    serverError.login = message
    console.error('Login error:', error)
  }
}

const register = async () => {
  const isValid = validateRegister()
  if (!isValid) return

  try {
    // Регистрация без входа
    await authStore.register({
      name: registerForm.name,
      email: registerForm.email,
      password: registerForm.password,
      age: 18
    })

    // Переход к подтверждению
    isConfirming.value = true
    serverError.register = ''
  } catch (error) {
    let message = 'Ошибка при регистрации'
    if (error.response?.data?.error) {
      message = extractErrorMessage(error.response.data.error)
    } else if (error.response?.data) {
      message = extractErrorMessage(error.response.data)
    }
    serverError.register = message
    console.error('Register error:', error)
  }
}

const confirmEmail = async () => {
  errors.confirmCode = ''
  serverError.confirm = ''

  if (!confirmCode.value) {
    errors.confirmCode = 'Введите код подтверждения'
    return
  }

  if (confirmCode.value.length !== 6 || isNaN(confirmCode.value)) {
    errors.confirmCode = 'Код должен содержать 6 цифр'
    return
  }

  try {
    await authStore.confirmEmail({
      email: registerForm.email,
      code: confirmCode.value
    })

    // Вход после подтверждения
    await authStore.login({
      email: registerForm.email,
      password: registerForm.password
    })
    router.push('/')
  } catch (error) {
    let message = 'Неверный код подтверждения'
    if (error.response?.data?.error) {
      message = extractErrorMessage(error.response.data.error)
    } else if (error.response?.data) {
      message = extractErrorMessage(error.response.data)
    }
    serverError.confirm = message
    console.error('Confirm error:', error)
  }
}

const cancelConfirmation = () => {
  isConfirming.value = false
  confirmCode.value = ''
  serverError.confirm = ''
}
</script>

<style scoped>
main {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100vh;
}

.auth-container {
  width: 100%;
  max-width: 420px;
  background: var(--color-card);
  border-radius: 20px;
  padding: 2.5rem;
  box-shadow: 0 20px 50px rgba(0, 0, 0, 0.5);
  position: relative;
  overflow: hidden;
}

.auth-container::before {
  content: '';
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: linear-gradient(45deg, transparent, var(--color-secondary), var(--color-primary), transparent);
  animation: rotate 6s linear infinite;
  z-index: 0;
  opacity: 0.15;
}

@keyframes rotate {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.auth-content {
  position: relative;
  z-index: 1;
}

.logo {
  text-align: center;
  margin-bottom: 2rem;
  font-size: 2.2rem;
  font-weight: 800;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.logo i {
  color: var(--color-primary);
}

.tabs {
  display: flex;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 12px;
  margin-bottom: 2rem;
  overflow: hidden;
}

.tab {
  flex: 1;
  text-align: center;
  padding: 0.9rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.tab.active {
  background: linear-gradient(90deg, var(--color-secondary), var(--color-primary));
  color: #000;
}

.form-group {
  margin-bottom: 1.4rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: var(--color-text);
}

.form-control {
  width: 100%;
  padding: 0.85rem 1.2rem;
  padding-right: 3rem;
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid #333;
  border-radius: 12px;
  color: white;
  font-size: 1rem;
  font-family: 'Inter', sans-serif;
}

.form-control:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 2px rgba(0, 240, 255, 0.2);
}

.password-wrapper {
  position: relative;
}

.toggle-password {
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  cursor: pointer;
  padding: 0;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #aaa;
  font-size: 1.1rem;
}

.toggle-password:hover {
  color: var(--color-primary);
}

.btn {
  width: 100%;
  padding: 0.9rem;
  background: linear-gradient(90deg, var(--color-secondary), var(--color-primary));
  color: #000;
  font-weight: 600;
  font-size: 1.05rem;
  border: none;
  border-radius: 12px;
  cursor: pointer;
  transition: opacity 0.3s;
}

.btn:hover {
  opacity: 0.95;
}

.btn-secondary {
  background: rgba(255, 255, 255, 0.1);
  color: white;
  margin-top: 0.8rem;
}

.btn-secondary:hover {
  opacity: 0.9;
}

.footer-link {
  text-align: center;
  margin-top: 1.5rem;
  color: var(--color-text-secondary);
  font-size: 0.95rem;
}

.error {
  color: #ff6b6b;
  font-size: 0.85rem;
  margin-top: 0.4rem;
}

.server-error {
  margin-top: 0.8rem;
  padding: 0.5rem;
  background: rgba(255, 107, 107, 0.15);
  border-radius: 8px;
  text-align: center;
}

.consent {
  margin-top: 0.5rem;
}

.consent-label {
  display: flex;
  align-items: flex-start;
  gap: 0.6rem;
  font-size: 0.9rem;
  color: var(--color-text-secondary);
  cursor: pointer;
}

.consent-label input {
  margin-top: 0.25rem;
  transform: scale(1.2);
  cursor: pointer;
}

.consent-label a {
  color: var(--color-primary);
  text-decoration: underline;
}

.consent-label a:hover {
  opacity: 0.8;
}

.confirm-instruction {
  text-align: center;
  margin-bottom: 1.5rem;
  color: var(--color-text-secondary);
}
.confirm-form{
  align-items: center;
  background-color: none;
}
</style>