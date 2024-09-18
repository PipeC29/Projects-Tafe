
window.addEventListener('load', () => {

    //Find all the buttons in the page that have the add-item-button
    let addButtons = document.querySelectorAll(".add-item-button");

    addButtons.forEach((button) => {


        //Get the value attribute from the button to get the bookId
        let bookId = parseInt(button.getAttribute("value"));

        //Add a listener to the button for the click action that will trigger the
        //AddItemToCart method when it is pressed.
        button.addEventListener('click', () => addItemToCart(bookId));
    });
})

async function addItemToCart(bookId)
{
    //Send the fetch requests to add the book to the user;s cart. The book
    //Id is added as part of the URL address
    let result = await fetch("/ShoppingCart/AddToCart/" + bookId);

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