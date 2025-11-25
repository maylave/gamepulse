<!-- src/components/admin/AdminModal.vue -->
<template>
  <div class="modal-overlay" @click="emit('close')">
    <div class="modal" @click.stop>
      <h3>{{ title }}</h3>

      <div class="modal-scrollable">
        <form @submit.prevent="onSubmit">
          <div v-for="field in fields" :key="field.key" class="form-group">
            <label>{{ field.label }}</label>

            <textarea
              v-if="field.type === 'textarea'"
              v-model="formData[field.key]"
              :required="field.required"
            ></textarea>

            <div
              v-else-if="field.type === 'multiselect' && field.options"
              class="multiselect"
              @click="toggleDropdown(field.key)"
            >
              <div class="multiselect__selected">
                <span v-if="!formData[field.key] || formData[field.key].length === 0" class="placeholder">
                  Выберите {{ field.label.toLowerCase() }}
                </span>
                <div v-else class="selected-tags">
                  <span
                    v-for="opt in getSelectedOptions(field)"
                    :key="opt.value"
                    class="tag"
                  >
                    {{ opt.label }}
                  </span>
                </div>
              </div>
              <div
                v-show="openDropdown === field.key"
                class="multiselect__dropdown"
                @click.stop
              >
                <label
                  v-for="option in field.options"
                  :key="getOptionValue(option)"
                  class="multiselect__option"
                  @click="toggleOption(field.key, option)"
                >
                  <div class="option-check">
                    <span v-if="isSelected(field.key, option)" class="checkmark">✓</span>
                  </div>
                  <span class="option-label">{{ getOptionLabel(option) }}</span>
                </label>
              </div>
            </div>

            <input
              v-else
              :type="field.type"
              v-model="formData[field.key]"
              :step="field.step"
              :required="field.required"
            />
          </div>
        </form>
      </div>

     

      <div class="modal-actions">
      
        <UButton type="submit" @click="onSubmit" :loading="saving" class="btn">Сохранить</UButton>
        <UButton @click="emit('close')" variant="ghost" class="btn">Отмена</UButton>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  item: Object,
  fields: Array,
  title: String,
  saving: Boolean
})
const emit = defineEmits(['save', 'close'])

const formData = ref({})
const openDropdown = ref(null)
const fileInput = ref(null)

// Вспомогательные функции
const getOptionValue = (opt) => opt?.value !== undefined ? opt.value : opt
const getOptionLabel = (opt) => opt?.label !== undefined ? opt.label : opt

function getSelectedOptions(field) {
  const selected = formData.value[field.key] || []
  return (field.options || []).filter(opt =>
    selected.includes(getOptionValue(opt))
  )
}

function isSelected(fieldKey, option) {
  return (formData.value[fieldKey] || []).includes(getOptionValue(option))
}

function toggleOption(fieldKey, option) {
  const val = getOptionValue(option)
  const current = formData.value[fieldKey] || []
  if (current.includes(val)) {
    formData.value[fieldKey] = current.filter(v => v !== val)
  } else {
    formData.value[fieldKey] = [...current, val]
  }
}

function toggleDropdown(key) {
  openDropdown.value = openDropdown.value === key ? null : key
}


watch(() => props.item, (item) => {
  const base = {}
  props.fields.forEach(field => {
    base[field.key] = field.type === 'multiselect' ? [] : ''
  })

  if (item) {
    formData.value = { ...base, ...item }
    if (formData.value.genreIds && typeof formData.value.genreIds === 'string') {
      formData.value.genreIds = formData.value.genreIds
        .split(',')
        .map(s => parseInt(s.trim()))
        .filter(n => !isNaN(n))
    }
  } else {
    formData.value = { ...base }
  }
}, { immediate: true })



function onSubmit() {
  const data = { ...formData.value }

  // Приведение к правильным типам
  if (typeof data.isPreorder === 'string') {
    data.isPreorder = data.isPreorder.toLowerCase() === 'true'
  }


  if (data.oldPrice === '' || data.oldPrice === undefined) {
    data.oldPrice = null
  }

  if (data.releaseDate instanceof Date) {
    data.releaseDate = data.releaseDate.toISOString().split('T')[0]
  }

  console.log('Отправляемые данные:', data) 
  emit('save', data)
}
</script>

<style lang="scss" scoped>
@use '@/assets/style/global/_variables' as *;

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}



.modal {
  background: rgba(20, 20, 60, 0.2);
  backdrop-filter: blur(10px) saturate(180%);
  border: 1px solid #53535385;
  border-radius: 2rem;
  box-shadow: 0 8px 20rem rgba(30, 20, 70, 0.6);
  padding: 1.5rem;
  width: 90%;
  max-width: 500px;
  max-height: 85vh;
  display: flex;
  flex-direction: column;
  overflow: hidden;

  h3 {
    margin: 0 0 1rem;
    color: var(--color-text);
    font-size: 1.3rem;
    flex-shrink: 0; // не сжимается при скролле
  }

  .modal-scrollable {
    overflow-y: auto;
    flex: 1;
    padding-right: 8px;
    margin-right: -8px;
    display: flex;
    flex-direction: column;
   
  &::-webkit-scrollbar {
    width: 8px; 
  }

  &::-webkit-scrollbar-track {
    background: transparent;
    border-radius: 4px;
  }

  &::-webkit-scrollbar-thumb {
   background: none;
    border: 2px solid #333;
    border-radius: 4px;
    transition: background 0.2s ease;
  }

  &::-webkit-scrollbar-thumb:hover {
    background: $color-primary;
  }
  }
 
  .form-group {
    margin-bottom: 1.2rem;

    label {
      display: block;
      margin-bottom: 6px;
      font-weight: 600;
      color: var(--color-text);
      font-size: 0.95rem;
    }

    input,
    textarea {
      width: 100%;
      padding: 10px 14px;
      border: 1px solid #333;
      border-radius: 8px;
      font-family: inherit;
      background: rgba(0, 0, 0, 0.2);
      color: #dededeff;
      transition: border 0.2s ease;

      &:focus {
        outline: none;
        border-color: $color-primary;
        box-shadow: 0 0 0 2px rgba($color-primary, 0.3);
      }
    }

    textarea {
      min-height: 90px;
      resize: vertical;
    }
  }
.modal-actions {
    display: flex;
    gap: 12px;
    justify-content: flex-end;
    margin-top: 1.8rem;
    flex-shrink: 0;
  }
 
  .multiselect {
    position: relative;
    width: 100%;
    cursor: pointer;
  }

  .multiselect__selected {
    min-height: 42px;
    padding: 8px 12px;
    border: 1px solid #333;
    border-radius: 8px;
    background: rgba(0, 0, 0, 0.2);
    display: flex;
    align-items: center;
    flex-wrap: wrap;
    gap: 6px;
    transition: border 0.2s ease;

    &:hover {
      border-color: $color-primary;
    }

    .placeholder {
      color: var(--color-text-secondary);
      font-style: italic;
    }

    .selected-tags {
      display: flex;
      flex-wrap: wrap;
      gap: 6px;
    }

    .tag {
      background: rgba($color-primary, 0.2);
      color: $color-primary;
      padding: 4px 10px;
      border-radius: 20px;
      font-size: 0.85rem;
      font-weight: 500;
    }
  }

  .multiselect__dropdown {
    position: absolute;
    top: 100%;
    left: 0;
    right: 0;
    background: $color-card;
    border: 1px solid #333;
    border-radius: 8px;
    margin-top: 6px;
    z-index: 100;
    max-height: 220px;
    overflow-y: auto;
    padding: 6px 0;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.4);

    .multiselect__option {
      display: flex;
      align-items: center;
      padding: 10px 14px;
      cursor: pointer;
      color: var(--color-text);
      transition: background 0.2s ease;

      &:hover {
        background: rgba($color-primary, 0.1);
      }

      .option-check {
        width: 20px;
        height: 20px;
        border: 1px solid #555;
        border-radius: 4px;
        margin-right: 10px;
        display: flex;
        align-items: center;
        justify-content: center;
        background: transparent;

        .checkmark {
          color: $color-primary;
          font-weight: bold;
          font-size: 0.9rem;
        }
      }

      .option-label {
        font-size: 0.95rem;
      }
    }
  }

  .modal-actions {
    display: flex;
    gap: 12px;
    justify-content: flex-end;
    margin-top: 1.8rem;
  }

  .btn {
    background: none;
    border: 2px solid #333;
    color: white;
    box-shadow: 0 0 15px rgba(10, 20, 30, 0.4);
    font-size: 0.9em !important;
    border-radius: 10px !important;
    padding: 10px 18px !important;
    font-weight: 600;
    transition: all 0.2s ease;

    &:hover {
      border-color: $color-primary;
      box-shadow: 0 0 20px rgba($color-primary, 0.3);
    }
  }
}
</style>