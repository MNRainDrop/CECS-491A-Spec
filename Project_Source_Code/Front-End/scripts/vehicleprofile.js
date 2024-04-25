
function getVehicles(username) {
    // this should be in config file
    const webServiceUrl = 'http://localhost:8727/VehicleProfileRetrieve/MyVehicleProfiles';

    const data = {
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
   
    const data = {
        "vehicleProfile": car
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