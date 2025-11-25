<template>
  <footer class="footer">
    <div class="container">
     
      <div v-if="!isMobile" class="footer-grid">
        <div class="footer-col">
          <h3>GamePulse</h3>
          <p>Лучший выбор игр, мгновенная доставка, надёжная поддержка 24/7.</p>
          <div class="social-icons">
            <a href="#" aria-label="Telegram"><i class="fab fa-telegram"></i></a>
            <a href="#" aria-label="ВКонтакте"><i class="fab fa-vk"></i></a>
            <a href="#" aria-label="Discord"><i class="fab fa-discord"></i></a>
            <a href="#" aria-label="YouTube"><i class="fab fa-youtube"></i></a>
          </div>
        </div>
        <div class="footer-col">
          <h3>Магазин</h3>
          <ul>
            <li><a href="#">Все игры</a></li>
            <li><a href="#">Предзаказы</a></li>
            <li><a href="#">Акции</a></li>
            <li><a href="#">Подарочные карты</a></li>
          </ul>
        </div>
        <div class="footer-col">
          <h3>Поддержка</h3>
          <ul>
            <li><a href="#">FAQ</a></li>
            <li><a href="#">Как активировать?</a></li>
            <li><a href="#">Возврат средств</a></li>
            <li><a href="#">Контакты</a></li>
          </ul>
        </div>
        <div class="footer-col">
          <h3>Юридическое</h3>
          <ul>
            <li><a href="#">Пользовательское соглашение</a></li>
            <li><a href="#">Политика конфиденциальности</a></li>
            <li><a href="#">Оферта</a></li>
          </ul>
        </div>
      </div>


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
              <a href="#" aria-label="Telegram"><i class="fab fa-telegram"></i></a>
              <a href="#" aria-label="ВКонтакте"><i class="fab fa-vk"></i></a>
              <a href="#" aria-label="Discord"><i class="fab fa-discord"></i></a>
              <a href="#" aria-label="YouTube"><i class="fab fa-youtube"></i></a>
            </div>
          </div>
          <ul v-else-if="item.links">
            <li v-for="(link, idx) in item.links" :key="idx">
              <a :href="link.href">{{ link.text }}</a>
            </li>
          </ul>
        </template>
      </Accordion>

  
    </div>
  </footer>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'

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
    title: 'GamePulse',
   
  },
  {
    key: 'shop',
    title: 'Магазин',
    links: [
      { text: 'Все игры', href: '#' },
      { text: 'Предзаказы', href: '#' },
      { text: 'Акции', href: '#' },
      { text: 'Подарочные карты', href: '#' }
    ]
  },
  {
    key: 'support',
    title: 'Поддержка',
    links: [
      { text: 'FAQ', href: '#' },
      { text: 'Как активировать?', href: '#' },
      { text: 'Возврат средств', href: '#' },
      { text: 'Контакты', href: '#' }
    ]
  },
  {
    key: 'legal',
    title: 'Юридическое',
    links: [
      { text: 'Пользовательское соглашение', href: '#' },
      { text: 'Политика конфиденциальности', href: '#' },
      { text: 'Оферта', href: '#' }
    ]
  }
]
</script>

<style lang="scss" scoped src="@/assets/style/components/footer/main.scss"></style>




