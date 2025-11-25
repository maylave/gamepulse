import AdminViewTable from '@/views/adminViewTable.vue'
import AccessDenied from '@/views/AccessDenied.vue'

export default [
  {
    path: '/table',
    name: 'table',
    component: AdminViewTable,
    meta: { 
      requiresLoading: true, 
      loadingMessage: 'Загрузка админки...', 
      requiresAdmin: true 
    }
  },
  {
    path: '/403',
    name: 'AccessDenied',
    component: AccessDenied,
    meta: { requiresLoading: false }
  }
]