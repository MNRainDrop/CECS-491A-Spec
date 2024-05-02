function createVehicleProfileView() {
    var dynamicContent = document.querySelector(".dynamic-content");
    while (dynamicContent.lastElementChild) {
        dynamicContent.removeChild(dynamicContent.lastElementChild);
    }

    var vehicleProfile = document.createElement('div');
    vehicleProfile.id = 'vehicle-profile';

    dynamicContent.appendChild(vehicleProfile);

    pages(createVehicleProfileView);
    // this should be in config file
    const webServiceUrl = 'http://localhost:8727/VehicleProfileRetrieve/MyVehicleProfiles';

    var popup = document.createElement('div');
    popup.id = 'vehicle-details';

    document.getElementsByClassName('dynamic-content')[0].appendChild(popup);
    // Insert Vehicle Creation Button Here
    // Make new function
    generateCreateButton()

    fetchWithTokens(webServiceUrl, 'POST', document.getElementById("current-page").innerText)
        .then(response => {
            if (!response.ok) {
                console.log('error');
                return;
            }
            else {
                return response.json();
            }
        })
        .then(data => {
            if (data.length == 0 && parseInt(document.getElementById('current-page').innerText) != 1)
            {
                decrementPages();
                return;
            }
            var content = document.getElementById('vehicle-profile');
            content.innerHTML = '';

            for (let i = 0; i < data.length; i++) {
                content.innerHTML += `
                    <div id='${data[i].vin}' class='vehicles'>
                        <h1 id=\'vehicle\'>${data[i].make} ${data[i].model} ${data[i].year}</h1>
                        <ul>
                            <li>${data[i].make}</li>
                            <li>${data[i].model}</li>
                            <li>${data[i].year}</li>
                            <li>${data[i].licensePlate}</li>
                        </ul>
                    </div>
                `;
                sessionStorage.setItem(data[i].vin, JSON.stringify(data[i]));
            }

            var vehicles = document.getElementsByClassName('vehicles');
            for (let i = 0; i < vehicles.length; i++) {
                vehicles[i].addEventListener('click', (event) => {
                    event.stopPropagation();
                    getVehicleDetails(vehicles[i].getAttribute("id"));
                })
            }
        })
        .catch(error => {
            console.log(error);
        });
}

async function getVehicleDetails(id) {
    var response;
    var content;
    const car = JSON.parse(sessionStorage.getItem(id));
    try {
        response = await getVehicleDetailsFromAPI(id);
        
        if (response) {
            content = document.getElementById('vehicle-details');
            content.innerHTML= `
                <h1 id='vehicle'>${response.make} ${response.model} ${response.year}</h1>
                <h2 id='vehicle-description'>${response.description}</h2>
                <ul id='vehicle-detail-list'>
                    <li>${response.color}</li>
                    <li>${response.licensePlate}</li>
                    <li class='vin'>${response.vin}</li>
                </ul>
            `;
            var buttons = document.createElement('div');
            buttons.id = 'vehicle-details-buttons';
            content.appendChild(buttons);
            content.style.display = "block";
            generateModifyButton(buttons)
            generateDeleteButton(buttons)
            generateViewServiceLogButton(buttons)
            generateUploadToMarketplaceButton(buttons)
            generateDonationButton(buttons)

            document.addEventListener('click', (event) => {
                if (!content.contains(event.target)) {
                    content.innerHTML = '';
                    content.style.display = "none";
                }
            })
        }

    }
    catch (error) {
        content = document.getElementById('vehicle-details');
        content.innerHTML= `
            <h1 id='error-message'>${error}. Please try again later.</h1>
        `;
    }
    content.style.display = "block";
    document.addEventListener('click', (event) => {
        if (!content.contains(event.target)) {
            content.innerHTML = '';
            content.style.display = "none";
        }
    })
    
}

async function getVehicleDetailsFromAPI(id) {
    // this should be in config file
    const webServiceUrl = 'http://localhost:8727/VehicleProfileRetrieve/MyVehicleProfileDetails';

    const car = JSON.parse(sessionStorage.getItem(id));
    return await fetchWithTokens(webServiceUrl, 'POST', car)
        .then(response => {
            if (response.status == 403) {
                throw "You are unauthorized";
            }
            if (!response.ok) {
                throw "Could not process request";
            }
            if (response.status == 202)
            {
                throw "There are no details available for your vehicle";
            }
            return response.json()
        })
        .then(data => {
            return {
                vin: car.vin,
                make: car.make,
                model: car.model,
                year: car.year,
                licensePlate: car.licensePlate,
                color: data[0].color,
                description: data[0].description
            }
        })
        .catch( error => {
            console.log(error);
            throw error;
        })
}

function generateCreateButton() {
    var createButton = document.createElement('input');
    createButton.type = 'button';
    createButton.id = 'create-vehicle-button';
    createButton.value = 'Create Vehicle';

    createButton.addEventListener('click', (event) => {
        event.stopPropagation();
        
        var content = document.getElementById('vehicle-details');
        content.innerHTML = '';
        content.style.display = "block";
        var title = document.createElement('h1');
        title.textContent = "Create Vehicle";
        content.appendChild(title);

        var message = document.createElement('p');
        message.textContent = "Click the button to automatically retrieve your Make Model and Year";
        content.appendChild(message);

        generateVehicleForm();
        var form = document.getElementById('form');
        var getMMYButton = document.createElement('input');
        getMMYButton.type = 'button'
        getMMYButton.id = 'get-MMY-button';
        getMMYButton.value = 'Get Make Model Year';
        
        form.insertBefore(getMMYButton, form.children[1]);

        getMMYButton = document.getElementById('get-MMY-button');
        getMMYButton.addEventListener('click', async () => {
            var vinInput = document.getElementById('vin');
            var data = await retrieveMMYFromAPI(vinInput.value);
            fillInForm(data);
        });

        document.getElementById('submit').addEventListener('click', (event) => {
            const vehicle = {
                vin: document.getElementById('vin').value.toUpperCase().trim(),
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
            const data = {
                vehicleProfile: vehicle,
                vehicleDetails: details
            };
            postCreateVehicleProfileRequest(data);
    
            event.stopImmediatePropagation();
        });
    });
    var dynamicContent = document.getElementsByClassName('dynamic-content')[0];
    dynamicContent.insertBefore(createButton, dynamicContent.children[0]);
}

function generateVehicleForm() {
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
    cancel.addEventListener('click', (event) => {
        if (content.contains(event.target)) {
            content.innerHTML = '';
            content.style.display = "none";
        }
    })
}

async function retrieveMMYFromAPI(vin) {
    const webServiceUrl = 'https://vpic.nhtsa.dot.gov/api/vehicles/DecodeVinValues/' + vin + '?format=json'

    const options = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json', // Specify content type as JSON
        },
    };
    return await fetch(webServiceUrl, options)
        .then(response => {
            return response.json()
        })
        .then(data => {
            const vehicle = {
                vin: data["Results"][0]['VIN'],
                make : data["Results"][0]['Make'],
                model: data["Results"][0]['Model'],
                year: data["Results"][0]['ModelYear']
            };
            return vehicle;
        })
        .catch(error => {
            console.log(error);
        })
}

function fillInForm(formData)
{
    for (const property in formData) {
        if (formData[property] != '')
        {
            document.getElementById(`${property}`).value = formData[property];
        }
    }
}

function postCreateVehicleProfileRequest(vehicle) {
    const webServiceUrl = 'http://localhost:8727/VehicleProfileCUD/CreateVehicleProfile';
    fetchWithTokens(webServiceUrl, 'POST', vehicle)
        .then(response => {
            if (!response.ok) {
                console.log(response.statusText);
                console.log('error');
                return;
            }
            else {
                return response.json();
            }
        })
        .then( () => {
            generateVehicleProfileView()
        })
        .catch(error => {
            console.log(error);
        })
}

function postModifyVehicleProfileRequest(vehicle) {
    const webServiceUrl = 'http://localhost:8727/VehicleProfileCUD/ModifyVehicleProfile';
    fetchWithTokens(webServiceUrl, 'POST', vehicle)
        .then(response => {
            if (!response.ok) {
                console.log(response.statusText);
                console.log('error');
                return;
            }
            else {
                return response.json();
            }
        })
        .then( () => {
            generateVehicleProfileView()
        })
        .catch(error => {
            console.log(error);
        })
};

function generateModifyButton(content) {
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'Modify Vehicle';

    button.addEventListener('click', (event) => {
        event.stopImmediatePropagation();
        const buttons = document.getElementById("vehicle-details-buttons");
        buttons.remove();
        generateModifyView();
    });
    content.appendChild(button);
}

async function generateModifyView() {
    generateVehicleForm();
    const vin = document.getElementsByClassName('vin')[0].innerText;
    var formData = await getVehicleDetailsFromAPI(vin);
    if (formData) {
        fillInForm(formData);
    }
    var submit = document.getElementById('submit');
    submit.addEventListener('click', (event) => {
        event.stopImmediatePropagation();
        
        const vehicle = {
            vin: document.getElementById('vin').value.toUpperCase().trim(),
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
        const data = {
            vehicleProfile: vehicle,
            vehicleDetails: details
        };
        postModifyVehicleProfileRequest(data);


    })
}

function generateDeleteButton(content) {
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'Delete Vehicle';

    button.addEventListener('click', () => {
        console.log("clicked delete vehicle button")
    });
    content.appendChild(button);
}
function generateUploadToMarketplaceButton(content) {
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'Upload To Marketplace';

    button.addEventListener('click', () => {
        console.log("clicked marketplace vehicle button")
    });
    content.appendChild(button);
}

function generateViewServiceLogButton(content) {
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'View Service Log';

    button.addEventListener('click', () => {
        console.log("clicked service log vehicle button")
    });
    content.appendChild(button);
}

function generateDonationButton(content) {
    var vehicleElement = document.getElementById("vehicle");
    var vehicleText = vehicleElement.textContent.trim();
    var makeModelYear = vehicleText.split(" ");
    var make = makeModelYear[0];
    var model = makeModelYear[1];
    var year = makeModelYear[2];
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'Donate Vehicle';

    button.addEventListener('click', createDonateYourCarView(make, model, year));
    content.appendChild(button);
}


function extractData(jsonData,make, model, year) {
    //var data = JSON.parse(jsonData);
    var dynamicContent = document.querySelector(".dynamic-content");
    //var temp = "http://localhost:3000/";
    var html = "<div class=DYCcontainer>";
    jsonData.forEach(function(data) {
        // Access values from each object
        var charity = data.Name;
        var description = data.Description;
        var link = data.Link;
        
        // Create HTML content with the extracted values
        //html += "<a href='" + link + "' target='_blank'>";
        //html += "<a href='#' onclick='handleCharityClick(\"" + link + "\")'>"; // Call handleCharityClick function
        html += "<a href='#' onclick='handleCharityClick(\"" + link + "\", \"" + make + "\", \"" + model + "\", \"" + year + "\")'>";
        html += "<div class=charity-listings>";
        html += '<h2> Charity:' + charity+'</h2>';
        html += '<p>description: ' + description + '</p>';
        html += '</div>'; 
        html += "</a>";

    });   
    html += "</div>"; 
    dynamicContent.innerHTML = html;
   }

function createDonateYourCarView(make, model, year){
    var permissionGranted;
    fetchWithTokens('http://localhost:5212/DonateYourCar/GetAuthStatus', 'POST','')
        .then(function (response) {
        if (response.status == 204) {
            var dynamicContent = document.querySelector(".dynamic-content");
            dynamicContent.innerHTML = "";
            // replace the parameter inside changeCSS() to the path of the css file you need
            changeCSS("styles/DYCstyles.css");
            fetchWithTokens('http://localhost:5212/DonateYourCar/RetrieveCharities', 'GET','')
            .then(function (response) {
                if (response.status == 200) { 
                    response.json()
                    .then(function(data) {
                        // Check if data is an array or object
                        if (Array.isArray(data) && data.length > 0) {
                            
                            extractData(data,make, model, year);
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
        else {
            alert("Permission to view denied");
        }
    }).catch(function (error) {
        permissionGranted = false;
        alert(error);
    })
}

function handleCharityClick(link, make, model, year) {
// Generate the dynamic link with the extracted values
var dynamicLink = link + "?make=" + encodeURIComponent(make) +
"&model=" + encodeURIComponent(model) +
"&year=" + encodeURIComponent(year);

// Open the dynamic link in a new tab
window.open(dynamicLink, '_blank');
}
