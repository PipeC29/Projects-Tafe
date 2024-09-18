
window.addEventListener('load', () => {

    //Find all the buttons in the page that have the add-item-button
    let addButtons = document.querySelectorAll(".add-item-button");

    addButtons.forEach((button) => {


        //Get the value attribute from the button to get the itemId
        let itemId = parseInt(button.getAttribute("value"));

        //Add a listener to the button for the click action that will trigger the
        //AddItemToCart method when it is pressed.
        button.addEventListener('click', () => addItemToCart(itemId));
    });
})

async function addItemToCart(itemId)
{
    //Send the fetch requests to add the product to the user;s cart. The product
    //Id is added as part of the URL address
    let result = await fetch("/ShoppingCart/AddToCart/" + itemId);

    //If the results is an unathorised message, redirect the user to the login page.
    if (result.status == 401)
    {

        location.href = "Login/Login";
        return;
    }

    //If the user message is anything other than woo show an error message
    if (result.status != 200)
    {
        alert("Add to cart failed")
        return;
    }
    //Refresh the cart modal. Even though this method is in a different file, we can
    //still call it here becouse it is in the _layout.cshtml which means it is available
    //wihtin all of the views
    showCartModal();

    
}

const colors = [
    "#ffffff",
    "#000000",
    "#ff0000",
    "#00ff00",
    "#0000ff",
    "#ffff00",
    "#ff00ff",
    "#00ffff",
];

function changeCardColor(cardElement) {
    const index = Math.floor(Math.random() * colors.length);
    cardElement.style.backgroundColor = colors[index];
}
