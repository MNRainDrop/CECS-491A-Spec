'use strict;'
// Validators
function isValidEmail(email) {
    // Minimum length check
    if (email.length < 3)
        return false;

    // Regular expression pattern for email validation
    var pattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9.-]{1,}$/;

    // Check if the email matches the pattern
    return pattern.test(email);
}

var isValidOTP = function (OTP) {
    // allows for OTP to be all numbers/letters
    var otpPattern = /^[a-zA-Z0-9]{10}$/;
    return otpPattern.test(OTP);
};
// Event Listeners
document.addEventListener("DOMContentLoaded", function () {
    var submitUsernameButton = document.getElementById("login-button");
    submitUsernameButton.addEventListener("click", submitUsername);

});


(async function() {
    //#region Initial Setup
    var webURL = "";
    var CONFIG = (await fetchConfig('./configs/RideAlong.config.json')
        .then(response => {
            if (!response.ok) {
                throw "Could not read config file";
            }
            return response.json();
        })
        .catch((error) => {
            console.error(error);
        }));
    webURL = CONFIG["ip"] + ':' + CONFIG["ports"]["security"]
    window.webURL = webURL;
}) (window);

// Implementation
function submitUsername() {
    var textInput = document.getElementById("text-input");
    var username = textInput.value.trim();
    // Check if username is not empty
    if (username) {
        if (isValidEmailAddress(username)) {
            // Calls Web API controller
            fetch(webURL + "/Auth/startLogin", {
                method: "POST",
                body: JSON.stringify(username),
                headers: {
                    "Content-Type": "application/json"
                }
            })
                .then(function (response) {
                if (response.ok) {
                    // If response is OK, display OTP view
                    showOTPView();
                    return response.text();
                }
                else {
                    // If response is not OK, display to user process failed
                    alert("Authentication failed!");
                    console.error("Error:", response.statusText);
                }
            })
                .then(function (data) {
                alert(data);
            })
                .catch(function (error) {
                alert("Something went wrong!");
                console.error("Error:", error);
            });
        }
        else {
            alert("Please retry entering your username!");
        }
    }
    else {
        alert("Username cannot be empty!");
        console.error("Username cannot be empty.");
    }
};

function submitOTP() {
    var textInput = document.getElementById("text-input");
    var otp = textInput.value.trim();
    // Retrieve the username from the paragraph element
    var usernameParagraph = document.querySelector("#username");
    var username = usernameParagraph.textContent.replace('Username: ', '').trim();
    // Check if OTP is not empty
    if (otp && username) {
        if (isValidOTP(otp) && isValidEmailAddress(username)) {
            var fetchResponse = false; // Assuming Fail response by default
            // Make fetch request to back - end
            fetch(webURL + "/Auth/tryAuthentication", {
                method: "POST",
                body: JSON.stringify({ username: username, otp: otp }),
                headers: {
                    "Content-Type": "application/json"
                }
            })
            .then(function (response) {
            if (response.ok) {
                return response.json();
            }
            else {
                alert("Authentication failed!");
                console.error("Error:", response.statusText);
            }
            })
            .then(function (data) {
            sessionStorage.setItem('IDToken', data.idToken);
            sessionStorage.setItem('AccessToken', data.accessToken);
            sessionStorage.setItem('RefreshToken', data.refreshToken);
            showMainContent();
            unhideNavigation();
            })
            .catch(function (error) {
            alert("Something went wrong!" + " " + error);
            console.error("Error:", error);
            });
        }
        else {
            alert("You have entered a bad OTP");
        }
    }
    else {
        alert("OTP cannot be empty!");
    }
};

// Views
function showOTPView() {
    // Remove the other options from login page
    var loginForm = document.querySelector(".login-form");
    while (loginForm.lastChild.id !== 'login-button') {
        loginForm.removeChild(loginForm.lastChild);
    }
    var submitButton = document.getElementById("login-button");
    submitButton.value = "Let's roll";

    // Create an h2 element to display the username
    var usernameH2 = document.createElement('h2');
    var username = document.getElementById("text-input").value.trim();
    usernameH2.id = 'username';
    usernameH2.textContent = 'Username: ' + username;
    document.querySelector(".messages").appendChild(usernameH2);

    // Create a paragraph element to tell user what to do next
    var message = document.createElement('h2');
    message.innerHTML = `An OTP has been sent to your email address.<br>Please enter the OTP`
    document.querySelector(".messages").appendChild(message);

    // Change contents of input text field
    var textInput = document.getElementById("text-input");
    textInput.value = "";
    textInput.placeholder = "One Time Password";

    // Attach event listener to submit button
    submitButton.removeEventListener('click', submitUsername);
    submitButton.addEventListener('click', submitOTP);
};

function showMainContent() {
    changeCSS();
    var dynamicContent = document.querySelector(".dynamic-content");
    dynamicContent.innerHTML = "\n        <div id=\"main-content\">\n            <p>Welcome, user! You are now logged in.</p>\n            <p>Welcome to the <b>Ride-Along</b> Application!<br>We are currently working on this page to make it better suited for you!</p>\n            <p>Work in progresss: Security, Vehicle Profile, and Service Log</p>\n        </div>\n    ";
};

function unhideNavigation() {
    // Updating nav
    var navigation = document.getElementById("navigation");
    navigation.classList.remove("hidden");
};
