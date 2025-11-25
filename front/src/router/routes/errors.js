import NotFoundView from '@/views/NotFoundView.vue'

export default [
  { 
    path: '/:pathMatch(.*)*', 
    name: 'NotFound', 
    component: NotFoundView,
    meta: { requiresLoading: false }
  }
]