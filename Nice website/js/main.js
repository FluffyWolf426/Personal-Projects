const clickButton = document.querySelector('.toggle-btn');

clickButton.addEventListener('click', ()=> {
    document.querySelector('nav').classList.toggle('show-nav');
});