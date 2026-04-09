/**
 * User Layout JavaScript
 * Handles navbar dropdowns, mobile menu, and utility functions
 */

/**
 * Toggle mobile navbar menu
 */
function toggleUserMobileMenu() {
    const navbar = document.getElementById('userNavbar');
    if (navbar) {
        navbar.classList.toggle('show');
    }
}

/**
 * Toggle dropdown menu
 * @param {HTMLElement} element - The dropdown toggle element
 */
function toggleUserDropdown(element) {
    event.preventDefault();
    
    const dropdown = element.parentElement;
    const menu = dropdown.querySelector('.dropdown-menu');
    
    if (!menu) return;
    
    // Close all other dropdowns
    document.querySelectorAll('.dropdown-menu.show').forEach(openMenu => {
        if (openMenu !== menu) {
            openMenu.classList.remove('show');
        }
    });
    
    // Toggle current dropdown
    menu.classList.toggle('show');
}

/**
 * Close all dropdowns when clicking outside
 */
document.addEventListener('click', function(event) {
    if (!event.target.closest('.dropdown')) {
        document.querySelectorAll('.dropdown-menu.show').forEach(menu => {
            menu.classList.remove('show');
        });
    }
});

/**
 * Show toast notification
 * @param {string} message - Notification message
 * @param {string} type - Type: 'success', 'error', 'warning', 'info'
 */
function showUserToast(message, type = 'info') {
    const toastContainer = document.getElementById('toast-container');
    if (!toastContainer) return;

    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    
    const icons = {
        success: 'check-circle',
        error: 'exclamation-circle',
        warning: 'exclamation-triangle',
        info: 'info-circle'
    };
    
    const titles = {
        success: 'Success',
        error: 'Error',
        warning: 'Warning',
        info: 'Information'
    };

    toast.innerHTML = `
        <div class="toast-header">
            <i class="fas fa-${icons[type]}"></i>
            <strong>${titles[type]}</strong>
            <button type="button" class="toast-close">&times;</button>
        </div>
        <div class="toast-body">${message}</div>
    `;

    toastContainer.appendChild(toast);

    // Auto remove after 5 seconds
    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transform = 'translateX(100%)';
        setTimeout(() => toast.remove(), 300);
    }, 5000);

    // Close button handler
    toast.querySelector('.toast-close').addEventListener('click', () => {
        toast.remove();
    });
}

/**
 * Format currency
 * @param {number} amount - Amount to format
 * @param {string} currency - Currency code (default: USD)
 * @returns {string}
 */
function formatUserCurrency(amount, currency = 'USD') {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: currency
    }).format(amount);
}

/**
 * Format date
 * @param {string|Date} date - Date to format
 * @returns {string}
 */
function formatUserDate(date) {
    if (!date) return 'N/A';
    const d = new Date(date);
    return d.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
}

/**
 * Get user ID from page data
 * @returns {string|null}
 */
function getCurrentUserId() {
    const userIdElement = document.getElementById('current-user-id');
    if (userIdElement) {
        return userIdElement.dataset.userId;
    }
    return null;
}
