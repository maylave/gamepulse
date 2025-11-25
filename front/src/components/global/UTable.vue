<template>
  <div class="base-table__container">
    <table class="base-table">
      <thead>
        <tr>
          <th
            v-for="header in headers"
            :key="header.key"
            :class="{ 'sortable': header.sortable }"
            @click="header.sortable ? sortBy(header.key) : null"
          >
            {{ header.label }}
            <span v-if="header.sortable" class="sort-indicator">
              <template v-if="sortConfig.key === header.key">
                {{ sortConfig.direction === 'asc' ? '▲' : '▼' }}
              </template>
              <template v-else>•</template>
            </span>
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(row, index) in sortedRows" :key="index">
          <td v-for="header in headers" :key="header.key">
            {{ row[header.key] }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  headers: {
    type: Array,
    required: true,
    // пример: [{ key: 'name', label: 'Имя', sortable: true }, ...]
  },
  rows: {
    type: Array,
    required: true
  }
})

const sortConfig = ref({ key: null, direction: 'asc' })

const sortedRows = computed(() => {
  const { key, direction } = sortConfig.value

  if (!key) return props.rows

  return [...props.rows].sort((a, b) => {
    const aVal = a[key]
    const bVal = b[key]

    if (typeof aVal === 'string' && typeof bVal === 'string') {
      return direction === 'asc'
        ? aVal.localeCompare(bVal, 'ru')
        : bVal.localeCompare(aVal, 'ru')
    }

    return direction === 'asc' ? aVal - bVal : bVal - aVal
  })
})

function sortBy(key) {
  if (sortConfig.value.key === key) {
    sortConfig.value.direction = sortConfig.value.direction === 'asc' ? 'desc' : 'asc'
  } else {
    sortConfig.value = { key, direction: 'asc' }
  }
}
</script>

<style lang="scss" scoped>
@use '@/assets/style/global/_variables' as *;

.base-table__container {
  overflow-x: auto;
  width: 100%;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  background: var(--color-background);
}

.base-table {
  width: 100%;
  border-collapse: collapse;
  color: var(--color-text);
  font-size: 1rem;

  thead {
    background-color: var(--color-primary);
    color: white;

    th {
      padding: 16px;
      text-align: left;
      font-weight: 600;
      cursor: default;
    }

    .sortable {
      cursor: pointer;
      user-select: none;
    }

    .sort-indicator {
      margin-left: 8px;
      font-size: 0.8em;
    }
  }

  tbody {
    tr {
      border-bottom: 1px solid var(--color-border);
      transition: background-color 0.2s;

      &:last-child {
        border-bottom: none;
      }

      &:hover {
        background-color: var(--color-hover-bg);
      }
    }

    td {
      padding: 14px 16px;
    }
  }
}


@media (max-width: 768px) {
  .base-table {
    font-size: 0.9rem;

    thead th,
    tbody td {
      padding: 12px 10px;
    }
  }
}
</style>