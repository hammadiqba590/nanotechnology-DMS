document.getElementById("first-time-login-form").onsubmit = async function (event) {
    event.preventDefault();

    const newPassword = document.getElementById("newPassword").value;
    const confirmPassword = document.getElementById("confirmPassword").value;
    const userId = document.getElementsByName("UserId")[0].value;
    const token = document.getElementsByName("Token")[0].value;
    const responseMessage = document.getElementById("responseMessage");

    responseMessage.className = "error-message";
    responseMessage.textContent = "";

    // ?? Client-side validation
    if (newPassword !== confirmPassword) {
        responseMessage.textContent = "Passwords do not match!";
        return;
    }

    if (!validatePassword(newPassword)) {
        responseMessage.textContent =
            "Password must be 12-128 characters and include uppercase, lowercase, numeric, and special characters.";
        return;
    }

    try {
        const apiUrl = `${window.location.origin}/apigateway/authservice/user/first-time-login`;

        const response = await fetch(apiUrl, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                UserId: userId,
                Token: token,
                NewPassword: newPassword,
                ConfirmPassword: confirmPassword,
            }),
        });

        // ? Safely parse response
        let data = {};
        const contentType = response.headers.get("content-type");

        if (contentType && contentType.includes("application/json")) {
            data = await response.json();
        } else {
            data.message = await response.text();
        }

        // ? SUCCESS HANDLING
        if (response.ok && (data.message?.toLowerCase() === "success" || data.success === true)) {
            responseMessage.className = "success-message";
            responseMessage.textContent = "Password set successfully!";

            setTimeout(() => {
                window.location.href = "http://195.7.7.34:7900/";
            }, 2000);
            return;
        }

        // ? API-level error
        responseMessage.textContent = data.message || "Password setup failed.";

    } catch (error) {
        responseMessage.textContent = "An unexpected error occurred. Please try again.";
        console.error(error);
    }
};

function validatePassword(password) {
    const passwordRegex =
        /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#^])[A-Za-z\d@$!%*?&#^]{12,128}$/;
    return passwordRegex.test(password);
}

function togglePassword(fieldId) {
    const passwordField = document.getElementById(fieldId);
    const icon = document.getElementById("toggle-" + fieldId + "-icon");

    if (passwordField.type === "password") {
        passwordField.type = "text";
        icon.classList.replace("fa-eye", "fa-eye-slash");
    } else {
        passwordField.type = "password";
        icon.classList.replace("fa-eye-slash", "fa-eye");
    }
}
