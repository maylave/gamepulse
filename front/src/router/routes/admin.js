import AdminViewTable from '@/views/adminViewTable.vue'
import AccessDenied from '@/views/AccessDenied.vue'
import SupportAdminPage from '../../views/SupportAdminPage.vue'
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
    path: '/supportAdmin',
    name: 'supportAdmin',
    component:  SupportAdminPage,
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