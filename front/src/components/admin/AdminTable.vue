<!-- src/components/admin/AdminTable.vue -->
<template>
  <div class="admin-table-container">
    <table v-if="!loading" class="admin-table">
      <thead>
        <tr>
          <th v-for="header in headers" :key="header.key">{{ header.label }}</th>
          <th>Действия</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="row in rows" :key="row.id">
          <td v-for="header in headers" :key="header.key">
            {{ typeof row[header.key] === 'object' ? JSON.stringify(row[header.key]) : row[header.key] }}
          </td>
          <td class="actions">
            <UButton size="xs" @click="$emit('edit', row)" class="btn"><i class="fas fa-edit" ></i>  </UButton>
            <UButton size="xs" @click="$emit('delete', row.id)" class="btn danger"><i class="fas fa-remove" ></i> </UButton>
          </td>
        </tr>
      </tbody>
    </table>
    <div v-else class="loading">Загрузка...</div>
  </div>
</template>

<script setup>

defineProps({
  headers: Array,
  rows: Array,
  loading: Boolean
})
defineEmits(['edit', 'delete'])
</script>

<style lang="scss" scoped>
@use '@/assets/style/global/_variables' as *;

.admin-table-container {
  overflow-x: auto;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
}

.admin-table {
  width: 100%;
  border-collapse: collapse;


  th, td {
    padding: 12px 16px;
    text-align: left;
    border-bottom: 1px solid var($color-secondary);
  }

  th {
    background: rgba($color-card, 0.8);
    color: $color-text;
  
    font-weight: 600;
    backdrop-filter: blur(2px);

  box-shadow: 0 0px 32px rgba(255, 255, 255, 0.4);
  }

  .actions {
    white-space: nowrap;
  }

  .btn-action {
    margin-right: 6px;
    padding: 4px 8px !important;
    font-size: 0.9em !important;
  }
  .btn {
    background: none;
    margin-right: 10px;
    border: 2px solid #333;
    color: white;
    box-shadow: 0 0 15px rgba(10, 20, 30, 0.4);
    font-size: 0.9em !important;
    border-radius: 15px !important;
    padding: 10px 18px !important;
    font-weight: 600;
    transition: all 0.2s ease;

    &:hover {
      border-color: $color-primary;
      box-shadow: 0 0 20px rgba($color-primary, 0.3);
    }
  }

}

.loading {
  padding: 2rem;
  text-align: center;
  color: var(--color-text);
}
</style>