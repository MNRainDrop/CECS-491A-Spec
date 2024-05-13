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
    // Generate view to prompt the user to enter their email
    generateEmailInputView();
}

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
    webURL = CONFIG["ip"] + ':' + CONFIG["ports"]["registration"]
    window.webURL = webURL;
}) (window);

function generateEmailInputView() {
    // Get the dynamic content area
    var dynamicContent = document.querySelector(".dynamic-content");
    dynamicContent.innerHTML = "";
    // Create a container for the email input form
    var emailInputContainer = document.createElement("div");
    emailInputContainer.classList.add("email-input-container");

    // Create and append a heading for the form
    var heading = document.createElement("h2");
    heading.textContent = "Enter your email";
    emailInputContainer.appendChild(heading);

    // Create the email input field
    var emailInput = document.createElement("input");
    emailInput.type = "email";
    emailInput.placeholder = "Email";
    emailInput.id = "email-input"; // Add an id for easier access
    emailInput.classList.add("email-input");
    emailInputContainer.appendChild(emailInput);

    // Create a submit button
    var submitButton = document.createElement("button");
    submitButton.textContent = "Submit";
    submitButton.classList.add("email-input-submit");
    submitButton.addEventListener("click", submitEmail);
    emailInputContainer.appendChild(submitButton);

    // Append the email input form to the dynamic content area
    dynamicContent.appendChild(emailInputContainer);
}

function submitEmail() {
    // Retrieve the value of the email input field
    var email = document.getElementById("email-input").value;

    // Validate the email input
    if (isValidEmail(email)) {
        // Proceed with the registration process


        // Send the registration request to the server
        fetchWithTokens(webURL + '/Registration/PostVerify', 'POST', email)
        .then(response => {
            if (response.ok) {
                // Registration successful
                return response.text(); // Parse the response text
            } else {
                // Registration failed, extract error message from response body
                return response.text().then(errorMessage => {
                    throw new Error(errorMessage);
                });
            }
        })
        .then(data => {
            // Show success message from the server response
            alert(data);
    
            // Call function to generate account information view
            generateAccountInfoView(email);
        })
        .catch(error => {
            // Show the error message in an alert
            alert(error.message || "An error occurred. Please try again later.");
        });
    
    } else {
        // If the email input is invalid, show an error message
        alert('Invalid email!');
    }
}

function generateAccountInfoView(email) {
    // Get the dynamic content area
    var dynamicContent = document.querySelector(".dynamic-content");
    dynamicContent.innerHTML = "";
    
    // Create a container for the account information form
    var accountInfoContainer = document.createElement("div");
    accountInfoContainer.classList.add("account-info-container");

    // Create and append a heading for the form
    var heading = document.createElement("h2");
    heading.textContent = "Account Information";
    accountInfoContainer.appendChild(heading);

    // Create the non-editable email input field
    var emailInput = document.createElement("input");
    emailInput.type = "text";
    emailInput.placeholder = "Email";
    emailInput.classList.add("account-info-input");
    emailInput.value = email;
    emailInput.disabled = true; // Make it non-editable
    accountInfoContainer.appendChild(emailInput);

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
    submitButton.addEventListener("click", function() {
        submitAccountInfo(email);
    });
    accountInfoContainer.appendChild(submitButton);

    // Append the account information form to the dynamic content area
    dynamicContent.appendChild(accountInfoContainer);
}

// Function to handle submission of account information
function submitAccountInfo() {

    var dynamicContent = document.querySelector(".dynamic-content");
    // Find the account info container within dynamic content
    var accountInfoContainer = dynamicContent.querySelector(".account-info-container");

    // Retrieve the values from the input fields within account info container
    var email = accountInfoContainer.querySelector("input[type='text']").value;
    var otp = accountInfoContainer.querySelector("input[type='text'][placeholder='OTP sent to your email']").value;
    var dob = accountInfoContainer.querySelector("input[type='date']").value;
    var alternateEmail = accountInfoContainer.querySelector("input[type='email']").value;
    var accountType = accountInfoContainer.querySelector("select").value;
    
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

var accountData = {
    dateOfBirth: dob,
    altEmail: alternateEmail,
    email: email,
    otp: otp,
    accountType: accountType
}

// Send the registration request to the server
fetchWithTokens( webURL + 'Registration/PostCreateUser', 'POST', accountData)
.then(response => {
    if (response.ok) {
        // Registration successful
        alert("Account created successfully! You can now log in.");
        // Redirect the user to the main page
        location.reload();
    } else {
        // Registration failed, get the error message from the response
        return response.json().then(data => { throw new Error(data.message); });
    }
})
.catch(error => {
    console.error('Error:', response.text());
    // Show an alert for any errors
    alert(response.text());
});
}