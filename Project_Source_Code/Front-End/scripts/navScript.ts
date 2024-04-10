import { fetchWithTokens } from "./FetchWithTokens";

document.addEventListener("DOMContentLoaded", function () {
    const vehicleProfileNav = document.getElementById("vehicle-profile-view");
    const rentalFleetNav = document.getElementById("rental-fleet-view");
    const inventoryManagementNav = document.getElementById("inventory-management-view");
    const vehicleMarketPlaceNav = document.getElementById("vehicle-marketplace-view");

    rentalFleetNav!.addEventListener("click", generateRentalView);
    vehicleProfileNav!.addEventListener("click", doAThing => {alert("beep boop")})
});

function generateRentalView () {
    alert("Got Here");
    var permissionGranted;
    fetchWithTokens('http://localhost:8081/Rentals/GetAuthStatus', 'GET', '')
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