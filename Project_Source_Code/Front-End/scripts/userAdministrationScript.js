//#region display marketplace 
function displayUserAdministration() {
    var dynamicContent = document.querySelector(".dynamic-content");
    var currPage;
    try {
        currPage = parseInt(document.getElementById('current-page').innerText);
    }
    catch {
        currPage = 1;
    }
    while (dynamicContent.lastElementChild) {
        dynamicContent.removeChild(dynamicContent.lastElementChild);
    }

    var VehicleMarketplace = document.createElement('div');
    VehicleMarketplace.id = 'vehicle-marketplace';

    dynamicContent.appendChild(VehicleMarketplace);

    var dynamicContent = document.getElementById('vehicle-marketplace');
    var html = "<div class=VPMcontainer>";
 
    var buttons = document.createElement('div');
    buttons.id = 'vehicle-details-buttons';
    dynamicContent.appendChild(buttons);
    dynamicContent.style.display = "block";
    generateModifyButton(buttons);
    generateAccountDeletionButton(buttons);
    generateUserInfoRequestButton(buttons);
}


function generateModifyButton(content) {
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'Modify';

    button.addEventListener('click', (event) => {
        event.stopImmediatePropagation();
        const buttons = document.getElementById("vehicle-details-buttons");
        alert("Modify button clicked");
        //add function here to make it do something 
        
    });
    content.appendChild(button);
}


function generateAccountDeletionButton(content) {
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'Account Deletion';

    button.addEventListener('click', (event) => {
        event.stopImmediatePropagation();
        const buttons = document.getElementById("vehicle-details-buttons");
        alert("Account deletion button clicked");
        //add function here to make it do something 
        
        
    });
    content.appendChild(button);
}

function generateUserInfoRequestButton(content) {
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'Info Request';

    button.addEventListener('click', (event) => {
        event.stopImmediatePropagation();
        const buttons = document.getElementById("vehicle-details-buttons");
        alert("Info request button clicked");
        //add function here to make it do something
        RequestingInfo();
         
        
    });
    content.appendChild(button);
}

function RequestingInfo(){
    fetchWithTokens('http://localhost:8004/RequestUserData/UserDataRequest', 'POST',"")
    .then(function (response) {
        if (response.status == 200) { 
            response.json()
            .then(function(data) {
                alert("Email sent");
            })
            .catch(function (error) {
                alert("Error parsing JSON: " + error);
            });
        } 
        else {
            // Handle response as string
            response.text()
            .then(function(text) {
                alert(text); // Alert the string response
                var dynamicContent = document.querySelector(".dynamic-content");
                dynamicContent.innerHTML = text;
            })
            .catch(function (error) {
                alert("Error reading response text: " + error);
            });
        }
    }); 

}

function exrtactDataUserAdmin(jsonData) {
    //var data = JSON.parse(jsonData);
    //page generation 
    var dynamicContent = document.querySelector(".dynamic-content");
    var html = "<div class=VPMcontainer>";
    jsonData.forEach(function(data) {
        // Access values from each object
        var VIN = data.userName;
        var make = data.address;
        var model = data.name;
        var uid = data.userId; 
        var hash = data.userHash;
        
        // Create HTML content with the extracted values
       
        // html += "<div class=vehicle-listings>";
        // html += '<img src=' + "VPM-resource/car.png" + '>'; 
        // html += '<h2> Vehicle' + '</h2>';
        html += `
        <div id=account-detail class=vehicle-listings>
                <ul>
                    <li>${VIN}</li>
                    <li>${make}</li>
                    <li>${model}</li>
                    <li>${uid}</li>
                    <li>${hash}</li>
                </ul>
                <button id= "update-button" class="corner-button">Update User Account</button>
                <button id= "disable-button" class="disable-button">Disable User Account</button>
                <button id= "enable-button" class="enable-button">Enable User Account</button>
            </div>
        `;
    });   
    html += "</div>"; 
    html += "<div id=detail></div>";
    dynamicContent.innerHTML = html;

    // Get all buttons with the class "corner-button"
    var buttons = document.getElementsByClassName("corner-button");

    // Add event listener to each button
    for (var i = 0; i < buttons.length; i++) {
    buttons[i].addEventListener("click", function() {
         // Perform your action here
         var content = document.getElementById('detail');
         content.innerHTML = '';
         content.style.display = "block";
         // Create H1 element 
         var title = document.createElement('h1');
         title.textContent = "Update Account";
         content.appendChild(title);
     
         // Create paragraph element. Displays the instructions use the VIN retrieval
         var message = document.createElement('p');
         message.textContent = "Input to any field you want to change, leave blank if you want to keep it the same";
         content.appendChild(message);
         generateForm();

        document.getElementById('submit').addEventListener('click', (event) => {
        const data = {
            userName: document.getElementById('UserName').value.trim(),
            uid: document.getElementById('userId').value.trim(),
            salt: 0,
            hash: document.getElementById('userHash').value.trim(),
            address:  document.getElementById('vin').value.trim() + ", " + document.getElementById('color').value.trim(),
            name:  document.getElementById('licensePlate').value.trim(), 
            phone:  document.getElementById('make').value.trim(),
            accountType:  document.getElementById('model').value.trim()
        };
        postUpdateAccount(data);

        event.stopImmediatePropagation();
    });
    });
    }

    // Get all buttons with the class "corner-button"
    var disable_buttons = document.getElementsByClassName("disable-button");

    // Add event listener to each button
    for (var i = 0; i < disable_buttons.length; i++) {
        disable_buttons[i].addEventListener("click", function() {
        // Perform your action here
        var content = document.getElementById('detail');
        content.innerHTML = '';
        content.style.display = "block";
        generateDeleteView(disable_buttons);
    });
    }


   
}

function generateDeleteButton(content) {
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'Delete Vehicle';

    button.addEventListener('click', (event) => {
        event.stopImmediatePropagation();
        generateDeleteView();
    });
    content.appendChild(button);
}

function postUpdateAccount(request){
    fetchWithTokens('http://localhost:8004/RequestUserData/UpdateUserAccount', 'POST',request)
    .then(response => {
        if (response.status == 403) {   
            throw "You are unauthorized";
        }
        if (!response.ok) {
            throw "Could not process request";
        }
        if (response.status == 202)
        {
            throw "Could not create vehicle";
        }
        else {
            return response.json();
        }
    })
    .then( () => {
        generateVehicleProfileView()
    })
    .catch(error => {
        alert(error)
    })

}

function generateDeleteView() {


    // Delete elements in the deatils box
    var vehicleDetails = document.getElementById("detail");
    while (vehicleDetails.lastChild) {
        vehicleDetails.removeChild(vehicleDetails.lastChild);
    }

    // Create new elements
    var message = document.createElement('h1');
    message.id = 'message';
    message.innerText = "Are you sure you want to disable this account?";

    var submit = document.createElement('input');
    submit.type = 'button';
    submit.id = 'submit';
    submit.value = 'Confim';
    
    var cancel = document.createElement('input');
    cancel.type = 'button';
    cancel.id = 'cancel';
    cancel.value = 'Cancel';

    var vehicleDetailsButton = document.createElement('div');
    vehicleDetailsButton.id = 'vehicle-details-buttons';

    vehicleDetailsButton.appendChild(cancel);
    vehicleDetailsButton.appendChild(submit);
    vehicleDetails.appendChild(message);
    vehicleDetails.appendChild(vehicleDetailsButton);

    // Add event listeners to the buttons
    submit.addEventListener('click', async () => {
        try {
            var vehicle = JSON.parse(sessionStorage.getItem(vin));
            await postDeleteVehicleProfile(vehicle);
            alert('Successfully disabled');
            DisplayAllAccounts();
        }
        catch (error) {
            alert(error);
        }
    });
    cancel.addEventListener('click', (event) => {
        var content = document.getElementById('detail');
        if (content.contains(event.target)) {
            content.innerHTML = '';
            content.style.display = "none";
        }
    });
}

function generateForm()
{
    var content = document.getElementById('detail');
    var form = document.createElement('form');
    form.id = 'form';

    var inputVIN = document.createElement('input');
    inputVIN.type = 'text';
    inputVIN.maxLength = 17;
    inputVIN.id = 'vin';
    inputVIN.placeholder = 'Address';
    var inputLicensePlate = document.createElement('input');
    inputLicensePlate.type = 'text';
    inputLicensePlate.maxLength = 8;
    inputLicensePlate.id = 'licensePlate';
    inputLicensePlate.placeholder = 'Name';
    var inputMake = document.createElement('input');
    inputMake.type = 'text';
    inputMake.id = 'make';
    inputLicensePlate.maxLength = 10;
    inputMake.placeholder = 'Phone';
    var inputModel = document.createElement('input');
    inputModel.type = 'text';
    inputModel.id = 'model';
    inputModel.placeholder = 'Account Type';
    var inputSubmit = document.createElement('input');
    inputSubmit.type = 'button';
    inputSubmit.value = 'Submit';
    inputSubmit.id = 'submit';
    var inputCancel = document.createElement('input');
    inputCancel.type = 'button';
    inputCancel.value = 'Cancel';
    inputCancel.id = 'cancel';
    var inputColor = document.createElement('input');
    inputColor.type = 'text';
    inputColor.id = 'color';
    inputColor.placeholder = 'State';
    var inputUserHash = document.createElement('input');
    inputUserHash.type = 'text';
    inputUserHash.id = 'userHash';
    inputUserHash.maxLength = 20;
    inputUserHash.placeholder = 'userHash';
    var inputUserID = document.createElement('input');
    inputUserID.type = 'text';
    inputUserID.id = 'userId';
    inputUserID.maxLength = 10;
    inputUserID.placeholder = 'userId';
    var inputUserName = document.createElement('input');
    inputUserName.type = 'text';
    inputUserName.id = 'UserName';
    inputUserName.maxLength = 10;
    inputUserName.placeholder = 'UserName';

    form.appendChild(inputVIN);
    form.appendChild(inputLicensePlate);
    form.appendChild(inputMake);
    form.appendChild(inputModel);
    form.appendChild(inputColor);
    form.appendChild(inputUserHash);
    form.appendChild(inputUserID);
    form.appendChild(inputUserName);
    form.appendChild(inputSubmit);
    form.appendChild(inputCancel);
    content.appendChild(form);

    var cancel = document.getElementById('cancel');
    cancel.addEventListener('click', () => {
        content.innerHTML = '';
        content.style.display = "none";
    })
}
window.generateUploadForm = generateUploadForm;

function DisplayAllAccounts()
{
    fetchWithTokens('http://localhost:8004/RequestUserData/RetrieveAllAccount', 'POST','')
    .then(function (response) {
        if (response.status == 200) { 
            response.json()
            .then(function(data) {
                // Check if data is an array or object
                if (Array.isArray(data) && data.length > 0) {
                    //alert("VP's retrieved");
                    exrtactDataUserAdmin(data);
                    // var vehicles = document.getElementsByClassName('vehicle-listings');
                    // for (let i = 0; i < vehicles.length; i++) {
                    //     vehicles[i].addEventListener('click', (event) => {
                    //         event.stopPropagation();
                    //         displayDetailMarketplace(vehicles[i].id);
                    //     })
                    // }
                } 
            })
            .catch(function (error) {
                alert("Error parsing JSON: " + error);
            });
        } 
        else {
            // Handle response as string
            response.text()
            .then(function(text) {
                alert(text); // Alert the string response
                var dynamicContent = document.querySelector(".dynamic-content");
                dynamicContent.innerHTML = text;
            })
            .catch(function (error) {
                alert("Error reading response text: " + error);
            });
        }
    }); 
}
    
