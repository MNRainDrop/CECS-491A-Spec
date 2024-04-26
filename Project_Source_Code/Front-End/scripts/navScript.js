document.addEventListener("DOMContentLoaded", function () {
    //const rentalFleetNav = document.getElementById("rental-fleet-view");
    //const inventoryManagementNav = document.getElementById("inventory-management-view");
    //const vehicleMarketPlaceNav = document.getElementById("vehicle-marketplace-view");
    var logOutNav = document.getElementById("log-out");
    logOutNav.addEventListener("click", logOut);

    var refreshPermissionsNav = document.getElementById("refresh-permissions");
    refreshPermissionsNav === null || refreshPermissionsNav === void 0 ? void 0 : refreshPermissionsNav.addEventListener("click", refreshUserTokens);
    
    var vehicleProfileNav = document.getElementById("vehicle-profile-view");
    vehicleProfileNav.addEventListener("click", generateVehicleProfileView);
    
    var rentalFleetNav = document.getElementById("rental-fleet-view");
    rentalFleetNav.addEventListener("click", generateRentalDefaultView);
    
    var carHealthRatingNav = document.getElementById("car-health-rating-view");
    carHealthRatingNav.addEventListener("click", generateCarHealthRatingDefaultView);
    
    var vehicleMarketPlaceNav = document.getElementById("vehicle-marketplace-view");
    vehicleMarketPlaceNav.addEventListener("click", generateVehicleMarketplaceDefaultView);
});

function generateRentalDefaultView() {
    var permissionGranted;
    fetchWithTokens('http://localhost:8081/Rentals/GetAuthStatus', 'POST', '')
        .then(function (response) {
        if (response.status == 204) {
            alert("permission granted!!!");
            // replace the parameter inside changeCSS() to the path of the css file you need
            changeCSS()
            var dynamicContent = document.querySelector(".dynamic-content");
            dynamicContent.innerHTML = '<div id="fleet-button-div"><button id="submit-username">Submit</button></div>';
        }
        else {
            alert("Permission to view denied");
        }
    }).catch(function (error) {
        permissionGranted = false;
        alert(error);
    });
};

function generateCarHealthRatingDefaultView()
{
    var permissionGranted;
    fetchWithTokens('http://localhost:8082/CarHealthRating/GetAuthStatus', 'POST', '')
        .then(function (response) {
        if (response.status == 204) {
            alert("Permission Granted!");
            // replace the parameter inside changeCSS() to the path of the css file you need
            changeCSS()
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
};

function generateVehicleProfileView()
{
    fetchWithTokens('http://localhost:8727/VehicleProfileRetrieve/PostAuthStatus', 'POST', '')
        .then(function (response) {
        if (response.status == 204) {
            var dynamicContent = document.querySelector(".dynamic-content");
            dynamicContent.innerHTML = "";
            
            dynamicContent.innerHTML = `<div id='vehicle-profile-creation-button'></div>`
            dynamicContent.innerHTML += `<div id='vehicle-profile'></div>`
            dynamicContent.innerHTML += `<nav id='pages'></nav>`
            dynamicContent.innerHTML += '<div id="vehicle-details"></div>'
            // replace the parameter inside changeCSS() to the path of the css file you need
            changeCSS("styles/VPstyles.css")
            pages(createVehicleProfileView);
            createVehicleProfileView();
        }
        else {
            alert("Permission to view denied");
        }
    }).catch(function (error) {
        alert(error);
    })
};
  
function generateVehicleMarketplaceDefaultView()
{
    var permissionGranted;
    fetchWithTokens('http://localhost:5104/VehicleMarketplace/GetAuthStatus', 'POST','')
        .then(function (response) {
        if (response.status == 204) {
            alert("Permission Granted!");
            var dynamicContent = document.querySelector(".dynamic-content");
            dynamicContent.innerHTML = "";
            // replace the parameter inside changeCSS() to the path of the css file you need
            changeCSS("styles/VPMstyles.css");
            displayMarketplace();
        }
        else {
            alert("Permission to view denied");
        }
    }).catch(function (error) {
        permissionGranted = false;
        alert(error);
    })
};

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

