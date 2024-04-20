// Note, doing the import function breaks the files functionality. I am unsure why this is the case, but I have not found a solution
//import { fetchWithTokens } from "./FetchWithTokens";

function fetchWithTokens(url, method, body) {
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
    var logOutNav = document.getElementById("log-out");
    logOutNav.addEventListener("click", logOut);
    var refreshPermissionsNav = document.getElementById("refresh-permissions");
    refreshPermissionsNav === null || refreshPermissionsNav === void 0 ? void 0 : refreshPermissionsNav.addEventListener("click", refreshUserTokens);
    var vehicleProfileNav = document.getElementById("vehicle-profile-view");
    vehicleProfileNav.addEventListener("click", function (doAThing) { alert("beep boop"); });
    var rentalFleetNav = document.getElementById("rental-fleet-view");
    rentalFleetNav.addEventListener("click", generateRentalDefaultView);
    var carHealthRatingNav = document.getElementById("car-health-rating-view");
    carHealthRatingNav.addEventListener("click", generateCarHealthRatingDefaultView);
});
function generateRentalDefaultView() {
    var permissionGranted;
    fetchWithTokens('http://localhost:8081/Rentals/GetAuthStatus', 'POST', '')
        .then(function (response) {
        if (response.status == 204) {
            alert("permission granted!!!");
            var dynamicContent = document.querySelector(".dynamic-content");
            dynamicContent.innerHTML = "\n            <div id=\"fleet-button-div\">\n                <button id=\"submit-username\">Submit</button>\n            </div>\n            ";
        }
        else {
            alert("Permission to view denied");
        }
    }).catch(function (error) {
        permissionGranted = false;
        alert(error);
    });
}
;
function generateCarHealthRatingDefaultView()
{
    var permissionGranted;
    fetchWithTokens('http://localhost:8082/CarHealthRating/GetAuthStatus', 'POST', '')
        .then(function (response) {
        if (response.status == 204) {
            alert("Permission Granted!");
            var dynamicContent = document.querySelector(".dynamic-content");
            dynamicContent.innerHTML = "";
            generateVehicleProfileRetrieval();
        }
        else {
            alert("Permission to view denied");
        }
    }).catch(function (error) {
        permissionGranted = false;
        alert(error);
    })
}
;
function logOut() {
    // Remove the tokens from storage
    sessionStorage.removeItem('IDToken');
    sessionStorage.removeItem('AccessToken');
    sessionStorage.removeItem('RefreshToken');
    // Give user confirmation they've logged out
    var dynamicContent = document.querySelector(".dynamic-content");
    dynamicContent.innerHTML = "\n    <div id=\"logout-view\">\n        Logged Out!!!\n    </div>\n    ";
    // Lets the view show for 5 seconds, then reloads the page
    setTimeout(function () {
        location.reload();
    }, 5000);
}
function refreshUserTokens() {
    fetchWithTokens('http://localhost:8080/Auth/refreshTokens', 'POST', '')
        .then(function (response) {
        if (response.ok) {
            return response.json();
        }
        else {
            alert("Refresh Failed! " + response.statusText);
            return;
        }
    })
        .then(function (data) {
        try {
            sessionStorage.setItem('IDToken', data.idToken);
            sessionStorage.setItem('AccessToken', data.accessToken);
            alert("Your session has been refreshed!!!!");
        }
        catch (_a) { }
        ;
    })
        .catch(function (error) {
        alert("An error occurred while Refreshing your session: " + error);
    });
}

