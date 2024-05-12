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
        fetchWithTokens('http://localhost:8003/Registration/PostVerify', 'POST', registrationData)
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
    // Get the dynamic content area
    var dynamicContent = document.querySelector(".dynamic-content");

    // Create a container for the account information form
    var accountInfoContainer = document.createElement("div");
    accountInfoContainer.classList.add("account-info-container");

    // Create and append a heading for the form
    var heading = document.createElement("h2");
    heading.textContent = "Account Information";
    accountInfoContainer.appendChild(heading);

    // Create the OTP input field
    var otpInput = document.createElement("input");
    otpInput.type = "text";
    otpInput.placeholder = "OTP sent to your email";
    otpInput.classList.add("account-info-input");
    accountInfoContainer.appendChild(otpInput);

    // Create the date of birth input field
    var dobInput = document.createElement("input");
    dobInput.type = "date";
    var currentDate = new Date();
    var maxDate = new Date(currentDate.getFullYear() - 18, 11, 31).toISOString().split('T')[0];
    dobInput.max = maxDate;
    dobInput.placeholder = "Date of Birth (YYYY-MM-DD)";
    dobInput.classList.add("account-info-input");
    accountInfoContainer.appendChild(dobInput);

    // Create the alternate email input field
    var alternateEmailInput = document.createElement("input");
    alternateEmailInput.type = "email";
    alternateEmailInput.placeholder = "Alternate Email";
    alternateEmailInput.classList.add("account-info-input");
    accountInfoContainer.appendChild(alternateEmailInput);

    // Create the account type select field
    var accountTypeSelect = document.createElement("select");
    accountTypeSelect.classList.add("account-info-input");
    // Create options for account types
    var defaultOption = document.createElement("option");
    defaultOption.value = "Default User";
    defaultOption.textContent = "Default User";
    accountTypeSelect.appendChild(defaultOption);
    var vendorOption = document.createElement("option");
    vendorOption.value = "Vendor";
    vendorOption.textContent = "Vendor";
    accountTypeSelect.appendChild(vendorOption);
    var rentalFleetOption = document.createElement("option");
    rentalFleetOption.value = "Rental Fleet";
    rentalFleetOption.textContent = "Rental Fleet";
    accountTypeSelect.appendChild(rentalFleetOption);
    // Set a default selected option
    defaultOption.selected = true;
    accountInfoContainer.appendChild(accountTypeSelect);

    // Create a submit button
    var submitButton = document.createElement("button");
    submitButton.textContent = "Submit";
    submitButton.classList.add("account-info-submit");
    submitButton.addEventListener("click", submitAccountInfo);
    accountInfoContainer.appendChild(submitButton);

    // Append the account information form to the dynamic content area
    dynamicContent.appendChild(accountInfoContainer);
}

// Function to handle submission of account information
function submitAccountInfo() {
    // Here you can retrieve the values from the input fields and perform further processing
    // For now, let's just show an alert with the values
    var otp = document.querySelector(".account-info-container input[type='text']").value;
    var dob = document.querySelector(".account-info-container input[type='date']").value;
    var alternateEmail = document.querySelector(".account-info-container input[type='email']").value;
    var accountType = document.querySelector(".account-info-container select").value;
    
// Validate the OTP
if (!isValidOTP(otp)) {
    alert("Invalid OTP!");
    return;
}

// Validate the alternate email
if (!isValidEmail(alternateEmail)) {
    alert("Invalid alternate email!");
    return;
}

   // Construct the registration data object
   var registrationData = {
    otp: otp,
    dob: dob,
    alternateEmail: alternateEmail,
    accountType: accountType
};

// Send the registration request to the server
fetchWithTokens('http://localhost:8003/Registration/PostCreateUser', 'POST', registrationData)
.then(response => {
    if (response.ok) {
        // Registration successful
        alert("Account created successfully! You can now log in.");
        // Redirect the user to the main page
        window.location.href = "mainPage.html"; // Replace "mainPage.html" with the actual URL of your main page --> config file
    } else {
        // Registration failed, get the error message from the response
        return response.json().then(data => { throw new Error(data.message); });
    }
})
.catch(error => {
    console.error('Error:', error);
    // Show an alert for any errors
    alert(error.message || 'Error occurred while creating account!');
});
}