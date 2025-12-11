
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
    const name = user.value.name.trim()
    if (!name) return '#2B00FFFF'


    let hash = 0
    for (let i = 0; i < name.length; i++) {
      hash = name.charCodeAt(i) + ((hash << 5) - hash)
    }

    const hue = Math.abs(hash) % 360
    return `hsl(${hue}, 80%, 45%)`
  })



  const isAdmin = computed(() => user.value?.roles?.includes('Admin'))
  const isSuperUser = computed(() => user.value?.roles?.includes('SuperUser'))
  const isSupport = computed(() => user.value?.roles?.includes('Support'))
  const isModerator = computed(() => user.value?.roles?.includes('Moderator'))
  const isUser = computed(() => user.value?.roles?.includes('User'))


  const canAccessAdminPanel = computed(() => isAdmin.value)
  const canAddGames = computed(() => isSuperUser.value || isAdmin.value)
  const canAccessSupport = computed(() => isSupport.value || isAdmin.value)
  const canAccessModeration = computed(() => isModerator.value || isAdmin.value)



  const login = async (credentials) => {
    try {
      loading.value = true
      error.value = null
      const data = await api.auth.login(credentials)
      const userData = {
        id: data.id,
        name: data.name,
        email: data.email,
        avatarUrl: data.avatarUrl || null,
        roles: Array.isArray(data.roles) ? data.roles : [data.roles]
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
  const updateUser = (updatedFields) => {
    if (!user.value) return
    user.value = { ...user.value, ...updatedFields }
    localStorage.setItem('user', JSON.stringify(user.value))
  }
  const getToken = () => localStorage.getItem('authToken')
  const getAvatar = async () => {
    try {
      const profile = await api.profile.get()
      localAvatarUrl.value = profile.avatarUrl || DEFAULT_AVATAR
      localName.value = profile.name
      email.value = profile.email


      authStore.updateUser({
        name: profile.name,
        email: profile.email,
        avatarUrl: profile.avatarUrl || null
      })
    } catch (error) {
      console.error('Ошибка загрузки профиля:', error)

    }
  }
  return {

    isAuthenticated,
    user,
    loading,
    error,


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
    updateUser,


    login,
    register,
    confirmEmail,
    resendConfirmation,
    logout,
    getAvatar,
    getToken
  }
})