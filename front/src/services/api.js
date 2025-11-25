// src/services/api.js

import { useAuthStore } from '@/stores/auth'

// === Конфигурация ===
const API_CONFIG = {
  primary:  'https://localhost:7066/api',
  //backup: '/api',
  timeout: 5000
}

// === Демо-данные (только для GET) ===
const DEMO_GAMES = [
  {
    id: 1,
    title: 'Cyberpunk 2077',
    price: 1299,
    oldPrice: 3249,
    tag: '-60%',
    image: 'https://images.unsplash.com/photo-1552820728-8b83bb6b773f?w=800&q=80',
    description: 'Open-world RPG от создателей The Witcher 3.',
    genre: 'RPG, Экшен',
    developer: 'CD Projekt RED',
    ageRating: 18,
    isPreorder: false,
    releaseDate: '2020-12-10'
  },
  {
    id: 2,
    title: 'Elden Ring',
    price: 2499,
    image: 'https://images.unsplash.com/photo-1511512578717-92578b575e9d?w=800&q=80',
    description: 'Эпическая фэнтези-RPG от FromSoftware.',
    genre: 'RPG, Экшен',
    developer: 'FromSoftware',
    ageRating: 16,
    isPreorder: false,
    releaseDate: '2022-02-25'
  }
]

const DEMO_CART = [{ id: 1, gameId: 1, quantity: 1 }]
const DEMO_WISHLIST = [{ id: 1, gameId: 2 }]


const fetchWithTimeout = async (url, options = {}) => {
  const controller = new AbortController()
  const timeoutId = setTimeout(() => controller.abort(), API_CONFIG.timeout)
  try {
    const response = await fetch(url, { ...options, signal: controller.signal })
    clearTimeout(timeoutId)
    return response
  } catch (error) {
    clearTimeout(timeoutId)
    throw error
  }
}

const request = async (endpoint, options = {}) => {
  const authStore = useAuthStore()
    const token = localStorage.getItem('authToken')
  const userStr = localStorage.getItem('user')
 const userId = userStr ? JSON.parse(userStr).id : null
  const urls = [
    `${API_CONFIG.primary}${endpoint}`//,
   // `${API_CONFIG.backup}${endpoint}`
  ]

  const baseHeaders = { 'Content-Type': 'application/json' }
    if (token) {
    baseHeaders.Authorization = `Bearer ${token}`
  }
  if (userId) {
    baseHeaders['X-User-Id'] = userId.toString()
  }
  const requestOptions = {
    ...options,
    headers: { ...baseHeaders, ...options.headers }
  }

  try {
    const res = await fetchWithTimeout(urls[0], requestOptions)
    if (res.ok) return res.status === 204 ? null : await res.json()
    const err = await res.text().catch(() => 'Server error')
    throw new Error(`HTTP ${res.status}: ${err}`)
  } catch (e) {
    console.warn(' Основной API недоступен:', e.message)
  }


 


  if (!['GET', 'HEAD'].includes(options.method || 'GET')) {
    throw new Error('Сервер недоступен. Попробуйте позже.')
  }


  if (endpoint === '/games') return DEMO_GAMES
  if (endpoint.startsWith('/games/')) {
    const id = endpoint.split('/').pop()
    const game = DEMO_GAMES.find(g => g.id == id)
    return game ? { ...game, reviews: [] } : null
  }
  if (endpoint === '/cart') return { items: DEMO_CART }
  if (endpoint === '/wishlist') return DEMO_WISHLIST

  throw new Error('Данные недоступны')
}


export const api = {
  auth: {
    register: (data) => request('/users', { method: 'POST', body: JSON.stringify(data) }),
    login: (data) => request('/users/login', { method: 'POST', body: JSON.stringify(data) }),
   logout: () => {
  localStorage.removeItem('authToken')
  localStorage.removeItem('user')
}
  },

  games: {
    get: () => request('/games'),
      getAll: (params = {}) => {
    const { category, page = 1, limit = 24 } = params

   
    const queryParams = new URLSearchParams()
    if (category && category !== 'all') {
      queryParams.append('category', category)
    }
    queryParams.append('page', page.toString())
    queryParams.append('limit', limit.toString())

    const queryString = queryParams.toString()
    const url = queryString ? `/games?${queryString}` : '/games'

    return request(url)
  },
    getById: (id) => request(`/games/${id}`),
    getAllUnlimited: () => request('/games', { params: { pageSize: 1000 } }),
    search: (q) => request(`/games/search?q=${encodeURIComponent(q)}`)
  },

  cart: {
    get: () => request('/cart'),
  
    add: (gameId) => request('/cart', {
      method: 'POST',
      body: JSON.stringify({ gameId })
    }),
    remove: (itemId) => request(`/cart/${itemId}`, { method: 'PUT' }),
    clear: () => request('/cart', { method: 'DELETE' })
  },

  wishlist: {
    get: () => request('/wishlist'),
    add: (gameId) => request('/wishlist', {
      method: 'POST',
      body: JSON.stringify({ gameId })
    }),
    remove: (gameId) => request(`/wishlist/${gameId}`, { method: 'DELETE' })
  },

  reviews: {
    add: (gameId, data) => request(`/games/${gameId}/reviews`, {
      method: 'POST',
      body: JSON.stringify(data)
    })
  },

 profile: {
  get: () => request('/Users/profile'),
  update: (data, options = {}) => {
   
    return request('/Users/profile', {
      method: 'PATCH',
      body: JSON.stringify(data),
      userId: options.userId 
    })
  }
},

  admin: {
  games: {
    getAll: () => request('/admin/games'),
    create: (data) => request('/admin/games', { method: 'POST', body: JSON.stringify(data) }),
    update: (id, data) => request(`/admin/games/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
    delete: (id) => request(`/admin/games/${id}`, { method: 'DELETE' })
  },
genres: {
  getAll: () => request('/admin/genre'),
  create: (data) => request('/admin/genre', { method: 'POST', body: JSON.stringify(data) }),
  update: (id, data) => request(`/admin/genre/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  delete: (id) => request(`/admin/genre/${id}`, { method: 'DELETE' })
},
  users: {
    getAll: () => request('/admin/users'),
    create: (data) => request('/admin/users', { method: 'POST', body: JSON.stringify(data) }),
    update: (id, data) => request(`/admin/users/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
    delete: (id) => request(`/admin/users/${id}`, { method: 'DELETE' }),
    assignRole: (data) => request('/admin/users/assign-role', { method: 'POST', body: JSON.stringify(data) })
  }
}
}