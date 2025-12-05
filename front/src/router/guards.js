// src/routes/guards.js
import { uiStoreInstance } from '@/stores/uiStoreInstance'



function getUserFromStorage() {
  try {
    const userStr = localStorage.getItem('user')
    return userStr ? JSON.parse(userStr) : null
  } catch (e) {
    console.warn('Не удалось прочитать данные пользователя:', e)
    return null
  }
}

function hasRole(requiredRoles) {
  const user = getUserFromStorage()
  if (!user || !Array.isArray(user.roles)) return false
  const roles = Array.isArray(requiredRoles) ? requiredRoles : [requiredRoles]
  return roles.some(role => user.roles.includes(role))
}


function canAccessAdminPanel() {
  return hasRole('Admin')
}

function canAddGames() {
  return hasRole(['SuperUser', 'Admin'])
}

function canAccessSupport() {
  return hasRole(['Support', 'Admin'])
}

function canAccessModeration() {
  return hasRole(['Moderator', 'Admin'])
}



export function setupGlobalGuards(router) {
  router.beforeEach((to, from, next) => {
  
    if (to.meta.requiresAdminPanel && !canAccessAdminPanel()) {
      next('/403')
      return
    }

  
    if (to.meta.requiresAddGame && !canAddGames()) {
      next('/403')
      return
    }

    
    if (to.meta.requiresSupport && !canAccessSupport()) {
      next('/403')
      return
    }

  
    if (to.meta.requiresModeration && !canAccessModeration()) {
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