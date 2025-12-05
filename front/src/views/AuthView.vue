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
                @input="checkPasswordStrength"
              >
              <button type="button" class="toggle-password" @click="toggleRegisterPassword" aria-label="Показать пароль">
                <i v-if="showRegisterPassword" class="fas fa-eye-slash"></i>
                <i v-else class="fas fa-eye"></i>
              </button>
            </div>
            
            <div v-if="registerForm.password" class="password-strength">
              <div 
                class="strength-bar" 
                :class="passwordStrengthClass"
                :style="{ width: passwordStrengthWidth }"
              ></div>
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

        <!-- Форма подтверждения -->
        <form v-if="isConfirming" @submit.prevent="confirmEmail" class="auth-form">
          <h2 class="confirm-h">Подтверждение email</h2>
          <p class="confirm-instruction">
            {{ isFromLogin ? 'Ваш email не подтверждён. Введите код из письма:' : 'Мы отправили код подтверждения на' }}
            <strong>{{ registerForm.email }}</strong>
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
          
          <!-- Кнопка повторной отправки -->
          <div class="resend-section">
            <p>Не получили код?</p>
            <button 
              type="button" 
              class="resend-btn" 
              @click="resendConfirmationCode"
              :disabled="resendCooldown > 0"
            >
              {{ resendCooldown > 0 ? `Отправить повторно через ${resendCooldown} сек` : 'Отправить код ещё раз' }}
            </button>
          </div>
          
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
import { ref, reactive, computed, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const router = useRouter()
const currentForm = ref('login')
const isConfirming = ref(false)
const isFromLogin = ref(false) // Новый флаг
const confirmCode = ref('')
const resendCooldown = ref(0)

onUnmounted(() => {
  if (window.resendTimer) {
    clearInterval(window.resendTimer)
    window.resendTimer = null
  }
})

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

const passwordStrength = ref(0)
const passwordStrengthMessages = {
  0: 'Слишком слабый',
  1: 'Слабый',
  2: 'Средний',
  3: 'Сильный'
}

const checkPasswordStrength = () => {
  const password = registerForm.password
  let strength = 0
  if (password.length >= 6) strength++
  if (/[a-z]/.test(password)) strength++
  if (/[A-Z]/.test(password)) strength++
  if (/[0-9]/.test(password)) strength++
  if (/[^A-Za-z0-9]/.test(password)) strength++
  passwordStrength.value = Math.min(strength, 4)
}

const passwordStrengthWidth = computed(() => {
  const widths = ['25%', '50%', '75%', '100%']
  return widths[Math.min(passwordStrength.value - 1, 3)] || '0%'
})

const passwordStrengthClass = computed(() => {
  const classes = ['weak', 'medium', 'strong', 'very-strong']
  return classes[Math.min(passwordStrength.value - 1, 3)] || 'very-weak'
})

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
  
  if (passwordStrength.value < 2) {
    errors.regPassword = 'Пароль слишком слабый'
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

const resendConfirmationCode = async () => {
  if (resendCooldown.value > 0) return

  try {
    await authStore.resendConfirmation(registerForm.email)
    serverError.confirm = ''
    
    resendCooldown.value = 60
    window.resendTimer = setInterval(() => {
      resendCooldown.value--
      if (resendCooldown.value <= 0) {
        clearInterval(window.resendTimer)
        window.resendTimer = null
      }
    }, 1000)
  } catch (error) {
    let message = 'Не удалось отправить код'
    if (error.response?.data?.error) {
      message = extractErrorMessage(error.response.data.error)
    } else if (error.response?.data) {
      message = extractErrorMessage(error.response.data)
    }
    serverError.confirm = message
    console.error('Resend error:', error)
  }
}

// ОБНОВЛЁННЫЙ МЕТОД LOGIN
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

    // 🔑 АВТОМАТИЧЕСКОЕ ПЕРЕКЛЮЧЕНИЕ НА ПОДТВЕРЖДЕНИЕ
    if (message.includes('Email не подтверждён')) {
      isConfirming.value = true
      isFromLogin.value = true
      registerForm.email = loginForm.email // Сохраняем email
      serverError.login = ''
      serverError.confirm = ''
    } else {
      serverError.login = message
    }
    
    console.error('Login error:', error)
  }
}

const register = async () => {
  const isValid = validateRegister()
  if (!isValid) return

  try {
    await authStore.register({
      name: registerForm.name,
      email: registerForm.email,
      password: registerForm.password,
      age: 18
    })
    isConfirming.value = true
    isFromLogin.value = false
    serverError.register = ''
    resendCooldown.value = 0
    if (window.resendTimer) {
      clearInterval(window.resendTimer)
      window.resendTimer = null
    }
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

// ОБНОВЛЁННЫЙ МЕТОД CONFIRM EMAIL
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

    // Автоматический вход после подтверждения
    await authStore.login({
      email: registerForm.email,
      // Если пришли из формы входа - пароль уже введён
      password: isFromLogin.value ? loginForm.password : registerForm.password
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
  isFromLogin.value = false
  confirmCode.value = ''
  serverError.confirm = ''
  resendCooldown.value = 0
  if (window.resendTimer) {
    clearInterval(window.resendTimer)
    window.resendTimer = null
  }
  registerForm.email = ''
}
</script>

<style lang="scss" scoped src="@/assets/style/views/auth/index.scss"></style>

<style scoped>
.auth-container {
  .resend-section {
    margin: 20px 0;
    text-align: center;
    
    p {
      margin-bottom: 8px;
      color: var(--color-text-secondary, #666);
      font-size: 14px;
    }
    
    .resend-btn {
      background: none;
      border: none;
      color: var(--color-primary, #e74c3c);
      font-weight: 600;
      cursor: pointer;
      padding: 0;
      font-size: 14px;
      text-decoration: underline;
      
      &:disabled {
        opacity: 0.5;
        cursor: not-allowed;
        text-decoration: none;
      }
    }
  }
}
</style>