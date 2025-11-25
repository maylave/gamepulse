
import { createPinia } from 'pinia'
import { useUiStore } from './ui'

const pinia = createPinia()


export const uiStoreInstance = useUiStore(pinia)