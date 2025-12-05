import AdminViewTable from '@/views/adminViewTable.vue'
import AccessDenied from '@/views/AccessDenied.vue'
import SupportAdminPage from '@/views/SupportAdminPage.vue' 
import SuperUser from '../../views/SuperUser.vue'
export default [
  {
    path: '/table',
    name: 'table',
    component: AdminViewTable,
    meta: { 
      requiresLoading: true, 
      loadingMessage: 'Загрузка админки...', 
      requiresAdminPanel: true 
    }
  },
  {
    path: '/supportAdmin',
    name: 'supportAdmin',
    component: SupportAdminPage,
    meta: { 
      requiresLoading: true, 
      loadingMessage: 'Загрузка админки...', 
      requiresAdminPanel: true 
    }
  },
  {
    path: '/add-game',
    name: 'AddGame',
    component: SuperUser,
    meta: { 
      requiresAddGame: true, 
      requiresLoading: false 
    }
  },
  {
    path: '/403',
    name: 'AccessDenied',
    component: AccessDenied,
    meta: { requiresLoading: false }
  }
]