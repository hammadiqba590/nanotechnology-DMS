document.getElementById("first-time-login-form").onsubmit = function (event) {
  event.preventDefault();
  const newPassword = document.getElementById("newPassword").value;
  const confirmPassword = document.getElementById("confirmPassword").value;
  const userId = document.getElementsByName("UserId")[0].value;
  const token = document.getElementsByName("Token")[0].value;
  const responseMessage = document.getElementById("responseMessage");
  responseMessage.className = "error-message"; // Reset message styling
  // Client-side validation
  if (newPassword !== confirmPassword) {
    responseMessage.textContent = "Passwords do not match!";
    return;
  }
  if (!validatePassword(newPassword)) {
    responseMessage.textContent =
      "Password must be 12-128 characters and include uppercase, lowercase, numeric, and special characters.";
    return;
  }
  // Submit via AJAX
  fetch("/api/User/first-time-login", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      UserId: userId,
      Token: token,
      NewPassword: newPassword,
      ConfirmPassword: confirmPassword,
    }),
  })
    .then((response) => response.json())
    .then((data) => {
      if (data.success) {
        responseMessage.className = "success-message";
        responseMessage.textContent =
          "Password set successfully! Redirecting...";
        setTimeout(() => {
          window.location.href = "/login";
        }, 2000);
      } else {
        responseMessage.textContent = "Error: " + data.Message;
      }
    })
    .catch((error) => {
      responseMessage.textContent = "An error occurred: " + error.message;
    });
};
function validatePassword(password) {
  const passwordRegex =
    /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@@$!%*?&#^])[A-Za-z\d@@$!%*?&#^]{12,128}$/;
  return passwordRegex.test(password);
}
function togglePassword(fieldId) {
  var passwordField = document.getElementById(fieldId);
  var icon = document.getElementById("toggle-" + fieldId + "-icon");
  var currentType = passwordField.getAttribute("type");
  if (currentType === "password") {
    passwordField.setAttribute("type", "text");
    icon.classList.remove("fa-eye");
    icon.classList.add("fa-eye-slash");
  } else {
    passwordField.setAttribute("type", "password");
    icon.classList.remove("fa-eye-slash");
    icon.classList.add("fa-eye");
  }
}
