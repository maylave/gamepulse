<!-- src/components/admin/AdminSidebar.vue -->
<template>
  <aside class="admin-sidebar">
    <nav>
      <button
        v-for="item in menuItems"
        :key="item.key"
        @click="$emit('update:active', item.key)"
        :class="{ active: modelValue === item.key }"
      >
        {{ item.label }}
      </button>
    </nav>
  </aside>
</template>

<script setup>
const props = defineProps({
  modelValue: { type: String, required: true }
})
const emit = defineEmits(['update:active'])

const menuItems = [
  { key: 'users', label: 'Пользователи' },
  { key: 'games', label: 'Игры' },
  {key: 'genres',label: 'genres'}
]
</script>

<style lang="scss" scoped>
@use '@/assets/style/global/_variables' as *;

.admin-sidebar {
  width: 220px;
 background: rgba(#131314FF, 0.9);
  border-right: 1px solid var($color-accent-dark);
  padding: 1.5rem 0;

  nav button {
    display: block;
    width: 100%;
    padding: 12px 20px;
    text-align: left;
    background: none;
    border: none;
    color: var(--color-text);
    font-size: 1rem;
    cursor: pointer;
    transition: background 0.2s;

    &:hover,
    &.active {
      background: rgba($color-primary, 0.15);
      color: $color-primary;
    }
    .active{
        border:1px solid black;
           background: rgba($color-primary, 0.15);
              border-left: 3px solid $color-primary; /* ← красивый акцент слева */
    }
  }
}
</style>