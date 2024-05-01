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

function getVehicleDetails(id) {
    // this should be in config file
    const webServiceUrl = 'http://localhost:8727/VehicleProfileRetrieve/MyVehicleProfileDetails';

    const car = JSON.parse(sessionStorage.getItem(id));

    fetchWithTokens(webServiceUrl, 'POST', car)
        .then(response => {
            if (!response.ok) {
                var content = document.getElementById('vehicle-details');
                content.innerHTML= `
                    <h1 id='error-message'>Could not retrieve vehicle details. Try again later.</h1>
                `;
                
                content.style.display = "block";
                document.addEventListener('click', (event) => {
                    if (!content.contains(event.target)) {
                        content.innerHTML = '';
                        content.style.display = "none";
                    }
                })

                return null;
            }
            else {
                if (response.status == 202)
                {
                    var content = document.getElementById('vehicle-details');
                    content.innerHTML= `
                        <h1 id='error-message'>There are no vehicle details available for your vehicle. Would you like to add some?</h1>
                    `;
                    content.style.display = "block";
                    document.addEventListener('click', (event) => {
                        if (!content.contains(event.target)) {
                            content.innerHTML = '';
                        content.style.display = "none";
                        }
                    })
                    return null;
                }
                return response.json();
            }
        })
        .then(data => {
            if (data == null)
            {
                return;
            }
            if (data.length == 1)
            {
                const content = document.getElementById('vehicle-details');
                content.innerHTML= `
                    <h1 id='vehicle'>${car.make} ${car.model} ${car.year}</h1>
                    <h2 id='vehicle-description'>${data[0].description}</h2>
                    <ul id='vehicle-detail-list'>
                        <li>${data[0].color}</li>
                        <li>${car.licensePlate}</li>
                        <li>${car.vin}</li>
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

        })
        .catch(error => {
            console.log(error);
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

function generateModifyButton(content) {
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'Modify Vehicle';

    button.addEventListener('click', generateModifyView);
    content.appendChild(button);
}

function generateModifyView() {
    generateVehicleForm();
    fillInForm();
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
    var button = document.createElement('input');
    button.type = 'button';
    button.value = 'Donate Vehicle';

    button.addEventListener('click',  () => {
        console.log("clicked donate vehicle button")
    });
    content.appendChild(button);
}

