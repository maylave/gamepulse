import { uiStoreInstance } from '@/stores/uiStoreInstance'

export function hasAdminRole() {
  try {
    const userStr = localStorage.getItem('user')
    if (!userStr) return false
    const user = JSON.parse(userStr)
    return Array.isArray(user.roles) && 
           (user.roles.includes('Admin') || user.roles.includes('SuperAdmin'))
  } catch (e) {
    console.warn('Не удалось прочитать данные пользователя:', e)
    return false
  }
}

export function setupGlobalGuards(router) {
  router.beforeEach((to, from, next) => {
  
    if (to.meta.requiresAdmin && !hasAdminRole()) {
      next('/403')
      return
    }

    
    if (to.meta.requiresLoading) {
      const message = to.meta.loadingMessage || 'Загрузка страницы...'
      uiStoreInstance.startLoading(message)
    } else {
      uiStoreInstance.endLoading()
    }

    next()
  })

  router.afterEach((to) => {
    if (to.meta.requiresLoading) {
      setTimeout(() => {
        uiStoreInstance.endLoading()
      }, 600)
    }
  })
}