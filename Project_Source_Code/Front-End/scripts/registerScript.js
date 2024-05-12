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


var createAccountButton = document.getElementById("registration-button");
createAccountButton.addEventListener("click", createAccount);

function createAccount() {
    // to signal we are in function
    alert("create account");

    // Retrieve the value of the email or username input field
    var userInput = document.getElementById("text-input").value;

    // Validate the email or username input
    if (isValidEmail(userInput)) {
        var registrationData = {
            email: userInput
        };
        
        // Send the registration request to the server
        fetchWithTokens('http://localhost:5107/Registration/PostVerify', 'POST', registrationData)
        .then(response => {
            if (response.ok) {
                // Registration successful
                return response.json(); // Parse the response JSON
            } else {
                // Registration failed, throw an error with the response message
                return response.json().then(data => { throw new Error(data.message); });
            }
        })
        .then(data => {
            // Show success message from the server response
            alert(data.message);

            // Call function to generate account information view
            generateAccountInfoView();
        })
        .catch(error => {
            console.error('Error:', error);
            // Show an alert for any errors
            alert(error.message || 'Error occurred while creating account!');
        });
    } else {
        // If the email or username input is invalid, show an error message
        alert('Invalid email!');
    }

}

function generateAccountInfoView() {
    // Here you can write code to dynamically generate the view for inputting account information
    // For simplicity, let's just show an alert for now
    alert("Generating account information view...");
}