// Note, doing the import function breaks the files functionality. I am unsure why this is the case, but I have not found a solution
//import { fetchWithTokens } from "./FetchWithTokens";
function fetchWithTokens(url, method, body) {
    alert("fetching with tokens");
    var _a;
    // Fetch the tokens from session storage
    var idToken = sessionStorage.getItem('IDToken');
    var accessToken = sessionStorage.getItem('AccessToken');
    var refreshToken = sessionStorage.getItem('RefreshToken');
    // Create the headers object with the tokens
    var headers = {
        'Authorization': (_a = "Bearer ".concat(idToken)) !== null && _a !== void 0 ? _a : '',
        'X-Access-Token': accessToken !== null && accessToken !== void 0 ? accessToken : '',
        'X-Refresh-Token': refreshToken !== null && refreshToken !== void 0 ? refreshToken : '',
        'Content-Type': 'application/json' // Set the Content-Type header for the request body
    };
    return fetch(url, {
        method: method,
        headers: headers,
        body: JSON.stringify(body)
    });
}
;
document.addEventListener("DOMContentLoaded", function () {
    //const rentalFleetNav = document.getElementById("rental-fleet-view");
    //const inventoryManagementNav = document.getElementById("inventory-management-view");
    //const vehicleMarketPlaceNav = document.getElementById("vehicle-marketplace-view");
    var vehicleProfileNav = document.getElementById("vehicle-profile-view");
    vehicleProfileNav.addEventListener("click", function (doAThing) { alert("beep boop"); });
    var rentalFleetNav = document.getElementById("rental-fleet-view");
    //rentalFleetNav!.addEventListener("click", () => {alert("boop beep");});
    rentalFleetNav.addEventListener("click", generateRentalView);
});
function generateRentalView() {
    alert("Got Here");
    var permissionGranted;
    fetchWithTokens('http://localhost:8081/Rentals/GetAuthStatus', 'POST', '')
        .then(function (response) {
        if (response.ok) {
            permissionGranted = true;
        }
        else {
            permissionGranted = false;
        }
    }).catch(function (error) {
        permissionGranted = false;
        alert(error);
    });
    if (permissionGranted) {
        var dynamicContent = document.querySelector(".dynamic-content");
        dynamicContent.innerHTML = "\n        <div id=\"fleet-button-div\">\n            <button id=\"submit-username\">Submit</button>\n        </div>\n        ";
    }
}
;
