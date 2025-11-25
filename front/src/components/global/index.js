import UButton from './UButton.vue'
import LoadingSpinner from './LoadingSpinner.vue'
import Accordion from './Accordion.vue'
import slider from './slider.vue'
import UniversalCarousel from './UniversalCarousel.vue'
import UTable from './UTable.vue'
const globalComponents = {
  UButton,
  LoadingSpinner,
  Accordion,
  slider,
  UniversalCarousel,
  UTable
}

export default function registerGlobalComponents(app) {
  Object.entries(globalComponents).forEach(([name, component]) => {
    app.component(name, component)
  })
}