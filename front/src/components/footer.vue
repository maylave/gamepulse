<template>
  <footer class="footer">
    <div class="container">
      <!-- Десктопная версия -->
      <div v-if="!isMobile" class="footer-grid">
        <div class="footer-col">
          <h3>GamePulse</h3>
          <p>Лучший выбор игр, мгновенная доставка, надёжная поддержка 24/7.</p>
          <div class="social-icons">
            <a href="https://t.me/gamepulse" target="_blank" aria-label="Telegram"><i class="fab fa-telegram"></i></a>
            <a href="https://vk.com/gamepulse" target="_blank" aria-label="ВКонтакте"><i class="fab fa-vk"></i></a>
            <a href="https://discord.gg/gamepulse" target="_blank" aria-label="Discord"><i class="fab fa-discord"></i></a>
            <a href="https://youtube.com/@gamepulse" target="_blank" aria-label="YouTube"><i class="fab fa-youtube"></i></a>
          </div>
        </div>

        <div class="footer-col">
          <h3>Магазин</h3>
          <ul>
            <li><router-link to="/catalog">Все игры</router-link></li>
            <li><router-link to="/catalog?onSale=true">Предзаказы</router-link></li>
            <li><router-link to="/catalog?onSale=true">Акции</router-link></li>
            <li><a href="#">Подарочные карты</a></li>
          </ul>
        </div>

        <div class="footer-col">
          <h3>Поддержка</h3>
          <ul>
            <li><router-link to="/faq">FAQ</router-link></li>
            <li><router-link to="/how-to-activate">Как активировать?</router-link></li>
            <li><router-link to="/refund">Возврат средств</router-link></li>
            <li><router-link to="/contacts">Контакты</router-link></li>
          </ul>
        </div>

        <div class="footer-col">
          <h3>Юридическое</h3>
          <ul>
            <li><router-link to="/terms">Пользовательское соглашение</router-link></li>
            <li><router-link to="/privacy">Политика конфиденциальности</router-link></li>
            <li><router-link to="/offer">Оферта</router-link></li>
          </ul>
        </div>
      </div>

      <!-- Мобильная версия (аккордеон) -->
      <Accordion
        v-else
        :items="accordionItems"
        :disable-animation-on-mobile="false"
        :no-animation="false"
        max-content-height="200"
      >
        <template #content="{ item }">
          <div v-if="item.key === 'info'">
            <p>Лучший выбор игр, мгновенная доставка, надёжная поддержка 24/7.</p>
            <div class="social-icons">
              <a href="https://t.me/gamepulse" target="_blank" aria-label="Telegram"><i class="fab fa-telegram"></i></a>
              <a href="https://vk.com/gamepulse" target="_blank" aria-label="ВКонтакте"><i class="fab fa-vk"></i></a>
              <a href="https://discord.gg/gamepulse" target="_blank" aria-label="Discord"><i class="fab fa-discord"></i></a>
              <a href="https://youtube.com/@gamepulse" target="_blank" aria-label="YouTube"><i class="fab fa-youtube"></i></a>
            </div>
          </div>
          <ul v-else-if="item.links">
            <li v-for="(link, idx) in item.links" :key="idx">
              <router-link :to="link.to">{{ link.text }}</router-link>
            </li>
          </ul>
        </template>
      </Accordion>
    </div>
  </footer>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'
import Accordion from '@/components/global/Accordion.vue'

const isMobile = ref(false)

const updateIsMobile = () => {
  isMobile.value = window.innerWidth <= 768
}

onMounted(() => {
  updateIsMobile()
  window.addEventListener('resize', updateIsMobile)
})

onBeforeUnmount(() => {
  window.removeEventListener('resize', updateIsMobile)
})

const accordionItems = [
  {
    key: 'info',
    title: 'GamePulse'
  },
  {
    key: 'shop',
    title: 'Магазин',
    links: [
      { text: 'Все игры', to: '/catalog' },
      { text: 'Предзаказы', to: '/catalog?onSale=true' },
      { text: 'Акции', to: '/catalog?onSale=true' },
      { text: 'Подарочные карты', to: '#' }
    ]
  },
  {
    key: 'support',
    title: 'Поддержка',
    links: [
      { text: 'FAQ', to: '/faq' },
      { text: 'Как активировать?', to: '/how-to-activate' },
      { text: 'Возврат средств', to: '/refund' },
      { text: 'Контакты', to: '/contacts' }
    ]
  },
  {
    key: 'legal',
    title: 'Юридическое',
    links: [
      { text: 'Пользовательское соглашение', to: '/terms' },
      { text: 'Политика конфиденциальности', to: '/privacy' },
      { text: 'Оферта', to: '/offer' }
    ]
  }
]
</script>



<style lang="scss" scoped src="@/assets/style/components/footer/main.scss"></style>




