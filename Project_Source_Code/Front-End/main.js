'use strict';

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
(function (root) {
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