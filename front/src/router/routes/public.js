import HomeView from '@/views/HomeView.vue'
import Catalog from '@/views/Catalog.vue'
import Game from '@/views/GameView.vue'
import AuthView from '@/views/AuthView.vue'
import CartGame from '@/views/CartGame.vue'
import support from '@/views/support.vue'
import sales from '@/views/sales.vue'
import New from '@/views/new.vue'
import profileView from '@/views/ProfileView.vue'
import FavoriteCard from '../../views/FavoriteCard.vue'
import WishlistPage from '../../views/WishlistPage.vue'
export default [
  { 
    path: '/', 
    name: 'Home', 
    component: HomeView,
    meta: { requiresLoading: false } 
  },
  { 
    path: '/catalog', 
    name: 'Catalog', 
    component: Catalog,
    meta: { requiresLoading: true, loadingMessage: 'Загрузка ассортимента...' }
  },
  { 
    path: '/game/:id',    
    name: 'GameDetail',         
    component: Game,
    props: route => ({ id: Number(route.params.id) }),
    meta: { requiresLoading: true, loadingMessage: 'Запуск игры...' }
  },
  { 
    path: '/auth', 
    name: 'Auth', 
    component: AuthView,
    meta: { requiresLoading: false } 
  },
  { 
    path: '/cartGame', 
    name: 'CartGame', 
    component: CartGame,
    meta: { requiresLoading: true, loadingMessage: 'Загрузка корзины...' }
  },
  { 
    path: '/profile', 
    name: 'profile', 
    component: profileView,
    meta: { requiresLoading: true, loadingMessage: 'Загрузка корзины...' }
  },
  { 
    path: '/support', 
    name: 'support', 
    component: support,
    meta: { requiresLoading: true, loadingMessage: 'Загрузка поддержки...' }
  },
    { 
    path: '/favorite', 
    name: 'favorite', 
    component: FavoriteCard,
    meta: { requiresLoading: true, loadingMessage: 'Загрузка поддержки...' }
  },
  {
    path: '/sales',
    name: 'sales',
    component: sales,
    meta: { requiresLoading: true, loadingMessage: 'Загрузка акций...' }
  },
  {
    path: '/wishlist',
    name: 'wishlist',
    component: WishlistPage,
    meta: { requiresLoading: true, loadingMessage: 'Загрузка акций...' }
  },
  {
    path: '/new',
    name: 'new',
    component: New,
    meta: { requiresLoading: true, loadingMessage: 'Загрузка новинок...' }
  }
]