'use strict';

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
(function (root) {

    // NOT exposed to the global object ("Private" functions)
    function getVehicles(username) {
        // this should be in config file
        const webServiceUrl = 'http://localhost:8727/MyVehicleProfiles';
    
        // make this dynamic
        // retrieve values from tokens
        const userAccount = {
            "userId": username,
            "userName": "123",
            "salt": 0,
            "userHash": "string"
        }

        const data = {
            "accountUser": userAccount,
            "page": document.getElementById("current-page").innerText
        };

        const options = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json', // Specify content type as JSON
            },
            body: JSON.stringify(data)
        };

        // Insert Vehicle Creation Button Here
        // Make new function
        var content = document.getElementById('vehicle-profile-creation-button');
        content.innerHTML = `<button>Click Me</button>`;

        fetch(webServiceUrl, options)
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
                    getVehicles();
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
        const webServiceUrl = 'http://localhost:8727/MyVehicleProfileDetails';

        const car = JSON.parse(sessionStorage.getItem(id));

        const userAccount = {
            "userId": car.owner_UID,
            "userName": "123",
            "salt": 0,
            "userHash": "string"
        }
        
        const data = {
            "vehicleProfile": car,
            "accountUser": userAccount,
        };

        const options = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json', // Specify content type as JSON
            },
            body: JSON.stringify(data)
        };

        fetch(webServiceUrl, options)
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
                            <h1 id='error-message'>There are no vehcile details available for your vehicle.</h1>
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
                        <h1 id='vehicle'>${car.name}</h1>
                        <h2 id='vehicle-description'>${data[0].description}</h2>
                        <ul id='vehicle-detail-list'>
                            <li>${car.make}</li>
                            <li>${car.model}</li>
                            <li>${car.year}</li>
                            <li>${data[0].color}</li>
                            <li>${car.licensePlate}</li>
                            <li>${car.vin}</li>
                        </ul>
                    `;
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

    function getFleet(username) {
        // Get the username from the input field

        const options = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: username
        }
        
        // Send POST request to fetch fleet data
        fetch('http://localhost:5145/rentals/RentalFleet/GetFleet', options)
            .then(response => {
                if (!response.ok) {
                    console.log('error');
                    return;
                }
                else {
                    return response.json()
                }
            })
            .then(data => {
                // Handle the response data
                displayFleetModels(data);
            })
            .catch(error => {
                alert('Error fetching fleet data:', error);
            });
    }

    function displayFleetModels(fleetModels) {
        const content = document.getElementsByClassName('dynamic-content')[0];
        content.innerHTML = `<div id="fleetContainer"></div>`;
        const fleetContainer = document.getElementById('fleetContainer');
        
        // Clear existing content
        fleetContainer.innerHTML = '';
    
        // Loop through each fleet model object and create corresponding HTML elements
        fleetModels.forEach(fleetModel => {
            const fleetModelDiv = document.createElement('div');
            fleetModelDiv.classList.add('fleet-model');
    
            // Populate the fleet model details
            fleetModelDiv.innerHTML = `
                <h2>Fleet Model</h2>
                <p>VIN: ${fleetModel.vin}</p>
                <p>Make: ${fleetModel.make}</p>
                <p>Model: ${fleetModel.model}</p>
                <p>Year: ${fleetModel.year}</p>
                <p>Date Created: ${fleetModel.dateCreated}</p>
                <p>Color: ${fleetModel.color}</p>
                <p>Status: ${fleetModel.status}</p>
                <p>Status Info: ${fleetModel.statusInfo}</p>
                <p>Expected Return Date: ${fleetModel.expectedReturnDate}</p>
            `;
    
            // Append the fleet model element to the container
            fleetContainer.appendChild(fleetModelDiv);
        });
    }
    
    function incrementPages() {
        var content = document.getElementById('current-page');
        var value = parseInt(content.innerText);
        value += 1;
        content.innerHTML = value;
    }

    function decrementPages() {
        var content = document.getElementById('current-page');
        var value = parseInt(content.innerText);
        value -= 1;
        if (value == 0){
            value = 1;
        }        
        content.innerHTML = value;
    }

    function pages() {
        var content = document.getElementById('pages');

        content.innerHTML = `
            <li id='back'><</li>
            <p id='current-page'>1</p>
            <li id='next'>></li>
        `;
        var next = document.getElementById('next');
        var back = document.getElementById('back');

        next.addEventListener('click', () => {
            incrementPages();
            getVehicles();
        });
        back.addEventListener('click', () => {
            decrementPages();
            getVehicles();
        });
    }

    function resetView() {
        var content = document.getElementsByClassName('dynamic-content')[0];
        
        content.innerHTML = '<label for="username">Username: </label><input type="text" id="username" name="username">'
    }

    // Initialize the current view by attaching event handlers 
    function init() {
        var login = document.getElementById('login-view');
        var vp = document.getElementById('vehicle-profile-view');
        var rf = document.getElementById('rental-fleet-view');
        var im = document.getElementById('inventory-management-view');

        // Dynamic event registration
        login.addEventListener('click', () => resetView());
        vp.addEventListener('click', () => {
            var username = document.getElementById('username');
            if (username != null)
            {
                username = parseInt(username.value);
            }
            else {
                return error;
            }
            var content = document.getElementsByClassName('dynamic-content')[0];
            content.innerHTML = `<div id='vehicle-profile-creation-button'></div>`
            content.innerHTML += `<div id='vehicle-profile'></div>`
            content.innerHTML += `<nav id='pages'></nav>`
            content.innerHTML += '<div id="vehicle-details"></div>'
            pages();
            getVehicles(username);
        });
        rf.addEventListener('click', () => {
            var username = document.getElementById('username');
            if (username != null)
            {
                username = parseInt(username.value);
            }
            else {
                return username;
            }
            getFleet(username);
        });
        im.addEventListener('click', () => {

        });

        resetView();
    }

    init();

})(window);