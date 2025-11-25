// src/services/auth.js
const API_BASE = 'https://localhost:7066/api/users'

export const authService = {
  
  async register(userData) {
    const response = await fetch(API_BASE, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(userData)
    })
    
    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || 'Ошибка регистрации')
    }
    
    const user = await response.json()
    return user
  },

  // Вход
  async login(credentials) {
    const response = await fetch(`${API_BASE}/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(credentials)
    })
    
    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || 'Ошибка входа')
    }
    const data = await response.json()
    localStorage.setItem('authToken', data.token)
    localStorage.setItem('user', JSON.stringify({
      id: data.id,
      name: data.name,
      email: data.email
    }))
    return data
  },
  logout() {
    localStorage.removeItem('authToken')
    localStorage.removeItem('user')
  },
  getCurrentUser() {
    const user = localStorage.getItem('user')
    return user ? JSON.parse(user) : null
  },

  isAuthenticated() {
    return !!localStorage.getItem('authToken')
  }
}