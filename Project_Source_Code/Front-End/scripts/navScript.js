"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var FetchWithTokens_1 = require("./FetchWithTokens");
document.addEventListener("DOMContentLoaded", function () {
    var vehicleProfileNav = document.getElementById("vehicle-profile-view");
    var rentalFleetNav = document.getElementById("rental-fleet-view");
    var inventoryManagementNav = document.getElementById("inventory-management-view");
    var vehicleMarketPlaceNav = document.getElementById("vehicle-marketplace-view");
    rentalFleetNav.addEventListener("click", generateRentalView);
    vehicleProfileNav.addEventListener("click", function (doAThing) { alert("beep boop"); });
});
function generateRentalView() {
    alert("Got Here");
    var permissionGranted;
    (0, FetchWithTokens_1.fetchWithTokens)('http://localhost:8081/Rentals/GetAuthStatus', 'GET', '')
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
