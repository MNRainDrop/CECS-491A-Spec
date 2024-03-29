document.addEventListener("DOMContentLoaded", function() {
    // Add event listener to the fetch fleet button
    const fetchFleetButton = document.getElementById('fetchFleetButton');
    fetchFleetButton.addEventListener('click', function() {
        // Get the username from the input field
        const username = document.getElementById('username').value;

        // Send POST request to fetch fleet data
        fetch('http://localhost:5145/rentals/GetFleet', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                uid: username
            })
        })
        .then(response => response.json())
        .then(data => {
            // Handle the response data
            displayFleetModels(data);
        })
        .catch(error => {
            alert('Error fetching fleet data:', error);
        });
    });
});

function displayFleetModels(fleetModels) {
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
