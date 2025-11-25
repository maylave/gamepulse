<template>
 <section class="hero">
    <h1>Распродажа «Летний взрыв» — скидки до 85%!</h1>
    <p>Более 500 игр со скидками. Только до конца недели. Не упусти шанс!</p>
    <div class="countdown" id="countdown">
      <div class="countdown-item">
        <div class="countdown-value" >{{countdown.days}}</div>
        <div class="countdown-label">Дней</div>
      </div>
      <div class="countdown-item">
        <div class="countdown-value" >{{countdown.hours}}</div>
        <div class="countdown-label">Часов</div>
      </div>
      <div class="countdown-item">
        <div class="countdown-value" >{{countdown.minutes}}</div>
        <div class="countdown-label">Минут</div>
      </div>
    </div>
    <UButton >Перейти к акциям</UButton>
  
  </section>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
const countdown = ref({
  days: '00',
  hours: '00',
  minutes: '00'
})

const updateCountdown = () => {
  const now = new Date()
  const target = new Date('2025-12-15T12:00:00')
  const diff = target - now

  if (diff > 0) {
    const days = Math.floor(diff / (1000 * 60 * 60 * 24))
    const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60))

    countdown.value = {
      days: String(days).padStart(2, '0'),
      hours: String(hours).padStart(2, '0'),
      minutes: String(minutes).padStart(2, '0')
    }
  }
}

onMounted(() => {
  updateCountdown()
  setInterval(updateCountdown, 60000)
})
</script>

<style lang="scss" scoped src="@/assets/style/components/hero/main.scss"></style>