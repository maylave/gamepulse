// src/stores/auth.js
import { defineStore } from 'pinia'
import { ref, computed } from 'vue' 
import { api } from '@/services/api'

export const useAuthStore = defineStore('auth', () => {
  const isAuthenticated = ref(!!localStorage.getItem('authToken'))
  const user = ref(JSON.parse(localStorage.getItem('user') || 'null'))
  const loading = ref(false)
  const error = ref(null)

  const userInitial = computed(() => {
    if (!user.value?.name) return '?'
    return user.value.name.trim().charAt(0).toUpperCase()
  })

  const userColor = computed(() => {
    if (!user.value?.name) return '#2B00FFFF'
    const firstChar = user.value.name.trim().charAt(0).toUpperCase()
    const charCode = firstChar.charCodeAt(0)
    let hue = (charCode % 26) * 10
    hue = Math.min(hue, 360)
    return `hsl(${hue}, 80%, 45%)`
  })

  // === РОЛЕВЫЕ ПРОВЕРКИ ===

  const isAdmin = computed(() => user.value?.roles?.includes('Admin'))
  const isSuperUser = computed(() => user.value?.roles?.includes('SuperUser'))
  const isSupport = computed(() => user.value?.roles?.includes('Support'))
  const isModerator = computed(() => user.value?.roles?.includes('Moderator'))
  const isUser = computed(() => user.value?.roles?.includes('User'))

  // Комплексные проверки (как в auth.js из services)
  const canAccessAdminPanel = computed(() => isAdmin.value)
  const canAddGames = computed(() => isSuperUser.value || isAdmin.value)
  const canAccessSupport = computed(() => isSupport.value || isAdmin.value)
  const canAccessModeration = computed(() => isModerator.value || isAdmin.value)

  // === АУТЕНТИФИКАЦИЯ ===

  const login = async (credentials) => {
    try {
      loading.value = true
      error.value = null
      const data = await api.auth.login(credentials)
      const userData = {
        id: data.id,
        name: data.name,
        email: data.email,
        roles: Array.isArray(data.roles) ? data.roles : [data.roles] // Защита на случай строки
      }
      localStorage.setItem('authToken', data.token)
      localStorage.setItem('user', JSON.stringify(userData))
      isAuthenticated.value = true
      user.value = userData 
      return data
    } catch (err) {
      error.value = err.message || 'Ошибка входа'
      throw err
    } finally {
      loading.value = false
    }
  }

  const register = async (userData) => {
    try {
      loading.value = true
      error.value = null
      await api.auth.register(userData)
      return true
    } catch (err) {
      error.value = err.message || 'Ошибка регистрации'
      throw err
    } finally {
      loading.value = false
    }
  }

  const confirmEmail = async (data) => {
    try {
      loading.value = true
      error.value = null
      await api.auth.confirmEmail(data)
    } catch (err) {
      error.value = err.message || 'Ошибка подтверждения email'
      throw err
    } finally {
      loading.value = false
    }
  }

  const resendConfirmation = async (email) => {
    try {
      loading.value = true
      error.value = null
      await api.auth.resendConfirmation({ email })
    } catch (err) {
      error.value = err.message || 'Не удалось отправить код повторно'
      throw err
    } finally {
      loading.value = false
    }
  }

  const logout = () => {
    api.auth.logout()
    localStorage.removeItem('authToken')
    localStorage.removeItem('user')
    isAuthenticated.value = false
    user.value = null
  }

  const getToken = () => localStorage.getItem('authToken')

  return {
    // Состояния
    isAuthenticated,
    user,
    loading,
    error,

    // Вычисляемые свойства — роли
    isAdmin,
    isSuperUser,
    isSupport,
    isModerator,
    isUser,
    canAccessAdminPanel,
    canAddGames,
    canAccessSupport,
    canAccessModeration,
    userInitial,
    userColor,

    // Методы
    login,
    register,
    confirmEmail,
    resendConfirmation,
    logout,
    getToken
  }
})