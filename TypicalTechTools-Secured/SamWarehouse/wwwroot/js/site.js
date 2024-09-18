// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Add an event listener to the window that will run once it finished loading.
window.addEventListener("load", () => {
    //Find the element called btnTheme and add a click event listener to it.
    document.getElementById("btnTheme").addEventListener('click', () => switchTheme());

    //Find the element called btnCreateAuthor and add a click event listener to it to trigger the method provided.
    document.getElementById("btnCreateAuthor").addEventListener('click', () => showCreateAuthorModal());

    //Finds and stores all the buttons on screen that has the delete-buttons class on them.
    let deleteButtons = document.getElementsByClassName("delete-button");
    //Cycle through all the buttons retrieved by the get method.
    for (var i = 0; i < deleteButtons.length; i++) {
        //Gets the value attribute from the DeleteButton and stores its value.
        let id = deleteButtons[i].getAttribute('value');
        //Add an event listener yto this current button and tell it whoch method to pass. 
        //This also passes the id retrieved in the previous step as a parameter of the method call.
        deleteButtons[i].addEventListener("click", () => showDeleteAuthorModal(id))
    }
})

async function showDeleteAuthorModal(id) {
    //Get the partial view from the Delete method of the Authors controller for the provided ID
    let result = await fetch("/Authors/Delete/" + id);
    //Gets the text from the body of the http response from the previous step. This will be the code
    //that makes up our create partial view.
    let htmlResult = await result.text();
    //Access the modal body using its ID and put the text content between the tags of the modal body.
    document.getElementById("authorModalBody").innerHTML = htmlResult;
    //Access the modal title using its ID and change its text.
    document.getElementById("authorModalLabel").innerHTML = "Delete Author??";

    //Use the query selector method on the document to find the component that is specified by the selector.
    //The selector parameter takes a string that specifies the element type (form) followed by a set of square 
    //brackets specifying a property and value that need to exist in the element to match the selector.
    let form = document.querySelector(`form[action='/Authors/Delete/${id}']`);
    //Add a listener to the form tnat will trigger when the submit/save button is pressed.
    form.addEventListener('submit', async (e) => {
        //Tell the form not to do its default action when pressed.
        e.preventDefault();

        //Use a JQuery query selector to find an input field with the name specified and get its value.
        let token = $('input[name="__RequestVerificationToken"]').val();
        //Send a fetch request with the form data and the request token to the controller
        let result = await fetch("/Authors/Delete/" + id, {
            method: "POST",
            headers: {
                "content-type": "application/json",
                "RequestVerificationToken": token
            },
            body: ""
        })
        //If we gat any response code other than 200, throw an error alert.
        if (result.status != 200) {
            alert("Something  went wrong.");
            return;
        }
        //If it was a successful response code, close the modal and refresh the page.
        location.reload();
    })

    //Tell the modal to show itself on screen. This command is using JQuery, a JavaScript library included in MVC
    //which provided shorcut commands to access some components and to use them.
    $("#authorModal").modal("show");
}

async function showCreateAuthorModal() {
    //Get the partial view from the Create method of the Authors controller
    let result = await fetch("/Authors/Create");
    //Gets the text from the body of the http response from the previous step. This will be the code
    //that makes up our create partial view.
    let htmlResult = await result.text();
    //Access the modal body using its ID and put the text content between the tags of the modal body.
    document.getElementById("authorModalBody").innerHTML = htmlResult;
    //Access the modal title using its ID and change its text.
    document.getElementById("authorModalLabel").innerHTML = "Create Author";

    //Use the query selector method on the document to find the component that is specified by the selector.
    //The selector parameter takes a string that specifies the element type (form) followed by a set of square 
    //brackets specifying a property and value that need to exist in the element to match the selector.
    let form = document.querySelector("form[action='/Authors/Create']");
    //Add a listener to the form tnat will trigger when the submit/save button is pressed.
    form.addEventListener('submit', async (e) => {
        //Tell the form not to do its default action when pressed.
        e.preventDefault();
        //Get the form fields and put the data from them into a JavaScript object.
        let formData = {
            FirstName: e.target["FirstName"].value,
            LastName: e.target["LastName"].value
        };
        //Use a JQuery query selector to find an input field with the name specified and get its value.
        let token = $('input[name="__RequestVerificationToken"]').val();
        //Send a fetch request with the form data and the request token to the controller
        let result = await fetch("/Authors/Create", {
            method: "POST",
            headers: {
                "content-type": "application/json",
                "RequestVerificationToken": token
            },
            body: JSON.stringify(formData)
        })
        //If we gat any response code other than 200, throw an error alert.
        if (result.status != 200) {
            alert("Something  went wrong. Please check the form details and try again");
            return;
        }
        //If it was a successful response code, close the modal and refresh the page.
        location.reload();
    })

    //Tell the modal to show itself on screen. This command is using JQuery, a JavaScript library included in MVC
    //which provided shorcut commands to access some components and to use them.
    $("#authorModal").modal("show");
}

//Method that changes our colour theme
async function switchTheme() {
    //Get our current theme setting form the localstorage of the PC. If it is null set it to Dark.
    let currentTheme = localStorage.getItem("Theme") || "Dark"
    var url = window.location.origin;

    if (currentTheme == "Dark") {

        localStorage.setItem("Theme", "Light")


        var rersult = await fetch(url + "/api/Settings/SetTheme", {
            method: "POST",
            headers: {
                "content-type": "application/json"
            },
            body: JSON.stringify({ Theme: "Light" })
        })

        document.getElementById("themeStyle").setAttribute("href", "/css/themes/light-theme.css")
    }
    else {
        localStorage.setItem("Theme", "Dark")

        var rersult = await fetch(url + "/api/Settings/SetTheme", {
            method: "POST",
            headers: {
                "content-type": "application/json"
            },
            body: JSON.stringify({ Theme: "Dark" })
        })

        document.getElementById("themeStyle").setAttribute("href", "/css/themes/dark-theme.css")
    }
}