const generateViewButton = document.getElementById("vehicle-marketplace-view");
//generateViewButton.addEventListener("click", generateMainView);

function exrtactData(jsonData) {
    //var data = JSON.parse(jsonData);
    //page generation 
    var dynamicContent = document.getElementById('vehicle-marketplace');
    var html = "<div class=VPMcontainer>";
    jsonData.forEach(function(data) {
        // Access values from each object
        var VIN = data.vin;
        var make = data.make;
        var model = data.model;
        
        // Create HTML content with the extracted values
       
        // html += "<div class=vehicle-listings>";
        // html += '<img src=' + "VPM-resource/car.png" + '>'; 
        // html += '<h2> Vehicle' + '</h2>';
        html += `
        <div id=${VIN} class=vehicle-listings>
        <img src="VPM-resource/car.png">
                <ul>
                    <li>${VIN}</li>
                    <li>${make}</li>
                    <li>${model}</li>
                </ul>
            </div>
        `;
    });   
    html += "</div>"; 
    //html += "<div id=detail></div>";
    dynamicContent.innerHTML = html;

   
}

function exrtactDataDetail(jsonData) {
    //var data = JSON.parse(jsonData);
    // var dynamicContent = document.querySelector(".dynamic-content");
    // var html = "<div class=VPMcontainer>";

    content = document.getElementById('detail');
    
    var VIN; 
    var make; 
    var model; 
    var description; 
    var date;

    jsonData.forEach(function(data) {
        // Access values from each object
        VIN = data.vin;
        make = data.make;
        model = data.model;
        description = data.description;
        date = data.dateCreated; 
        var license = data.licensePlate; 
        var status = data.marketplaceStatus; 
        var id = data.owner_UID; 
        var view = data.viewStatus;
        var year = data.year;   
    });
    content.innerHTML= `
    <div id=test class=vehicle-listings>
    <img src="VPM-resource/car.png">
            <ul id="detail-description">
                <li class="vin">${VIN}</li>
                <li>${make}</li>
                <li>${model}</li>
                <li>${description}</li>
                <li>Date posted: ${date}</li>
            </ul>
        </div>
    `;
    //});   
    //html += "</div>"; 
    var buttons = document.createElement('div');
    buttons.id = 'vehicle-details-buttons';
    content.appendChild(buttons);
    content.style.display = "block";
    generateRequestMarketValueButton(buttons,VIN);    
    generateBuyRequestButton(buttons,VIN);

    document.addEventListener('click', (event) => {
        if (!content.contains(event.target)) {
            content.innerHTML = '';
            content.style.display = "none";
        }
    })
    //dynamicContent.innerHTML = html;

   
}

function exrtactDataCreatePost() {
    //var data = JSON.parse(jsonData);
    var dynamicContent = document.querySelector(".dynamic-content");
    var html = "<div class=VPMcontainer>";
    html += '<p>Vehicle Create Success</p>';
    html += "</div>"; 
    dynamicContent.innerHTML = html;

   
}

//#region request market value  
function generateRequestMarketValueButton(content,VIN) {
    var button = document.createElement('input');
        button.type = 'button';
        button.value = 'Request Market Value';
    
        button.addEventListener('click', () => {
            fetchAPIMarktValue(VIN);
            //console.log("clicked request button")
        });
    content.appendChild(button);
}

async function fetchAPIMarktValue(vin){
    const url = 'https://car-utils.p.rapidapi.com/marketvalue?vin='+vin;
    const options = {
        method: 'GET',
        headers: {
            'X-RapidAPI-Key': '37741929a5msh0db65cff0af718dp12ed32jsnfd3141e82723',
            'X-RapidAPI-Host': 'car-utils.p.rapidapi.com'
        }
    };
    try {
        const response = await fetch(url, options);
        const result = await response.text();
        console.log(result);
    } catch (error) {
        console.error(error);
    };
}
//#endregion




//#region display marketplace 
function displayMarketplace() {
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

    pages(displayMarketplace);
    document.getElementById('current-page').innerText = currPage;
        
    fetchWithTokens(CONFIG["ip"] + ':' + CONFIG["ports"]["vehicleMarketplace"] + '/VehicleMarketplace/GetVehicleMarketplace', 'POST',currPage)
    .then(function (response) {
        if (response.status == 200) { 
            response.json()
            .then(function(data) {
                // Check if data is an array or object
                if (data.length == 0 && parseInt(document.getElementById('current-page').innerText) != 1)
                    {
                        decrementPages();
                        displayMarketplace();
                        return;
                    }
                else if (Array.isArray(data) && data.length > 0) {
                    //alert("VP's retrieved");
                    exrtactData(data);
                    var vehicles = document.getElementsByClassName('vehicle-listings');
                    for (let i = 0; i < vehicles.length; i++) {
                        vehicles[i].addEventListener('click', (event) => {
                            event.stopPropagation();
                            displayDetailMarketplace(vehicles[i].id);
                        })
                    }
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

function displayDetailMarketplace(vin){
    var response;
    var content;
    try {
        fetchWithTokens(CONFIG["ip"] + ':' + CONFIG["ports"]["vehicleMarketplace"]+'/VehicleMarketplace/VehicleMarketplacePostDetailRetrieval', 'POST',vin)
        .then(function (response) {
        if (response.status == 200) { 
            response.json()
            .then(function(data) {
                // // Check if data is an array or object
                // if (Array.isArray(data) && data.length > 0) {
                //     content = document.getElementById('detail');
                //     content.innerHTML= `
                //         <h1 id='vehicle'>${vin}</h1>
                    
                //     `;
                //     var buttons = document.createElement('div');
                //     buttons.id = 'vehicle-details-buttons';
                //     content.appendChild(buttons);
                //     content.style.display = "block";
                // } 
                exrtactDataDetail(data);
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
    catch (error) {
        content = document.getElementById('vehicle-details');
        content.innerHTML= `
            <h1 id='error-message'>${error}. Please try again later.</h1>
        `;
        content.style.display = "block";
    }
}

//#endregion


//#region Upload to marketplace 
function uploadToMarketplace(VIN,description){
    var content = document.getElementById('vehicle-details');
    content.innerHTML = '';
    content.style.display = "block";
    // Create H1 element 
    var title = document.createElement('h1');
    title.textContent = "Description";
    content.appendChild(title);

    // Create paragraph element. Displays the instructions use the VIN retrieval
    var message = document.createElement('p');
    message.textContent = "Please input iyour description of the vehicle";
    content.appendChild(message);

    // Generate a form that lets the user input vin, licenseplate, make, model, year, color, and description
    generateUploadForm();

  
    

    document.getElementById('submit').addEventListener('click', (event) => {
        const vehicle = {
            vin: VIN,
            licensePlate: document.getElementById('licensePlate').value.trim(),
            make: document.getElementById('make').value.trim(),
            model: document.getElementById('model').value.trim(),
            year: parseInt(document.getElementById('year').value),
        };
        const details = {
            vin: document.getElementById('vin').value.toUpperCase().trim(),
            color: document.getElementById('color').value.trim(),
            description: document.getElementById('description').value.trim()
        };
        const marketplaceStatus = {
            View: 1,
            MarketplaceStatus: 1,
            Description: document.getElementById('description').value.trim()
        };
        const data = {
            vehicleProfile: vehicle,
            vehicleDetails: details, 
            status: marketplaceStatus
        };
        /*console.log("Submit button clicked");
        console.log("vehicle ", vehicle); 
        console.log("details: ", details);
        console.log("status: ", marketplaceStatus)*/
        postUploadToMarketplace(data);

        event.stopImmediatePropagation();
    });

}

function postUploadToMarketplace(vehicle) {
    fetchWithTokens(CONFIG["ip"] + ':' + CONFIG["ports"]["vehicleMarketplace"]+'/VehicleMarketplace/VehicleMarketplacePostCreation', 'POST',vehicle)
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
//#endregion

//#region Delete from marketplace 
function deleteFromMarketplace(VIN,description){
    var content = document.getElementById('vehicle-details');
    content.innerHTML = '';
    content.style.display = "block";
    // Create H1 element 
    var title = document.createElement('h1');
    title.textContent = "Description";
    content.appendChild(title);

    // Create paragraph element. Displays the instructions use the VIN retrieval
    var message = document.createElement('p');
    message.textContent = "Please input iyour description of the vehicle";
    content.appendChild(message);

    // Generate a form that lets the user input vin, licenseplate, make, model, year, color, and description
    generateUploadForm();

  
    

    document.getElementById('submit').addEventListener('click', (event) => {
        const vehicle = {
            vin: VIN,
            licensePlate: document.getElementById('licensePlate').value.trim(),
            make: document.getElementById('make').value.trim(),
            model: document.getElementById('model').value.trim(),
            year: parseInt(document.getElementById('year').value),
        };
        const details = {
            vin: document.getElementById('vin').value.toUpperCase().trim(),
            color: document.getElementById('color').value.trim(),
            description: document.getElementById('description').value.trim()
        };
        const marketplaceStatus = {
            View: 1,
            MarketplaceStatus: 1,
            Description: document.getElementById('description').value.trim()
        };
        const data = {
            vehicleProfile: vehicle,
            vehicleDetails: details, 
            status: marketplaceStatus
        };
        /*console.log("Submit button clicked");
        console.log("vehicle ", vehicle); 
        console.log("details: ", details);
        console.log("status: ", marketplaceStatus)*/
        postDeleteFromMarketplace(data);

        event.stopImmediatePropagation();
    });

}

function postDeleteFromMarketplace(vehicle) {
    fetchWithTokens(CONFIG["ip"] + ':' + CONFIG["ports"]["vehiclemarketplace"]+'/VehicleMarketplace/VehicleMarketplacePostDeletion', 'POST',vehicle)
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
//#endregion 

//#region buy request button 
function generateBuyRequestButton(content,VIN) {
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'Request Buy';

    button.addEventListener('click', (event) => {
        event.stopImmediatePropagation();
        console.log("clicked request button");
        console.log(VIN);

        const request = {
            uid: 1,
            vin: VIN,
            price: 1000
        };

        
        var message = document.createElement('h1');
        message.id = 'message';
        message.innerText = "Are you sure you want to send a buy request for this vehicle?";

        var submit = document.createElement('input');
        submit.type = 'button';
        submit.id = 'submit';
        submit.value = 'Confim';
        
        var cancel = document.createElement('input');
        cancel.type = 'button';
        cancel.id = 'cancel';
        cancel.value = 'Cancel';

        var vehicleDetails = document.getElementById("detail");
        var vehicleDetailsButton = document.createElement('div');
        vehicleDetailsButton.id = 'vehicle-details-buttons';

        vehicleDetailsButton.appendChild(cancel);
        vehicleDetailsButton.appendChild(submit);
        vehicleDetails.appendChild(message);
        vehicleDetails.appendChild(vehicleDetailsButton);

        // Add event listeners to the buttons
        submit.addEventListener('click', async () => {
            try {
                sendBuyRequest(request);
                alert('Buy request sent');
            }
            catch (error) {
                alert(error);
            }
        });
        cancel.addEventListener('click', (event) => {
            var content = document.getElementById('vehicle-details');
            if (content.contains(event.target)) {
                content.innerHTML = '';
                content.style.display = "none";
            }
        });

        event.stopImmediatePropagation();
        
    });
    content.appendChild(button);
}

function generateBuyRequestWindow(){
    var content = document.getElementById('detail');
    content.innerHTML = '';
    content.style.display = "block";
    // Create H1 element 
    var title = document.createElement('h1');
    title.textContent = "Description";
    content.appendChild(title);

    // Create paragraph element. Displays the instructions use the VIN retrieval
    var message = document.createElement('p');
    message.textContent = "Please input your username";
    content.appendChild(message);

    // Generate a form that lets the user input vin, licenseplate, make, model, year, color, and description
    generateBuyForm();

    document.getElementById('submit').addEventListener('click', (event) => {
        console.log("Before stopImmediatePropagation");
        event.stopImmediatePropagation();
    });
}

function sendBuyRequest(vehicle) {
    fetchWithTokens(CONFIG["ip"] + ':' + CONFIG["ports"]["vehicleMarketplace"]+'/VehicleMarketplace/SendBuyRequest', 'POST',vehicle)
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
//#endregion

function generateBuyForm(){
    var content = document.getElementById('detail');
    var form = document.createElement('form');
    form.id = 'form';

    var inputVIN = document.createElement('input');
    inputVIN.type = 'text';
    inputVIN.maxLength = 50;
    inputVIN.id = 'username';
    inputVIN.placeholder = 'Username';
    var inputSubmit = document.createElement('input');
    inputSubmit.type = 'button';
    inputSubmit.value = 'Submit';
    inputSubmit.id = 'submit';
    var inputCancel = document.createElement('input');
    inputCancel.type = 'button';
    inputCancel.value = 'Cancel';
    inputCancel.id = 'cancel';

    form.appendChild(inputVIN);
    form.appendChild(inputSubmit);
    form.appendChild(inputCancel);
    content.appendChild(form);
    var cancel = document.getElementById('cancel');
    cancel.addEventListener('click', () => {
        content.innerHTML = '';
        content.style.display = "none";
    })
}
window.generateBuyForm = generateBuyForm;

function generateUploadForm() {
    var content = document.getElementById('vehicle-details');
    var form = document.createElement('form');
    form.id = 'form';

    var inputVIN = document.createElement('input');
    inputVIN.type = 'text';
    inputVIN.maxLength = 17;
    inputVIN.id = 'vin';
    inputVIN.placeholder = 'VIN';
    var inputLicensePlate = document.createElement('input');
    inputLicensePlate.type = 'text';
    inputLicensePlate.maxLength = 8;
    inputLicensePlate.id = 'licensePlate';
    inputLicensePlate.placeholder = 'License Plate';
    var inputMake = document.createElement('input');
    inputMake.type = 'text';
    inputMake.id = 'make';
    inputMake.placeholder = 'Make';
    var inputModel = document.createElement('input');
    inputModel.type = 'text';
    inputModel.id = 'model';
    inputModel.placeholder = 'Model';
    var inputYear = document.createElement('input');
    inputYear.type = 'number';
    inputYear.min = '1970';
    inputYear.max = new Date().getFullYear();
    inputYear.id = 'year';
    inputYear.placeholder = 'Year';
    var inputColor = document.createElement('input');
    inputColor.type = 'text';
    inputColor.id = 'color';
    inputColor.placeholder = 'Color';
    var inputDescription = document.createElement('input');
    inputDescription.type = 'text';
    inputDescription.maxLength = 500;
    inputDescription.id = 'description';
    inputDescription.placeholder = 'Description';
    var inputSubmit = document.createElement('input');
    inputSubmit.type = 'button';
    inputSubmit.value = 'Submit';
    inputSubmit.id = 'submit';
    var inputCancel = document.createElement('input');
    inputCancel.type = 'button';
    inputCancel.value = 'Cancel';
    inputCancel.id = 'cancel';

    form.appendChild(inputVIN);
    form.appendChild(inputLicensePlate);
    form.appendChild(inputMake);
    form.appendChild(inputModel);
    form.appendChild(inputYear);
    form.appendChild(inputColor);
    form.appendChild(inputDescription);
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