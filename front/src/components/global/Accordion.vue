<template>
  <div class="accordion" :class="{ 'no-animation': !animated }">
    <div
      v-for="(item, index) in items"
      :key="item.id || index"
      class="accordion-item"
      :class="{ active: activeIndex === index }"
    >
    
      <div class="accordion-header" @click="toggle(index)">
        <slot name="header" :item="item" :index="index">
          {{ item.title }}
        </slot>
        <span class="accordion-icon">▼</span>
      </div>

    
      <div class="accordion-content">
        <slot name="content" :item="item" :index="index">
          {{ item.content }}
        </slot>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'

const props = defineProps({

  items: {
    type: Array,
    required: true,
    default: () => []
  },
 
  noAnimation: {
    type: Boolean,
    default: false
  },

  disableAnimationOnMobile: {
    type: Boolean,
    default: true
  },

  maxContentHeight: {
    type: [String, Number],
    default: 300
  }
})

const emit = defineEmits(['update:active', 'change'])

const activeIndex = ref(null)


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


const animated = computed(() => {
  if (props.noAnimation) return false
  if (props.disableAnimationOnMobile && isMobile.value) return false
  if (window.matchMedia?.('(prefers-reduced-motion: reduce)').matches) return false
  return true
})

function toggle(index) {
  const newIndex = activeIndex.value === index ? null : index
  activeIndex.value = newIndex
  emit('change', newIndex)
  emit('update:active', newIndex)
}
</script>

<style lang="scss" scoped>
@use '@/assets/style/global/_variables' as *;

.accordion {
  width: 100%;
  
}

.accordion-item {
  border-bottom: 1px solid $color-card;
  margin-bottom: 0.5rem;
}

.accordion-header {
  border-top: $color-card solid 1px;
  color: $color-text;
  padding: 1.2rem 1.5rem;
  cursor: pointer;
  font-weight: 600;
  display: flex;
  justify-content: space-between;
  align-items: center;
  transition: background 0.2s;

  @media (hover: hover) and (pointer: fine) {
    &:hover {
      background:$color-card;
    }
  }
}

.accordion-icon {
  transition: transform 0.3s;
}

.accordion-content {
  
  color: $color-text;
  padding: 0 1.5rem;
  max-height: 0;
  overflow: hidden;
  transition: max-height 0.3s ease, padding 0.3s ease;
}

.accordion-item.active {
  .accordion-content {
    padding: 1.5rem;
    max-height: v-bind('props.maxContentHeight + "px"');
  }

  .accordion-icon {
    transform: rotate(180deg);
  }
}


.accordion.no-animation .accordion-content {
  transition: none !important;
}

.accordion.no-animation .accordion-item:not(.active) .accordion-content {
  max-height: 0 !important;
}

.accordion.no-animation .accordion-item.active .accordion-content {
  max-height: none !important;
}
</style>