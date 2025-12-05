function getUser() {
  try {
    const userStr = localStorage.getItem('user')
    return userStr ? JSON.parse(userStr) : null
  } catch (e) {
    console.warn('Не удалось прочитать пользователя:', e)
    return null
  }
}

function hasRole(requiredRoles) {
  const user = getUser()
  if (!user || !Array.isArray(user.roles)) return false
  const roles = Array.isArray(requiredRoles) ? requiredRoles : [requiredRoles]
  return roles.some(role => user.roles.includes(role))
}



export const Roles = {
  User: 'User',
  Support: 'Support',
  Moderator: 'Moderator',
  Admin: 'Admin',
  SuperUser: 'SuperUser'
}

export { hasRole }




export function isUser() {
  return hasRole('User')
}


export function canUseSupportPanel() {
  return hasRole(['Support', 'Admin'])
}


export function canModerate() {
  return hasRole(['Moderator', 'Admin'])
}


export function canManageGames() {
  return hasRole(['SuperUser', 'Admin'])
}


export function canManageUsersAndRoles() {
  return hasRole('Admin') // Только Admin!
}


export function isAdmin() {
  return hasRole('Admin')
}


export function isSuperUser() {
  return hasRole('SuperUser')
}