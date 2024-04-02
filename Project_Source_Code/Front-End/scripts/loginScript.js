// To note: unhiding certain navigation attribuites based on user claims --> admin has System obserbility
// Attach event listener to submit username button
var submitUsernameButton = document.getElementById("submit-username");
submitUsernameButton.addEventListener("click", submitUsername);
// Checks user username
var isValidEmailAddress = function (email) {
    var emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailPattern.test(email);
};
// Checks user OTP
// Note forces OTP to have at least one lowercase, uppercase, number with at least characters in total
var isValidOTP = function (OTP) {
    var otpPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/;
    return otpPattern.test(OTP);
};
// Function to handle submitting username
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
// Function to show OTP view
function showOTPView() {
    var dynamicContent = document.querySelector(".dynamic-content");
    dynamicContent.innerHTML += "\n        <div id=\"otp-container\">\n            <p>An OTP has been sent to your email address. Please enter the OTP:</p>\n            <input type=\"text\" id=\"otp-input\" placeholder=\"Enter OTP\">\n            <button id=\"submit-otp\">Submit</button>\n        </div>\n    ";
    // Attach event listener to submit OTP button
    var submitOTPButton = document.getElementById("submit-otp");
    submitOTPButton.addEventListener("click", submitOTP);
}
// Function to handle submitting OTP
function submitOTP() {
    var otpInput = document.getElementById("otp-input");
    var otp = otpInput.value.trim();
    var usernameInput = document.getElementById("username-input");
    var username = usernameInput.value.trim();
    // Check if OTP is not empty
    if (otp && username) {
        if (isValidOTP(otp) && isValidEmailAddress(username)) {
            var fetchResponse = false; // Assuming Fail response by default
            // Make fetch request to back - end
            alert("Trying Fetch");
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
                alert("Got to the data Stage");
                fetchResponse = true;
                sessionStorage.setItem('IDToken', data.idToken);
                sessionStorage.setItem('AccessToken', data.accessToken);
                sessionStorage.setItem('RefreshToken', data.refreshToken);
                alert("Set tokens");
            })
                .catch(function (error) {
                alert("Something went wrong!" + " " + error);
                console.error("Error:", error);
            });
            // if OK --> generate ShowMainContent(), UnhideNavigation
            if (fetchResponse) {
                // If OTP is valid, show main content and unhide navigation
                showMainContent();
                unhideNavigation();
            }
            else {
                alert("Invalid OTP!");
            }
        }
        else {
            alert("You have entered a bad OTP");
        }
    }
    else {
        alert("OTP cannot be empty!");
    }
}
// Function to show main content --> refers to Welcome page.
// Where we want access to all features such as VP, SL, CHR....
function showMainContent() {
    var dynamicContent = document.querySelector(".dynamic-content");
    dynamicContent.innerHTML = "\n        <div id=\"main-content\">\n            <p>Welcome, user! You are now logged in.</p>\n            <p>Welcome to the <b>Ride-Along</b> Application!<br>We are currently working on this page to make it better suited for you!</p>\n            <p>Work in progresss: Security, Vehicle Profile, and Service Log</p>\n        </div>\n    ";
}
// Function to unhide navigation
function unhideNavigation() {
    var navigation = document.getElementById("navigation");
    navigation.classList.remove("hidden");
    // Add Event listeners to nagvigation items
    var navigationItems = navigation.querySelectorAll('li');
    navigationItems.forEach(function (item) {
        item.addEventListener('click', handleNavigationItemClick);
    });
}
function handleNavigationItemClick(event) {
    // Handle navigation item click
    var selectedItem = event.target.id;
    // Perform actions based on the selected item, such as navigating to different pages or triggering specific functionality
    console.log(selectedItem + ' clicked');
}
