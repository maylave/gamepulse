const subtractSign = document.querySelector('.btn__sign--subtract');
const addSign = document.querySelector('.btn__sign--add');
const number = document.querySelector('.btn__number');


let count = 0 
function subtract(){
    if (count > 0 )
    {
        count --;
        number.textContent = count
    }
}
function add(){
   

        count ++;
        number.textContent = count
   
}

function getStyleNumber (){
    let getStyle = window.getComputedStyle(number);
   let left = parseInt(getComputedStyle(number).left);
    let right  = parseInt(getComputedStyle(number).right);
    return {left, right};
}


function mathOnDrag(){
    let {left,right} = getStyleNumber();
    if(left <= 0) subtract();
    if(right <= 0) add();
}
function centerButton(){
    number.style.left = `1.5em`;
     number.style = `transition: left .3s ease`;
}
function dragNumber (event){
let {left} = getStyleNumber();

number.style.left = `${left + event.movementX}px`;
}
subtractSign.addEventListener('click', subtract)
addSign.addEventListener('click', add)


number.addEventListener('mousedown', (e) => {
     e.preventDefault();
    number.addEventListener('mousemove', dragNumber);
      number.style = `transition: none`;
});


number.addEventListener('mouseup', () => {
    number.removeEventListener('mousemove', dragNumber);
    centerButton();
     mathOnDrag();
});

document.addEventListener('mouseleave', () => {
    number.removeEventListener('mousemove', dragNumber);
    centerButton();
      mathOnDrag();
});