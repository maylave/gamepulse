<template>
  <main>
    <div class="auth-container">
    <div class="auth-content">
      <div class="logo">GamePulse</div>

      <div class="tabs">
        <div 
          class="tab" 
          :class="{ active: currentForm === 'login' }"
          @click="currentForm = 'login'"
        >
          Войти
        </div>
        <div 
          class="tab" 
          :class="{ active: currentForm === 'register' }"
          @click="currentForm = 'register'"
        >
          Регистрация
        </div>
      </div>

      <!-- Форма входа -->
      <form v-if="currentForm === 'login'" @submit.prevent="login" class="auth-form">
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
          <input 
            id="loginPassword" 
            v-model="loginForm.password" 
            type="password" 
            class="form-control" 
            minlength="6" 
            required
          >
          <div v-if="errors.loginPassword" class="error">{{ errors.loginPassword }}</div>
        </div>
        <button type="submit" class="btn">Войти</button>
      </form>

    
      <form v-if="currentForm === 'register'" @submit.prevent="register" class="auth-form">
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
          <input 
            id="regPassword" 
            v-model="registerForm.password" 
            type="password" 
            class="form-control" 
            minlength="6" 
            required
          >
          <div v-if="errors.regPassword" class="error">{{ errors.regPassword }}</div>
        </div>
        <button type="submit" class="btn">Зарегистрироваться</button>
      </form>

      <div class="footer-link">
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


const loginForm = reactive({
  email: '',
  password: ''
})

const registerForm = reactive({
  name: '',
  email: '',
  password: ''
})


const errors = reactive({
  loginEmail: '',
  loginPassword: '',
  regName: '',
  regEmail: '',
  regPassword: ''
})


const validateEmail = (email) => {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)
}


const validateLogin = () => {
  errors.loginEmail = ''
  errors.loginPassword = ''
  
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
  
  return true
}

const login = async () => {
  const isValid = validateLogin()
  
  if (isValid) {
    try {
      await authStore.login({
        email: loginForm.email,
        password: loginForm.password
      })
      router.push('/')
    } catch (error) {
      alert( error.message)
    }
  }
}

const register = async () => {
  const isValid = validateRegister()
  
  if (isValid) {
    try {
      await authStore.register({
        name: registerForm.name,
        email: registerForm.email,
        password: registerForm.password,
        age: 18
      })
      
    
      await authStore.login({
        email: registerForm.email,
        password: registerForm.password
      })
      router.push('/')
    } catch (error) {
      alert(error.message)
    }
  }
}
</script>


<style scoped>
main{
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
  background: linear-gradient(90deg, var(--color-primary), var(--color-secondary));
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
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
</style>