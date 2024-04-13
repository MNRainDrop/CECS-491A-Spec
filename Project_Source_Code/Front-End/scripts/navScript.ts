// Note, doing the import function breaks the files functionality. I am unsure why this is the case, but I have not found a solution
//import { fetchWithTokens } from "./FetchWithTokens";
function fetchWithTokens(url: string, method: string, body: any) {  
    // Fetch the tokens from session storage
    const idToken = sessionStorage.getItem('IDToken');
    const accessToken = sessionStorage.getItem('AccessToken');
    const refreshToken = sessionStorage.getItem('RefreshToken');
    // Create the headers object with the tokens
    const headers : HeadersInit = {
        'Authorization': `Bearer ${idToken}` ?? '',
        'X-Access-Token': accessToken ?? '',
        'X-Refresh-Token': refreshToken ?? '',
        'Content-Type': 'application/json' // Set the Content-Type header for the request body
    };
    return fetch(url, {
        method: method,
        headers: headers,
        body: JSON.stringify(body)
    });
};

document.addEventListener("DOMContentLoaded", () => {
    //const rentalFleetNav = document.getElementById("rental-fleet-view");
    //const inventoryManagementNav = document.getElementById("inventory-management-view");
    //const vehicleMarketPlaceNav = document.getElementById("vehicle-marketplace-view");
    
    const vehicleProfileNav = document.getElementById("vehicle-profile-view");
    vehicleProfileNav!.addEventListener("click", doAThing => {alert("beep boop")});
    const rentalFleetNav = document.getElementById("rental-fleet-view");
    //rentalFleetNav!.addEventListener("click", () => {alert("boop beep");});
    
    rentalFleetNav!.addEventListener("click", generateRentalView);
});

function generateRentalView () {
    alert("Got Here");
    var permissionGranted;
    fetchWithTokens('http://localhost:8081/Rentals/GetAuthStatus', 'POST', '')
    .then(function (response) {
        if(response.ok){
            permissionGranted = true;
        } else {
            permissionGranted = false;
        }
    }).catch(error => {
        permissionGranted = false;
        alert(error);
    });
    if (permissionGranted)
    {
        const dynamicContent = document.querySelector(".dynamic-content");
        dynamicContent!.innerHTML = `
        <div id="fleet-button-div">
            <button id="submit-username">Submit</button>
        </div>
        `;
    }
};