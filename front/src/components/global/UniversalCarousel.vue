<template>
  <div class="universal-carousel-wrapper">
    <div ref="carouselRef" class="universal-carousel" @mousedown="onMouseDown" @mouseleave="onMouseUp"
      @mouseup="onMouseUp" @touchstart="onTouchStart" @touchend="onTouchEnd" @touchmove="onTouchMove">
      <div ref="trackRef" class="carousel-track" :style="{
        transform: `translateX(${dragOffset + baseOffset}px)`,
        transition: isDragging ? 'none' : 'transform 0.4s ease',
      }">
        <div v-for="(item, index) in items" :key="item.id || index" class="carousel-slide" @click="handleClick(item)">
          <slot :item="item" :index="index"></slot>
        </div>
      </div>
    </div>

    <!-- Индикатор прогресса -->
    <div v-if="items.length > 1" class="carousel-indicators">
      <div v-for="(_, i) in items" :key="i" class="indicator-line"
        :class="{ 'indicator-line--active': i === baseIndex }"></div>
    </div>
  </div>
</template>
<script>

let resizeHandler = null
let mouseMoveHandler = null
let mouseUpHandler = null
let touchMoveHandler = null
let touchEndHandler = null</script>
<script setup>
import { ref, computed, onMounted, onUnmounted, nextTick } from 'vue'

const props = defineProps({
  items: { type: Array, required: true },
})

const emit = defineEmits(['change', 'click-slide'])

const carouselRef = ref(null)
const isDragging = ref(false)
const hasDragged = ref(false)
const startPos = ref({ x: 0, y: 0 }) // ← теперь сохраняем X и Y
const dragStartOffset = ref(0)
const dragOffset = ref(0)
const baseIndex = ref(0)
const isMounted = ref(true)

const handlers = {
  resize: null,
  mousemove: null,
  mouseup: null,
  touchmove: null,
  touchend: null
}

const slideWidth = computed(() => {
  if (!carouselRef.value) return 130
  const container = carouselRef.value.offsetWidth
  if (container < 480) return container * 0.8
  if (container < 768) return Math.min(130, (container - 24) / 2)
  return 130
})

const maxIndex = computed(() => props.items.length - 1)
const baseOffset = computed(() => -baseIndex.value * slideWidth.value)

const constrainedDragOffset = (offset) => {
  const minOffset = -maxIndex.value * slideWidth.value - baseOffset.value
  const maxOffset = 0 - baseOffset.value
  return Math.min(maxOffset, Math.max(minOffset, offset))
}

const onMouseDown = (e) => {
  isDragging.value = true
  hasDragged.value = false
  startPos.value = { x: e.clientX, y: e.clientY }
  dragStartOffset.value = dragOffset.value
  carouselRef.value?.classList.add('carousel--dragging')
  e.preventDefault()
}

const onMouseMove = (e) => {
  if (!isDragging.value) return

  const deltaX = e.clientX - startPos.value.x
  const deltaY = e.clientY - startPos.value.y


  if (Math.abs(deltaY) > Math.abs(deltaX)) {
    isDragging.value = false
    return
  }

  if (Math.abs(deltaX) > 5) hasDragged.value = true
  dragOffset.value = constrainedDragOffset(deltaX)
}


const onTouchStart = (e) => {
  isDragging.value = true
  hasDragged.value = false
  startPos.value = { x: e.touches[0].clientX, y: e.touches[0].clientY }
  dragStartOffset.value = dragOffset.value
  carouselRef.value?.classList.add('carousel--dragging')

}

const onTouchMove = (e) => {
  if (!isDragging.value) return

  const currentX = e.touches[0].clientX
  const currentY = e.touches[0].clientY
  const deltaX = currentX - startPos.value.x
  const deltaY = currentY - startPos.value.y

  // Определяем основное направление
  if (Math.abs(deltaY) > Math.abs(deltaX)) {
    // Вертикальный свайп → отпускаем карусель, позволяем скролл
    isDragging.value = false
    return
  }

  // Горизонтальный свайп → управляем каруселью
  if (Math.abs(deltaX) > 5) hasDragged.value = true
  dragOffset.value = constrainedDragOffset(deltaX)
  e.preventDefault() // блокируем скролл ТОЛЬКО здесь
}

const onMouseUp = () => finalizeDrag()
const onTouchEnd = () => finalizeDrag()

const finalizeDrag = () => {
  if (!isDragging.value) return
  isDragging.value = false

  const totalOffset = dragOffset.value + baseOffset.value
  const targetIndex = Math.round(-totalOffset / slideWidth.value)
  baseIndex.value = Math.max(0, Math.min(maxIndex.value, targetIndex))
  dragOffset.value = 0

  emit('change', baseIndex.value)
  carouselRef.value?.classList.remove('carousel--dragging')
}

const handleClick = (item) => {
  if (!hasDragged.value) emit('click-slide', item)
}

// === Обработчики событий ===
onMounted(() => {
  isMounted.value = true

  handlers.resize = () => { if (isMounted.value) nextTick() }
  handlers.mousemove = onMouseMove
  handlers.mouseup = onMouseUp
  handlers.touchmove = onTouchMove
  handlers.touchend = onTouchEnd

  window.addEventListener('resize', handlers.resize)
  document.addEventListener('mousemove', handlers.mousemove)
  document.addEventListener('mouseup', handlers.mouseup)
  document.addEventListener('touchmove', handlers.touchmove, { passive: false })
  document.addEventListener('touchend', handlers.touchend)
})

onUnmounted(() => {
  isMounted.value = false
  window.removeEventListener('resize', handlers.resize)
  document.removeEventListener('mousemove', handlers.mousemove)
  document.removeEventListener('mouseup', handlers.mouseup)
  document.removeEventListener('touchmove', handlers.touchmove)
  document.removeEventListener('touchend', handlers.touchend)
})
const prev = () => {
  if (baseIndex.value > 0) {
    baseIndex.value--
    dragOffset.value = 0
    emit('change', baseIndex.value)
  }
}

const next = () => {
  if (baseIndex.value < maxIndex.value) {
    baseIndex.value++
    dragOffset.value = 0
    emit('change', baseIndex.value)
  }
}

// Экспонируем методы наружу
defineExpose({
  prev,
  next
})
</script>

<style scoped>
.universal-carousel-wrapper {
  position: relative;
  width: 100%;
}

.universal-carousel {
  width: 100%;
  overflow: hidden;
  user-select: none;
  -webkit-user-select: none;
  margin-bottom: 1.25rem;

  
}

.carousel-track {
  display: flex;
  gap: 1.5rem;
  padding: 0 0.75rem;
  min-width: min-content;
  box-sizing: border-box;
}

.carousel-slide {
  flex: none;
  width: 130px;
  max-width: 100%;
  box-sizing: border-box;
}


.carousel-indicators {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 0.5rem;
  padding: 0 0.5rem;
}

.indicator-line {
  width: 24px;
  height: 4px;
  border-radius: 2px;
  background-color: var(--indicator-inactive, #ddd);
  transition: background-color 0.3s ease;
}

.indicator-line--active {
  background-color: var(--color-primary, #6d35e5);
}


@media (min-width: 1024px) {
  .carousel-indicators {
    display: none;
  }
}
</style>