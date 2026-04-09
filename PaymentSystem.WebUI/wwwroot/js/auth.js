/**
 * Auth JavaScript
 * Handles login, register, and password visibility toggles
 */

/**
 * Toggle password visibility
 * @param {string} inputId - Password input field ID
 */
function togglePassword(inputId) {
    const input = document.getElementById(inputId);
    const toggleBtn = input.parentElement.querySelector('.toggle-password');
    const icon = toggleBtn.querySelector('i');
    
    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('fa-eye');
        icon.classList.add('fa-eye-slash');
    } else {
        input.type = 'password';
        icon.classList.remove('fa-eye-slash');
        icon.classList.add('fa-eye');
    }
}

/**
 * Google login handler
 */
function googleLogin() {
    // Placeholder for Google OAuth
    alert('Google login will be implemented here');
    console.log('Google login initiated');
}

/**
 * Form validation for login
 */
document.addEventListener('DOMContentLoaded', function() {
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', function(e) {
            const email = document.getElementById('Email').value;
            const password = document.getElementById('Password').value;
            
            if (!email || !password) {
                e.preventDefault();
                alert('Please fill in all required fields');
            }
        });
    }
    
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', function(e) {
            const password = document.getElementById('Password').value;
            const confirmPassword = document.getElementById('ConfirmPassword').value;
            
            if (password !== confirmPassword) {
                e.preventDefault();
                alert('Passwords do not match');
            }
            
            if (password.length < 6) {
                e.preventDefault();
                alert('Password must be at least 6 characters');
            }
        });
    }
});
