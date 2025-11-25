<template>
  <div class="not-found">
    <div class="container">
      <div class="error-content">
        <h1 class="error-code">403</h1>
        <h2 class="error-title">Доступ запрещён</h2>
        <p class="error-message">
         У вас недостаточно прав для просмотра этой страницы.
        </p>
        <div class="error-actions">
          <router-link to="/" 
          >
          <UButton class="btn"> Вернуться на главную</UButton>
           
          </router-link>
         
        </div>
      </div>
    </div>
  </div>
</template>



<style lang="scss" scoped>
@use '@/assets/style/global/_variables' as *;

.not-found {
  min-height: 100vh;
  display: flex;
  align-items: center;
  padding: 2rem 0;
  background-color: $color-bg;
}

.container {
  width: 90%;
  max-width: 800px;
  margin: 0 auto;
  text-align: center;

}

.error-content {
  background: $color-card;
  border-radius: 20px;
  padding: 3rem;
  box-shadow: 0 20px 50px rgba(0, 0, 0, 0.5);
  position: relative;
  overflow: hidden;
  

 
}



.error-code {
  font-size: 8rem;
  font-weight: 800;
  margin-bottom: 1rem;
  background: linear-gradient(90deg, $color-primary, $color-secondary);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  line-height: 1;
}

.error-title {
  font-size: 2.5rem;
  margin-bottom: 1.5rem;
  color: $color-text;
}

.error-message {
  font-size: 1.2rem;
  color: $color-text-secondary;
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
  
z-index:10;

}


// Мобильная адаптация
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

  
}
</style>