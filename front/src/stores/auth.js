
import { defineStore } from 'pinia'
import { ref, computed } from 'vue' // 👈 добавь computed
import { api } from '@/services/api'

export const useAuthStore = defineStore('auth', () => {
  const isAuthenticated = ref(!!localStorage.getItem('authToken'))


  const user = ref(
    JSON.parse(localStorage.getItem('user') || 'null')
  )

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
  const login = async (credentials) => {
    try {
      loading.value = true
      error.value = null

      const data = await api.auth.login(credentials)

      const userData = {
        id: data.id,
        name: data.name,
        email: data.email,
        roles: data.roles // ← обязательно включить
      }

      localStorage.setItem('authToken', data.token)
      localStorage.setItem('user', JSON.stringify(userData))

      isAuthenticated.value = true
      user.value = userData // ← передавай полный объект

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

  const logout = () => {
    api.auth.logout()

    localStorage.removeItem('authToken')
    localStorage.removeItem('user')
    isAuthenticated.value = false
    user.value = null
  }

  const getToken = () => localStorage.getItem('authToken')
  const isAdmin = computed(() => user.value?.roles?.includes('Admin'));
  return {
    isAuthenticated,
    user,
    userInitial,
    userColor,
    isAdmin,
    loading,
    error,
    login,
    register,
    logout,
    getToken
  }
})