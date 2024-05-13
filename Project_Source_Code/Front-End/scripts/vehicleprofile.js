'use strict';
(async function() {
    //#region Initial Setup
    var vehicleProfileURL = "";
    var CONFIG = (await fetchConfig('./configs/RideAlong.config.json')
        .then(response => {
            if (!response.ok) {
                throw "Could not read config file";
            }
            return response.json();
        })
        .catch((error) => {
            console.error(error);
        }));
    vehicleProfileURL = CONFIG["ip"] + ':' + CONFIG["ports"]["vehicleProfile"]
    //#endregion

    //#region Vehicle Profile View
    function createVehicleProfileView() {
        refreshUserTokens();
        //PAGINATION
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
    
        var vehicleProfile = document.createElement('div');
        vehicleProfile.id = 'vehicle-profile';
    
        dynamicContent.appendChild(vehicleProfile);
    
        pages(createVehicleProfileView);
        document.getElementById('current-page').innerText = currPage;
        const webServiceUrl = vehicleProfileURL + '/VehicleProfileRetrieve/MyVehicleProfiles';
    
        var popup = document.createElement('div');
        popup.id = 'vehicle-details';
    
        document.getElementsByClassName('dynamic-content')[0].appendChild(popup);
        
        generateCreateButton()
    
        fetchWithTokens(webServiceUrl, 'POST', currPage)
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
                else {
                    return response.json();
                }
            })
            .then(data => {
                if (data.length == 0 && parseInt(document.getElementById('current-page').innerText) != 1)
                {
                    decrementPages();
                    createVehicleProfileView();
                    return;
                }
            //PAGINATION
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
                decrementPages();
                alert(error)
            });
    }
    window.createVehicleProfileView = createVehicleProfileView;
    //#endregion

    //#region Get Vehicle Details
    async function getVehicleDetails(id) {
        var response;
        var content;
        try {
            response = await postGetVehicleDetailsFromAPI(id);
            
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
                generateDeleteFromMarketplaceButton(buttons)
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
    
    async function postGetVehicleDetailsFromAPI(id) {
        const webServiceUrl = vehicleProfileURL + '/VehicleProfileRetrieve/MyVehicleProfileDetails';
    
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
                throw error;
            })
    }
    //#endregion
    
    //#region Creation
    function generateCreateButton() {
        var createButton = document.createElement('input');
        createButton.type = 'button';
        createButton.id = 'create-vehicle-button';
        createButton.value = 'Create Vehicle';
    
        createButton.addEventListener('click', (event) => {
            event.stopPropagation();
    
            generateCreationView();
        });
        var dynamicContent = document.getElementsByClassName('dynamic-content')[0];
        dynamicContent.insertBefore(createButton, dynamicContent.children[0]);
    }
    
    function generateCreationView() {
        var content = document.getElementById('vehicle-details');
        content.innerHTML = '';
        content.style.display = "block";
    
        // Create H1 element 
        var title = document.createElement('h1');
        title.textContent = "Create Vehicle";
        content.appendChild(title);
    
        // Create paragraph element. Displays the instructions use the VIN retrieval
        var message = document.createElement('p');
        message.textContent = "Input vehicle VIN and click the button below to automatically retrieve the Make Model and Year.";
        content.appendChild(message);
    
        // Generate a form that lets the user input vin, licenseplate, make, model, year, color, and description
        generateVehicleForm();
    
        // Generates button to get vehicle make model and year
        var form = document.getElementById('form');
        var getMMYButton = document.createElement('input');
        getMMYButton.type = 'button'
        getMMYButton.id = 'get-MMY-button';
        getMMYButton.value = 'Get Make Model Year';
        getMMYButton.addEventListener('click', async () => {
            var vinInput = document.getElementById('vin');
            try {
                var data = await retrieveMMYFromAPI(vinInput.value);
                fillInVehicleForm(data);
            } catch (error) {
                alert(error);
            }
            
        });
        
        form.insertBefore(getMMYButton, form.children[1]);
        
    
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
                if (!response.ok)
                {
                    throw "Could not retrieve from NHTSA API"
                }
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
                throw error;
            })
    }

    function postCreateVehicleProfileRequest(vehicle) {
        const webServiceUrl = vehicleProfileURL + '/VehicleProfileCUD/CreateVehicleProfile';
        fetchWithTokens(webServiceUrl, 'POST', vehicle)
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

    //#region Vehicle Forms
    function fillInVehicleForm(formData)
    {
        for (const property in formData) {
            if (formData[property] != '')
            {
                document.getElementById(`${property}`).value = formData[property];
            }
        }
    }
    window.fillInVehicleForm = fillInVehicleForm;

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
    window.generateVehicleForm = generateVehicleForm;
    //#endregion
    
    //#region Modify Vehicle
    async function postModifyVehicleProfileRequest(vehicle) {
        const webServiceUrl = vehicleProfileURL + '/VehicleProfileCUD/ModifyVehicleProfile';
        return await fetchWithTokens(webServiceUrl, 'POST', vehicle)
            .then(response => {
                if (response.status == 403) {
                    throw `You do not have permission to edit vehicle: ${vehicle.vin}.`;
                }
                else if (response.status == 400) {
                    throw `Insufficient details provided.`
                }
                else if (response.status == 404) {
                    throw `Could not modify vehicle`;
                }
                else {
                    sessionStorage.setItem(vehicle.vin, JSON.stringify(vehicle));
                    return response.json();
                }
            })
            .catch(error => {
                throw error;
            })
    }
    
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
        document.getElementById('vin').readOnly = true;
        const vin = document.getElementsByClassName('vin')[0].innerText;
        var formData = await postGetVehicleDetailsFromAPI(vin);
        if (formData) {
            fillInVehicleForm(formData);
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
            try {
                postModifyVehicleProfileRequest(data);
            }
            catch (error) {
                alert(error);
                
            }
            finally {
                refreshUserTokens();
                generateVehicleProfileView();
            }
        })
    }
    //#endregion

    //#region Delete Vehicle    
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

    function generateDeleteView() {
        // Get vin before deleting elements
        const vin = document.getElementsByClassName('vin')[0].innerText;

        // Delete elements in the deatils box
        var vehicleDetails = document.getElementById("vehicle-details");
        while (vehicleDetails.lastChild) {
            vehicleDetails.removeChild(vehicleDetails.lastChild);
        }

        // Create new elements
        var message = document.createElement('h1');
        message.id = 'message';
        message.innerText = "Are you sure you want to delete your vehicle?";

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
                alert('Vehicle successfully deleted');
                generateVehicleProfileView();
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
    }

    async function postDeleteVehicleProfile(vehicle)
    {
        const webServiceUrl = vehicleProfileURL + '/VehicleProfileCUD/DeleteVehicleProfile';
        return await fetchWithTokens(webServiceUrl, 'POST', vehicle)
            .then(response => {
                if (response.status == 403) {
                    throw `You do not have permission to delete vehicle: ${vehicle.vin}.`;
                }
                else if (response.status == 400) {
                    throw `Insufficient details provided.`;
                }
                else if (response.status == 404) {
                    throw `Could not delete vehicle`;
                }
                else {
                    sessionStorage.removeItem(vehicle.vin);
                    return response.json();
                }
            })
            .catch(error => {
                throw error;
            })
    }
    //#region 

    //#region Delete from Marketplace Button 
    function generateDeleteFromMarketplaceButton(content) {
        var button = document.createElement('input');
        button.type = 'button';
        button.value = 'Delete From Marketplace';
        var vehicleElement = document.getElementById("vehicle");
        var vehicleDetail = document.getElementById("vehicle-detail-list");
        var list = vehicleDetail.getElementsByTagName("li");
        var vehicleText = vehicleElement.textContent.trim();
        var makeModelYear = vehicleText.split(" ");
        var make = makeModelYear[0];
        var model = makeModelYear[1];
        var VIN = list[2].textContent;
          
        button.addEventListener('click', (event) => {
            event.stopImmediatePropagation();
            deleteFromMarketplace(VIN,model)
        });
        content.appendChild(button);
    }
    //#endregion 

    //#region Upload to Marketplace Button
    function generateUploadToMarketplaceButton(content) {
        var button = document.createElement('input');
        button.type = 'button';
        button.value = 'Upload To Marketplace';
        var vehicleElement = document.getElementById("vehicle");
        var vehicleDetail = document.getElementById("vehicle-detail-list");
        var list = vehicleDetail.getElementsByTagName("li");
        var vehicleText = vehicleElement.textContent.trim();
        var makeModelYear = vehicleText.split(" ");
        var make = makeModelYear[0];
        var model = makeModelYear[1];
        var VIN = list[2].textContent;
          
        button.addEventListener('click', (event) => {
            event.stopImmediatePropagation();
            uploadToMarketplace(VIN,model)
        });
        content.appendChild(button);
    }
    //#endregion

    //#region Service Log Button    
    function generateViewServiceLogButton(content) {
        var button = document.createElement('input');
        button.type = 'button';
        button.value = 'View Service Log';
    
        button.addEventListener('click', () => {
            console.log("clicked service log vehicle button")
        });
        content.appendChild(button);
    }
    //#endregion
    
    //#region Donations
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

        button.addEventListener('click', (event) => {
            event.stopImmediatePropagation();
            createDonateYourCarView(make, model, year)
        });
        content.appendChild(button);
    }
    //#endregion
})(window);