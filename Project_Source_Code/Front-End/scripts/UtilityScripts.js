"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.incrementPages = exports.fetchWithTokens = void 0;
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
exports.fetchWithTokens = fetchWithTokens;
;
// 
function incrementPages() {
    var content = document.getElementById('current-page');
    var value = parseInt(content.innerText);
    value += 1;
    content.innerHTML = String(value);
}
exports.incrementPages = incrementPages;
function decrementPages() {
    var content = document.getElementById('current-page');
    var value = parseInt(content.innerText);
    value -= 1;
    if (value == 0) {
        value = 1;
    }
    content.innerHTML = String(value);
}
function pages() {
    var content = document.getElementById('pages');
    content.innerHTML = "\n        <li id='back'><</li>\n        <p id='current-page'>1</p>\n        <li id='next'>></li>\n    ";
    var next = document.getElementById('next');
    var back = document.getElementById('back');
    next.addEventListener('click', function () {
        incrementPages();
        // this is not extensible
        //getVehicles();
    });
    back.addEventListener('click', function () {
        decrementPages();
        // this is not extensible
        //getVehicles();
    });
}
