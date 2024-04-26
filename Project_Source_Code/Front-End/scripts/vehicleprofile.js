function vehicleProfileView() {
    // this should be in config file
    const webServiceUrl = 'http://localhost:8727/VehicleProfileRetrieve/MyVehicleProfiles';

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
                var content = document.getElementById('vehicle-details');
                content.innerHTML= `
                    <h1 id='vehicle'>${car.make} ${car.model} ${car.year}</h1>
                    <h2 id='vehicle-description'>${data[0].description}</h2>
                    <ul id='vehicle-detail-list'>
                        <li>${data[0].color}</li>
                        <li>${car.licensePlate}</li>
                        <li>${car.vin}</li>
                    </ul>
                `;
                generateModifyButton()
                generateDeleteButton()
                content.style.display = "block";

                document.addEventListener('click', (event) => {
                    if (!content.contains(event.target)) {
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
    var content = document.getElementById('vehicle-profile-creation-button');
    content.innerHTML = `<button id='create-vehicle-button'>Create Vehicle Profile</button>`;

    const createButton = document.getElementById('create-vehicle-button');
    createButton.addEventListener('click', (event) => {
        event.stopPropagation();
        generateVehicleForm();
        
        var formData = retrieveFromAPI();

        if (formData !== null) {
            fillInForm(formData);
        }
        document.addEventListener('submit', (event) => {
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
}

function generateVehicleForm() {
    var content = document.getElementById('vehicle-details');
    content.innerHTML = '';
    content.style.display = "block";

    var form = document.createElement('form');

    var title = document.createElement('h1');
    title.textContent = "Create Vehicle";

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
    inputSubmit.type = 'submit';
    inputSubmit.value = 'Submit';
    var inputCancel = document.createElement('input');
    inputCancel.type = 'button';
    inputCancel.value = 'Cancel';
    inputCancel.id = 'cancel';

    form.appendChild(title);
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

function retrieveFromAPI() {

}

function fillInForm(formData)
{
    var vinInput = document.getElementById('vin');
    vinInput.value = formData.vin;
    var licensePlateInput = document.getElementById('licensePlate');
    licensePlateInput.value = formData.licensePlate;
    var makeInput = document.getElementById('make');
    makeInput.value = formData.make;
    var modelInput = document.getElementById('model');
    modelInput.value = formData.model;
    var yearInput = document.getElementById('year');
    yearInput.value = formData.year;
    var colorInput = document.getElementById('color');
    colorInput.value = formData.color;
    var descriptionInput = document.getElementById('description');
    descriptionInput.value = formData.description;
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
        .then(
            vehicleProfileView()
        )
        .catch(error => {
            console.log(error);
        })
}

function generateModifyButton() {

}

function generateDeleteButton() {

}