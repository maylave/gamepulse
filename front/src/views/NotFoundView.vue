<template>
  <div class="not-found">
    <div class="container">
      <div class="error-content">
        <h1 class="error-code">404</h1>
        <h2 class="error-title">Страница не найдена</h2>
        <p class="error-message">
          К сожалению, запрашиваемая страница не существует.<br>
          Возможно, она была удалена или перемещена.
        </p>
        <div class="error-actions">
          <router-link to="/" class="btn btn-primary">
            Вернуться на главную
          </router-link>
          <router-link to="/catalog" class="btn btn-secondary">
            Перейти в каталог
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
</script>

<style scoped>
.not-found {
  background: linear-gradient(135deg, var(--color-bg), #0a0a10);
  min-height: 100vh;
  display: flex;
  align-items: center;
  padding: 2rem 0;
}

.container {
  width: 90%;
  max-width: 800px;
  margin: 0 auto;
  text-align: center;
}

.error-content {
  background: var(--color-card);
  border-radius: 20px;
  padding: 3rem;
  box-shadow: 0 20px 50px rgba(0, 0, 0, 0.5);
  position: relative;
  overflow: hidden;
}

.error-content::before {
  content: '';
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: linear-gradient(45deg, transparent, var(--color-secondary), var(--color-primary), transparent);
  animation: rotate 8s linear infinite;
  z-index: 0;
  opacity: 0.1;
}

@keyframes rotate {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error-code {
  font-size: 8rem;
  font-weight: 800;
  margin-bottom: 1rem;
  background: linear-gradient(90deg, var(--color-primary), var(--color-secondary));
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  line-height: 1;
}

.error-title {
  font-size: 2.5rem;
  margin-bottom: 1.5rem;
  color: var(--color-text);
}

.error-message {
  font-size: 1.2rem;
  color: var(--color-text-secondary);
  margin-bottom: 2.5rem;
  line-height: 1.6;
}

.error-actions {
  display: flex;
  gap: 1.5rem;
  justify-content: center;
  flex-wrap: wrap;
}

.btn {
  padding: 0.9rem 2rem;
  border-radius: 12px;
  font-weight: 600;
  font-size: 1.1rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.8rem;
   z-index: 10;
  transition: all 0.3s ease;
}

.btn-primary {
  background: linear-gradient(90deg, var(--color-secondary), var(--color-primary));
  color: #000;
  border: none;
}

.btn-secondary {
  background: rgba(255, 255, 255, 0.05);
  color: var(--color-text);
  border: 1px solid #333;
}

.btn:hover {
  opacity: 0.9;
  transform: translateY(-3px);
}

@media (max-width: 600px) {
  .error-code {
    font-size: 6rem;
  }
  
  .error-title {
    font-size: 2rem;
  }
  
  .error-actions {
    gap: 1rem;
  }
  
  .btn {
    padding: 0.8rem 1.5rem;
    font-size: 1rem;
  }
}
</style>