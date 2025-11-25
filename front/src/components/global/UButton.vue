<template>
  <button
    class="base-button"
    :class="{
      'base-button--animated': animated,
      'base-button--block': block,
      'base-button--disabled': disabled,
      [`base-button--${size}`]: size
    }"
    :disabled="disabled"
    @click="$emit('click', $event)"
  >
    <slot />
  </button>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  disabled: { type: Boolean, default: false },
  block: { type: Boolean, default: false },
  noAnimation: { type: Boolean, default: false },

  size: {
    type: String,
    default: 'md',
    validator: (value) => ['sm', 'md', 'lg'].includes(value)
  },
  disableAnimationOnMobile: {
    type: Boolean,
    default: true 
  }
})

defineEmits(['click'])


const isMobile = window.innerWidth <= 768


const animated = computed(() => {
  const prefersReducedMotion = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches ?? false
  const shouldDisableOnMobile = props.disableAnimationOnMobile && isMobile
  return !props.noAnimation && !prefersReducedMotion && !shouldDisableOnMobile
})
</script>

<style lang="scss" scoped>
@use '@/assets/style/global/_variables' as *;

.base-button {
  background: linear-gradient(90deg, $color-secondary, $color-primary);
  color: #000;
  border: none;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  border-radius: 30px;
  font-size: 1rem;
  padding: 0.7rem 1.6rem;

  
  &--sm {
    font-size: 0.875rem;
    padding: 0.5rem 1.2rem;
  }

  &--md {
    font-size: 1rem;
    padding: 0.7rem 1.6rem;
  }

  &--lg {
    font-size: 1.125rem;
    padding: 0.85rem 2rem;
  }

  &:hover:not(:disabled) {
    opacity: 0.9;
    transform: translateY(var(--ty, -2px));
  }

  &--animated {
    --ty: -2px;
  }

  &--block {
    width: 100%;
  }

  &:disabled,
  &--disabled {
    opacity: 0.5;
    cursor: not-allowed;
    transform: none !important;
  }
}
</style>