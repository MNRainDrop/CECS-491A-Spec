// Validators
var isValidEmailAddress = function (email) {
    var emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailPattern.test(email);
};
var isValidOTP = function (OTP) {
    var otpPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/;
    return otpPattern.test(OTP);
};
// Event Listeners
document.addEventListener("DOMContentLoaded", function () {
    //const vehicleProfileNav = document.getElementById("vehicle-profile-view");
    //vehicleProfileNav!.addEventListener("click", doAThing => {alert("beep boop")});
    //const rentalFleetNav = document.getElementById("rental-fleet-view");
    //rentalFleetNav!.addEventListener("click", () => {alert("boop beep");});
    var submitUsernameButton = document.getElementById("submit-username");
    submitUsernameButton.addEventListener("click", submitUsername);
});
// Implementation
function submitUsername() {
    var usernameInput = document.getElementById("username-input");
    var username = usernameInput.value.trim();
    // Check if username is not empty
    if (username) {
        if (isValidEmailAddress(username)) {
            // Calls Web API controller -- login --> change when moved to Ride Along
            fetch("http://localhost:8080/Auth/startLogin", {
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
                    // Trying to attach original username input into text box
                    usernameInput.value = username;
                }
                else {
                    // If response is not OK, display to user process failed
                    alert("Authentication failed!");
                    console.error("Error:", response.statusText);
                }
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
}
;
function submitOTP() {
    var otpInput = document.getElementById("otp-input");
    var otp = otpInput.value.trim();
    // Retrieve the username from the paragraph element
    var usernameParagraph = document.querySelector("#username-paragraph");
    var username = usernameParagraph.textContent.replace('Username: ', '').trim();
    // Check if OTP is not empty
    if (otp && username) {
        if (isValidOTP(otp) && isValidEmailAddress(username)) {
            var fetchResponse = false; // Assuming Fail response by default
            // Make fetch request to back - end
            fetch("http://localhost:8080/Auth/tryAuthentication", {
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
}
;
// Views
function showOTPView() {
    // Retrieving the page's dynamic html div
    var dynamicContent = document.querySelector(".dynamic-content");
    // Getting rid of the first submit button, for increased clarity
    var submitUsernameButton = document.getElementById("submit-username");
    submitUsernameButton.remove();
    // Create a paragraph element to display the username
    var usernameParagraph = document.createElement('p');
    var usernameInputField = document.getElementById("username-input");
    usernameParagraph.textContent = 'Username: ' + usernameInputField.value.trim();
    usernameParagraph.id = 'username-paragraph';
    // Remove the username input field
    var usernameInput = document.getElementById("username-input");
    usernameInput.remove();
    // Creating the new HTML for the OTP view
    var otpContainer = document.createElement('div');
    otpContainer.innerHTML += "\n        <div id=\"otp-container\">\n            <p>An OTP has been sent to your email address. Please enter the OTP:</p>\n            <input type=\"text\" id=\"otp-input\" placeholder=\"Enter OTP\">\n            <button id=\"submit-otp\">Submit</button>\n        </div>\n    ";
    otpContainer.id = 'otp-container';
    // Append the OTP view and the username paragraph to the dynamic content
    dynamicContent.innerHTML = '';
    dynamicContent.appendChild(usernameParagraph);
    dynamicContent.appendChild(otpContainer);
    // Attach event listener to submit OTP button
    var submitOTPButton = document.getElementById("submit-otp");
    submitOTPButton.addEventListener("click", submitOTP);
}
;
function showMainContent() {
    var dynamicContent = document.querySelector(".dynamic-content");
    dynamicContent.innerHTML = "\n        <div id=\"main-content\">\n            <p>Welcome, user! You are now logged in.</p>\n            <p>Welcome to the <b>Ride-Along</b> Application!<br>We are currently working on this page to make it better suited for you!</p>\n            <p>Work in progresss: Security, Vehicle Profile, and Service Log</p>\n        </div>\n    ";
}
;
function unhideNavigation() {
    // Updating nav
    var navigation = document.getElementById("navigation");
    navigation.classList.remove("hidden");
}
;