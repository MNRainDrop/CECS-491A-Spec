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

