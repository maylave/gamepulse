import { createRouter, createWebHistory } from 'vue-router'

import publicRoutes from './routes/public.js'
import adminRoutes from './routes/admin.js'
import errorRoutes from './routes/errors.js'


const routes = [...publicRoutes, ...adminRoutes, ...errorRoutes]

const router = createRouter({
  history: createWebHistory(),
  routes,
})


import { setupGlobalGuards } from './guards.js'
setupGlobalGuards(router)

export default router