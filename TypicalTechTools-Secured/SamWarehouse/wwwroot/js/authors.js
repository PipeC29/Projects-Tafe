//File to manage the Javascript code that is only for the authors page. By utting it here instead of in the
//site.js file, we can setup our website to only run this coder form the authors page. If it was in the 
//site.js file, it would try to run on every page that opens.

window.addEventListener('load', () => {

    document.getElementById("btnCreateAuthor").addEventListener("click", () => showCreateAuthorModal())

    //gets all the elements with the 'edit-Button' class applied and stores them.
    let deleteButtons = document.getElementsByClassName("delete-Button")

    //Cycle through the array of buttons
    for (let i = 0; i < deleteButtons.length; i++) {
        //get the Id out of the value atribute or the current button
        let id = deleteButtons[i].getAttribute('value');
        //Attach an event listener to the button and set ti to trigger the specified method
        //when pressed and pass the id to the method.
        deleteButtons[i].addEventListener("click", () => showDeleteAuthorModal(id))
    }

    //gets all the elements with the 'edit-Button' class applied and stores them.
    let editButtons = document.getElementsByClassName("edit-Button")

    //Cycle through the array of buttons
    for (let i = 0; i < editButtons.length; i++) {
        //get the Id out of the value atribute or the current button
        let id = editButtons[i].getAttribute('value');
        //Attach an event listener to the button and set ti to trigger the specified method
        //when pressed and pass the id to the method.
        editButtons[i].addEventListener("click", () => showEditAuthorModal(id))
    }

    //gets all the elements with the 'details-Button' class applied and stores them.
    let detailsButtons = document.getElementsByClassName("details-Button")

    //Cycle through the array of buttons
    for (let i = 0; i < detailsButtons.length; i++) {
        //get the Id out of the value atribute or the current button
        let id = detailsButtons[i].getAttribute('value');
        //Attach an event listener to the button and set ti to trigger the specified method
        //when pressed and pass the id to the method.
        detailsButtons[i].addEventListener("click", () => showDetailsAuthorModal(id))
    }

})

async function showEditAuthorModal(id) {
    //Request the partial view from the controller
    var result = await fetch("/Authors/Edit/" + id);
    //Convert the response into its plain HTML content
    var htmlResult = await result.text();
    //Access the modal body by its ID and change its internal content
    //to be the partial view content.
    document.getElementById("authorModalBody").innerHTML = htmlResult;
    //Update the title of the modal to reflect the pertial view being shown
    document.getElementById("authorModalTitle").innerHTML = "Edit Author";

    //Uses the Javascript query selector command to find the component of the specified type (form)
    //that has an action connecting to the specified endpoint.
    //NOTE: The word form needs to be lower case and have the endpoint capitalised as per your controller
    //endpoint casing used.
    let form = document.querySelector(`form[action='/Authors/Edit/${id}']`);

    //Passes the form to JQUERY(A Javascript library) to parse/interpret the form to work out what
    //rules it needs to follow to class as valid. This needs to be done manually because this stp is normally
    //done when the page loads, but the form in the modal is added to the page at a later stage when we push the 
    //button to open the modal.
    $.validator.unobtrusive.parse($('form'));

    console.log(form);
    //Add listener to form to trigger on submit event.
    form.addEventListener('submit', async (e) => {
        //Cancel the default submit action of this form when the button is pressed.
        e.preventDefault();

        //Get the form that was submitted by the event and store it as a variable.
        //NOTE: At this stage the form will just be HTML text.
        let form = e.target;
        //Pass the form to JQUERY to be translated/retrieved at a javascript object.
        //This step will make the form interactable in our code.
        let formResult = $(form);
        //Checks the status of the form to see if it is valid. If it is not valid, which means
        //it doesn't meet our validation rules for the form fields, don't proceed any further.
        //This will stop the form even being sent to the erver by our browser.
        if (formResult.valid() == false) {
            return;
        }

        let formData = {
            //get the form fields by name (ID or asp-for tag) and store them in properties
            //of the form data
            //NOTE: Your targets will be capitalised as per their naming in C# and will need to be
            //in pascal case in the javascript object in order for it to map to your controller.
            Id: id,
            FirstName: e.target["FirstName"].value,
            LastName: e.target["LastName"].value
        }

        console.log(formData);

        //Get the anti-forgery token from the form and store it.
        var token = $('input[name="__RequestVerificationToken"]').val();

        //Send a fetch request top the desired endpoint to create an author
        let result = await fetch("/Authors/Edit/" + id, {
            //Set the HTTP method type 
            method: "POST",
            //Set the headers to specify the content type of the request that will be sent.
            data: {

            },
            headers: {
                "content-type": "application/json",
                "RequestVerificationToken": token
            },
            //Convert the form data to a JSON string and add to to the reqest body
            body: JSON.stringify(formData)
        })

        if (result.status != 200) {
            alert("Something Went wrong")
            return;
        }

        location.reload();
    });

    //Tell the modal to show itself on screen.
    $('#authorModal').modal("show");
}

async function showDetailsAuthorModal(id) {
    //Request the partial view from the controller
    var result = await fetch("/Authors/Details/" + id);
    //Convert the response into its plain HTML content
    var htmlResult = await result.text();
    //Access the modal body by its ID and change its internal content
    //to be the partial view content.
    document.getElementById("authorModalBody").innerHTML = htmlResult;
    //Tell the modal to show itself on screen.
    //Update the title of the modal to reflect the pertial view being shown
    document.getElementById("authorModalTitle").innerHTML = "Author Details";

    $('#authorModal').modal("show");
}

async function showDeleteAuthorModal(id) {
    //Request the partial view from the controller
    var result = await fetch("/Authors/Delete/" + id);
    //Convert the response into its plain HTML content
    var htmlResult = await result.text();
    //Access the modal body by its ID and change its internal content
    //to be the partial view content.
    document.getElementById("authorModalBody").innerHTML = htmlResult;
    //Update the title of the modal to reflect the pertial view being shown
    document.getElementById("authorModalTitle").innerHTML = "Delete Author?";

    //Uses the Javascript query selector command to find the component of the specified type (form)
    //that has an action connecting to the specified endpoint.
    //NOTE: The word form needs to be lower case and have the endpoint capitalised as per your controller
    //endpoint casing used.
    let form = document.querySelector(`form[action='/Authors/Delete/${id}']`);
    console.log(form);
    //Add listener to form to trigger on submit event.
    form.addEventListener('submit', async (e) => {
        //Cancel the default submit action of this form when the button is pressed.
        e.preventDefault();

        //Get the anti-forgery token from the form and store it.
        var token = $('input[name="__RequestVerificationToken"]').val();

        //Send a fetch request top the desired endpoint to create an author
        let result = await fetch("/Authors/Delete/" + id, {
            //Set the HTTP method type 
            method: "POST",
            //Set the headers to specify the content type of the request that will be sent.
            data: {

            },
            headers: {
                "content-type": "application/json",
                "RequestVerificationToken": token
            },
            //Convert the form data to a JSON string and add to to the reqest body
            body: ""
        })

        if (result.status != 200) {
            alert("Something Went wrong")
            return;
        }

        location.reload();
    });

    //Tell the modal to show itself on screen.
    $('#authorModal').modal("show");
}


async function showCreateAuthorModal() {
    //Request the partial view from the controller
    var result = await fetch("/Authors/Create");
    //Convert the response into its plain HTML content
    var htmlResult = await result.text();
    //Access the modal body by its ID and change its internal content
    //to be the partial view content.
    document.getElementById("authorModalBody").innerHTML = htmlResult;
    //Tell the modal to show itself on screen.
    //Update the title of the modal to reflect the pertial view being shown
    document.getElementById("authorModalTitle").innerHTML = "Create Author";

    //Uses the Javascript query selector command to find the component of the specified type (form)
    //that has an action connecting to the specified endpoint.
    //NOTE: The word form needs to be lower case and have the endpoint capitalised as per your controller
    //endpoint casing used.
    let form = await document.querySelector("form[action='/Authors/Create']");

    //Passes the form to JQUERY(A Javascript library) to parse/interpret the form to work out what
    //rules it needs to follow to class as valid. This needs to be done manually because this stp is normally
    //done when the page loads, but the form in the modal is added to the page at a later stage when we push the 
    //button to open the modal.
    $.validator.unobtrusive.parse($('form'));

    //Add listener to form to trigger on submit event.
    form.addEventListener('submit', async (e) => {
        //Cancel the default submit action of this form when the button is pressed.
        e.preventDefault();

        //Get the form that was submitted by the event and store it as a variable.
        //NOTE: At this stage the form will just be HTML text.
        let form = e.target;
        //Pass the form to JQUERY to be translated/retrieved at a javascript object.
        //This step will make the form interactable in our code.
        let formResult = $(form);
        //Checks the status of the form to see if it is valid. If it is not valid, which means
        //it doesn't meet our validation rules for the form fields, don't proceed any further.
        //This will stop the form even being sent to the erver by our browser.
        if (formResult.valid() == false) {
            return;
        }


        //Create a Javascript object to hold our form data
        let formData = {
            //get the form fields by name (ID or asp-for tag) and store them in properties
            //of the form data
            //NOTE: Your targets will be capitalised as per their naming in C# and will need to be 
            //in pascal case in the javascript object in order for it to map to your controller.
            FirstName: e.target["FirstName"].value,
            LastName: e.target["LastName"].value
        }

        //Get the anti-forgery token from the form and store it.
        var token = $('input[name="__RequestVerificationToken"]').val();

        //Send a fetch request top the desired endpoint to create an author
        let result = await fetch("/Authors/Create", {
            //Set the HTTP method type 
            method: "POST",
            //Set the headers to specify the content type of the request that will be sent.
            data: {

            },
            headers: {
                "content-type": "application/json",
                "RequestVerificationToken": token
            },
            //Convert the form data to a JSON string and add to to the reqest body
            body: JSON.stringify(formData)
        })

        if (result.status != 200) {
            alert("Something Went wrong")
            return;
        }

        location.reload();
    });

    $('#authorModal').modal("show");
}