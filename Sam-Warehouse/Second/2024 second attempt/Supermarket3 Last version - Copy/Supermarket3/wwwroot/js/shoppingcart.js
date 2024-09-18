//Add a listener to the broswer window which will run the enclosed code once it finishes loading.
window.addEventListener('load',()=> {
    //Find the cart modal button and add a listener which will open the method shown when
    //the button is pressed.
    document.getElementById("btnCartModal").addEventListener('click', () => showCartModal());
});

//The method triggered by the modal button event listener.
async function showCartModal()
{
    //Send a request to the endpoint provided inside the fetch request and store the response
    let result = await fetch("/ShoppingCart/Index");
    //Check if the request was successful or not. It will not be 
    //successful if the user is not logged in.
    if (result.status != 200)
    {
        //Redirect to the login controller and run the login endpoint method.
        location.href = "/Login/Login";
    }
    //Retrieve the HTML content from the body of the fetch request result.
    //This will be the details of the shopping cart partial view.
    let htmlResult = await result.text();
    //Find the cart modal body by using its ID and put the partial view html
    //between its html tags.
    document.getElementById("cartModalBody").innerHTML = htmlResult;

    //Searches through the HTML of the page and finds all the item-qty-form objects.
    let itemQtyForms = document.querySelectorAll(".item-qty-form");

    //Cycle through the collection of forms. For each form and an event listener
    //that will trigger on the form submit action to tell the form not to do its default action.
    //This means the form will not auto-submit or close the modal/window it is in.
    itemQtyForms.forEach((form) =>
    {
        form.addEventListener('submit', (e) => {
            e.preventDefault();
        })
    });


    setupQuantityButtons();
    setupRemoveButtons();
    //Calculate and display the current total cost of the cart
    calculateCartTotal();

    //Use the query selector to find the checkout button by using its ID.
    let checkoutButton = document.querySelector("#btnCheckout")
    let clearCartButton = document.querySelector("#btnClearCart")
    //Check if the button was found. This will fail if the modal has no valid cart showing.
    if (checkoutButton != null) {
        //Add alistener to the button to trigger the finalise cart method.
        checkoutButton.addEventListener('click', (e) => finaliseCart(e))
    }

    if (clearCartButton != null) {
        //Add alistener to the button to trigger the finalise cart method.
        clearCartButton.addEventListener('click', (e) => clearCart(e))
    }


    //Use JQUERY to find the modal by its ID (dont forget to use the # symbol before the ID name)
    //Then run the modal-show command on the modal to make it visible.
    $('#cartModal').modal('show');
}

async function finaliseCart(e) {
    if (confirm("Complete Checkout?") == true) {
        //Get the cart ID our of the value attribute of the button
        let id = parseInt(e.target.getAttribute("value"))
        //Send a fetch request to the controller using the ID and store the response
        let result = await fetch("/ShoppingCart/FinaliseCart/" + id);
        //Throw an error alert if the fetch request fails.
        if (result.status != 200) {
            alert("Something went wrong! Unable to finalise cart.")
        }
        else {
            $('#cartModal').modal('hide');
        }
    }
    else {
        return;
    }
}

async function clearCart(e) {
    if (confirm("Clear Cart?") == true) {
        //Get the cart ID our of the value attribute of the button
        let id = parseInt(e.target.getAttribute("value"))
        //Send a fetch request to the controller using the ID and store the response
        let result = await fetch("/ShoppingCart/ClearCart/" + id);
        //Throw an error alert if the fetch request fails.
        if (result.status != 200) {
            alert("Something went wrong! Unable to clear cart.")
        }
        else {
            $('#cartModal').modal('hide');
        }
    }
    else {
        return;
    }
}


async function setupRemoveButtons()
{
    //Searches through the HTML of the page and finds all the objects with the
    //remove-button class on them
    let removeButtons = document.querySelectorAll(".remove-button");

    //Cycle through each button and add a listener to each one that will trigeer the method
    //called remove Item
    removeButtons.forEach((button) => {

        //Get the value attribute from the button tags and store its value. this will be
        //the primary key we stored when setting the buttons up.
        let cartItemId = parseInt(button.getAttribute("value"));
        button.addEventListener("click", () => removeItem(cartItemId));
    });
}

async function removeItem(cartItemId)
{

    //Create a JSON object to hold the cart ID
    let cartItem = {
        Id: cartItemId
    };

    //Send a fetch request to the controller to request the item to be removed from the database
    let result = await fetch('/ShoppingCart/RemoveFromCart', {
        method: "DELETE",
        headers: { "content-type": "application/json" },
        body: JSON.stringify(cartItem)

    });

    //If request fails, alert the user with an error message
    if (result.status != 200) {
        alert("Remove failed")
        return;
    }
    //Refresh the cart modal
    showCartModal();

}

async function setupQuantityButtons()
{

    //Searches through the HTML of the page and finds all the objects with the
    //minurs-button class on them
    let minusButtons = document.querySelectorAll(".minus-button");

    //Cycle through each button and add a listener to each one that will trigeer the method
    //called decreaseQuantity
    minusButtons.forEach((button) => {
        button.addEventListener("click", (e) => decreaseQuantity(e));
    });

    //Searches through the HTML of the page and finds all the objects with the
    //plus-button class on them
    let plusButtons = document.querySelectorAll(".plus-button");

    //Cycle through each button and add a listener to each one that will trigeer the method
    //called increaseQuantity
    plusButtons.forEach((button) => {
        button.addEventListener("click", (e) => increaseQuantity(e));
    });
}


async function increaseQuantity(e)
{

    //Get the target of the event. Find the target's parent from the run the query
    //selector on the form to find the first input element and get its value.
    let carItemId = parseInt(e.target.form.querySelector("input").value);

    //Do the same as above except find the item in the form using the .qty class and grab its
    //innerText which is the text between the tags of the element.
    let qty = parseInt(e.target.form.querySelector(".qty").innerText);

    //Increase the quantity and update the qty field in the form with the new value
    qty++;
    e.target.form.querySelector(".qty").innerText = qty;


        //These lines first use the query selector to get the lineItem field from modified
        //the form using its class to find it.
        //Then it retrieves the single unit price from the field and stores it as a variable.
        //This requires a parse to float operation to convert it from text to a
        // number.
        //Finally it replaces the current content of the lineItem element with the new
        //price using the same format as it’s initial setup.
        //The toPrecision command at the end of the calculation is used to keep
        //the price accurate and to 2 decimal places.This is because floating
        //point numbers in JavaScript can be very inaccurate when performing
        //multiplications on them.
        //Once this is done to both methods in the correct section, your total fields in
        //each shopping cart item should change every time you press the plus or
        //minus button.

    let lineItem = e.target.form.querySelector(".lineTotal");
    let unitPrice = parseFloat(lineItem.getAttribute("value"));
    lineItem.innerText = "Total: " + (qty * unitPrice).toPrecision(4);
    calculateCartTotal();

    //Pass the new qty and cart Item ID to the method that will request the controller to update the
    //value in the database.
    updateQuantity(qty, carItemId);

}

async function decreaseQuantity(e) {

    //Get the target of the event. Find the target's parent from the run the query
    //selector on the form to find the first input element and get its value.
    let carItemId = parseInt(e.target.form.querySelector("input").value);

    //Do the same as above except find the item in the form using the .qty class and grab its
    //innerText which is the text between the tags of the element.
    let qty = parseInt(e.target.form.querySelector(".qty").innerText);

    //If the quantity is already at 1, dont decrease it. We will remove it instead.
    if (qty == 1)
    {
        return;
    }

    //Decrease the quantity and update the qty field in the form with the new value
    qty--;
    e.target.form.querySelector(".qty").innerText = qty;

    //These lines first use the query selector to get the lineItem field from modified
        //the form using its class to find it.
        //Then it retrieves the single unit price from the field and stores it as a variable.
        //This requires a parse to float operation to convert it from text to a
        // number.
        //Finally it replaces the current content of the lineItem element with the new
        //price using the same format as it’s initial setup.
        //The toPrecision command at the end of the calculation is used to keep
        //the price accurate and to 2 decimal places.This is because floating
        //point numbers in JavaScript can be very inaccurate when performing
        //multiplications on them.
        //Once this is done to both methods in the correct section, your total fields in
        //each shopping cart item should change every time you press the plus or
        //minus button.

    let lineItem = e.target.form.querySelector(".lineTotal");
    let unitPrice = parseFloat(lineItem.getAttribute("value"));
    lineItem.innerText = "Total: " + (qty * unitPrice).toPrecision(4);


    calculateCartTotal();

    //Pass the new qty and cart Item ID to the method that will request the controller to update the
    //value in the database.
    updateQuantity(qty, carItemId);

}

async function updateQuantity(qty, carItemId)
{
    //Create a Javascript object to hold the carItemId and update queantity. The fields of
    //this object need to match the shoppingCartItem model of our project.
    let updatedItem = {
        Quantity: qty,
        Id: carItemId
    }

    //Send a fetch request to the Shopping Cart controller to run the Update Quantity
    //endpoint. The request will be a PUT  (update) method and will pass the updateItem
    //details in the request body.
    let response = await fetch("/ShoppingCart/updateQuantity",
        {
            method: "PUT",
            headers: { "Content-type": "application/json" },
                body: JSON.stringify(updatedItem)
        });

    //If the request fails and send an error response message. Log the error and reset the modal.
    if (response.status != 200) {
        console.log("Update Quantity Failed")
        showCartModal();
    }
}

async function calculateCartTotal()
{
    //Get the components from each of the shopping cart cards that holds a total price
    let lineItems = document.querySelectorAll(".lineTotal");

    //Create a variable for the total price and default it to $0.00
    let total = 0.00;

    lineItems.forEach((item) => {
        //Get the total price from the line item. This requires splitting the string to
        //remove the "Total" section
        let linePrice = parseFloat(item.innerText.split(":")[1])
        total += linePrice
    });

    //Find the field for the cart grand total by using its ID
    //Then update the total show the calculated total of all the values.
    document.querySelector("#modalGrandTotal").innerText = "Cart Total: $" + total.toPrecision(4);


}


