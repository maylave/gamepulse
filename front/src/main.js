
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import registerGlobalComponents from './components/global'
import { setupGlobalGuards } from './router/guards'

import './assets/style/global/main.scss'
const app = createApp(App)
const pinia = createPinia()
registerGlobalComponents(app)
setupGlobalGuards(router)    // ← потом гварды
app.use(router)
app.use(pinia)
app.mount('#app')