<template>
  <div :class="['drag-counter', `drag-counter--${size}`]">
    <button
      type="button"
      class="drag-counter__btn drag-counter__btn--minus"
      :disabled="modelValue <= min"
      @click="handleSubtract"
      aria-label="Уменьшить количество"
    >
      −
    </button>

    <div
      ref="numberRef"
      class="drag-counter__number"
      @mousedown="onMouseDown"
    >
      {{ modelValue }}
    </div>

    <button
      type="button"
      class="drag-counter__btn drag-counter__btn--plus"
      @click="handleAdd"
      aria-label="Увеличить количество"
    >
      +
    </button>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'

const props = defineProps({
  modelValue: {
    type: Number,
    required: true,
    validator: (v) => v >= 0
  },
  min: {
    type: Number,
    default: 0
  },
 
})

const emit = defineEmits(['update:modelValue'])

const numberRef = ref(null)
let isDragging = false
let offsetX = 0

const handleSubtract = () => {
  if (props.modelValue > props.min) {
    emit('update:modelValue', props.modelValue - 1)
  }
}

const handleAdd = () => {
  emit('update:modelValue', props.modelValue + 1)
}

const onMouseDown = (e) => {
  e.preventDefault()
  isDragging = true
  offsetX = 0

  if (numberRef.value) {
    numberRef.value.style.transition = 'none'
    numberRef.value.style.transform = 'translateX(0)'
    numberRef.value.style.backgroundColor = ''
    numberRef.value.style.borderColor = 'var(--border-color, #fff)'
  }

  window.addEventListener('mousemove', onMouseMove)
  window.addEventListener('mouseup', onMouseUp)
}

const onMouseMove = (e) => {
  if (!isDragging || !numberRef.value) return

  offsetX += e.movementX
  const transformX = Math.max(-40, Math.min(40, offsetX)) // ограничение, чтобы не уходил далеко


  if (transformX < -10) {
    numberRef.value.style.backgroundColor = 'rgba(215, 66, 128, 0.4)'
    numberRef.value.style.borderColor = '#FA3889'
  } else if (transformX > 10) {
    numberRef.value.style.backgroundColor = 'rgba(32, 156, 245, 0.5)'
    numberRef.value.style.borderColor = '#209CF5'
  } else {
    numberRef.value.style.backgroundColor = ''
    numberRef.value.style.borderColor = 'var(--border-color, #fff)'
  }

  numberRef.value.style.transform = `translateX(${transformX}px)`
}

const onMouseUp = () => {
  if (!isDragging) return

  isDragging = false
  window.removeEventListener('mousemove', onMouseMove)
  window.removeEventListener('mouseup', onMouseUp)

  let newValue = props.modelValue

  if (offsetX < -20 && props.modelValue > props.min) {
    newValue = props.modelValue - 1
  } else if (offsetX > 20) {
    newValue = props.modelValue + 1
  }

  if (newValue !== props.modelValue) {
    emit('update:modelValue', newValue)
  }


  if (numberRef.value) {
    numberRef.value.style.transition = 'transform 0.3s ease, background-color 0.2s, border-color 0.2s'
    numberRef.value.style.transform = 'translateX(0)'
    numberRef.value.style.backgroundColor = ''
    numberRef.value.style.borderColor = 'var(--border-color, #fff)'
  }

  offsetX = 0
}


onMounted(() => {
  window.addEventListener('mouseleave', onMouseUp)
  if (numberRef.value) {
    numberRef.value.style.transform = 'translateX(0)'
  }
})

onUnmounted(() => {
  window.removeEventListener('mouseleave', onMouseUp)
  window.removeEventListener('mousemove', onMouseMove)
  window.removeEventListener('mouseup', onMouseUp)
})
</script>

<style scoped>
.drag-counter {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  padding: 2px;
  border-radius: 8px;
  
  user-select: none;
}

.drag-counter__btn {
  width: 28px;
  height: 28px;
  font-size: 1.3rem;
  font-weight: bold;
  border: 1px solid rgba(255, 255, 255, 0.2);
  background: rgba(255, 255, 255, 0.08);
  color: var(--color-text);
  border-radius: 6px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
}

.drag-counter__btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.drag-counter__btn:not(:disabled):hover {
  background: var(--color-primary);
  color: #000;
}

.drag-counter__number {
  min-width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 1.1rem;
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 6px;
  background: rgba(251, 251, 251, 0.3);
  color: var(--color-primary);
  cursor: grab;
  user-select: none;
  padding: 0 4px;
}






</style>