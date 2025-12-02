const btn = document.querySelector('.btn');



btn.addEventListener('click', function() {
    this.classList.add('active');
    setTimeout(() => {
        this.classList.remove('active');
    }, 1000);
});
