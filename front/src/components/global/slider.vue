<template>
  <div class="slider-contener">
    <div class="slider-track">
      <div
        v-for="item in slides"
        
        class="slide"
       
      >
        <p>{{ item.text }}</p>
      </div>
    </div>

    <button class="nav-btn prev" @click="prevSlide">‹</button>
    <button class="nav-btn next" @click="nextSlide">›</button>

    <div class="line-scroll">
      <div
        class="line-scroll__indicator"
        :style="{
          left: `${((currentIndex / slides.length).toFixed(1)) * 100 }%`,
          width: `${100 / slides.length}%`,
          transform: 'translateX(-50%)'
        }"
      ></div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const slides = [
  { text: 'Slide 1' },
  { text: 'Slide 2' },
  { text: 'Slide 3' },
  { text: 'Slide 4' },
  { text: 'Slide 5' },
  { text: 'Slide 6' },
   { text: 'Slide 2' },
  { text: 'Slide 3' },
  { text: 'Slide 4' },
  { text: 'Slide 5' },
  { text: 'Slide 6' },
];

const currentIndex = ref(0);

const nextSlide = () => {
  currentIndex.value = (currentIndex.value + 1) % slides.length;
};

const prevSlide = () => {
  currentIndex.value = (currentIndex.value - 1 + slides.length) % slides.length;
};
</script>

<style scoped>
.slider-contener{
background-color: #2552;
}
.slider-track{
    display: grid;
    grid-auto-flow: column;
    scroll-behavior: auto;
    gap: 1.2rem;
    overflow-y: auto;
  
}


.slide{
   min-height: 300px;
   min-width: 200px;
    border-radius: 10px;
   background-color: #8888;
   padding: auto;
scroll-snap-align: start;
}


p {
  color: var(--color-text);
  margin: 0;
  font-size: 18px;
}

.nav-btn {
  position: absolute;
  
  transform: translateY(-50%);
  background: rgba(10, 10, 15, 0.7);
  color: var(--color-text);
  border: none;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.2s;
  z-index: 2;
}



.nav-btn.prev { left: 12px; }
.nav-btn.next { right: 12px; }

</style>