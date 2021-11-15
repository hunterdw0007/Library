document.getElementsByClassName("cartbutton").foreach().addEventListener('click', (e) => {
    var cartnumber = Number(document.getElementById('cart').innerHTML);
    cartnumber += 1;
    document.getElementById('cart').innerHTML = cartnumber;
});