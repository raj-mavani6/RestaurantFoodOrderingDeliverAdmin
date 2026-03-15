// ================================================
// Restaurant Food Ordering - Delivery Admin Panel
// JavaScript Functions
// ================================================

// ================================================
// 1. Utility Functions
// ================================================

// Show toast notification
function showToast(message, type = 'info') {
    const toastHtml = `
        <div class="toast-notification toast-${type}">
            <i class="fas fa-${getIconByType(type)} me-2"></i>
            ${message}
        </div>
    `;
    
    const toastContainer = document.getElementById('toastContainer') || createToastContainer();
    toastContainer.insertAdjacentHTML('beforeend', toastHtml);
    
    const toast = toastContainer.lastElementChild;
    setTimeout(() => {
        toast.classList.add('show');
    }, 10);
    
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

function getIconByType(type) {
    const icons = {
        'success': 'check-circle',
        'error': 'exclamation-circle',
        'warning': 'exclamation-triangle',
        'info': 'info-circle'
    };
    return icons[type] || 'info-circle';
}

function createToastContainer() {
    const container = document.createElement('div');
    container.id = 'toastContainer';
    container.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9999;
    `;
    document.body.appendChild(container);
    return container;
}

// ================================================
// 2. Leave Management Functions
// ================================================

function approveLeave(leaveId) {
    if (confirm('Are you sure you want to approve this leave request?')) {
        // TODO: Send approval request to server
        showToast('Leave approved successfully!', 'success');
        // Refresh page or update UI
        location.reload();
    }
}

function rejectLeave(leaveId) {
    const adminNotes = prompt('Please enter rejection reason:');
    if (adminNotes) {
        // TODO: Send rejection request to server
        showToast('Leave rejected successfully!', 'success');
        // Refresh page or update UI
        location.reload();
    }
}

// ================================================
// 3. User Management Functions
// ================================================

function deactivateUser(userId) {
    if (confirm('Are you sure you want to deactivate this user?')) {
        // TODO: Send deactivation request to server
        showToast('User deactivated successfully!', 'success');
        location.reload();
    }
}

function activateUser(userId) {
    if (confirm('Are you sure you want to activate this user?')) {
        // TODO: Send activation request to server
        showToast('User activated successfully!', 'success');
        location.reload();
    }
}

// ================================================
// 4. Table Functions
// ================================================

function sortTable(columnIndex) {
    const table = document.querySelector('.table');
    const tbody = table.querySelector('tbody');
    const rows = Array.from(tbody.querySelectorAll('tr'));
    
    rows.sort((a, b) => {
        const aValue = a.cells[columnIndex].textContent.trim();
        const bValue = b.cells[columnIndex].textContent.trim();
        
        const aNum = parseFloat(aValue);
        const bNum = parseFloat(bValue);
        
        if (!isNaN(aNum) && !isNaN(bNum)) {
            return aNum - bNum;
        }
        
        return aValue.localeCompare(bValue);
    });
    
    rows.forEach(row => tbody.appendChild(row));
}

function searchTable(searchTerm) {
    const table = document.querySelector('.table');
    const rows = table.querySelectorAll('tbody tr');
    let visibleCount = 0;
    
    rows.forEach(row => {
        const text = row.textContent.toLowerCase();
        
        if (text.includes(searchTerm.toLowerCase())) {
            row.style.display = '';
            visibleCount++;
        } else {
            row.style.display = 'none';
        }
    });
    
    if (visibleCount === 0) {
        showToast('No results found', 'info');
    }
}

// ================================================
// 5. Navigation Functions
// ================================================

function logout() {
    if (confirm('Are you sure you want to logout?')) {
        window.location.href = '/Auth/Logout';
    }
}

function goBack() {
    window.history.back();
}

// ================================================
// 6. Event Listeners Setup
// ================================================

document.addEventListener('DOMContentLoaded', function() {
    // Setup any initialization code here
    console.log('Admin Panel Loaded');
});

// ================================================
// 7. API Functions
// ================================================

async function fetchData(url) {
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return await response.json();
    } catch (error) {
        console.error('Error fetching data:', error);
        showToast('Error loading data. Please try again.', 'error');
        return null;
    }
}

async function postData(url, data) {
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data)
        });
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        return await response.json();
    } catch (error) {
        console.error('Error posting data:', error);
        showToast('Error submitting data. Please try again.', 'error');
        return null;
    }
}

// ================================================
// 8. Storage Functions
// ================================================

function saveToLocalStorage(key, value) {
    try {
        localStorage.setItem(key, JSON.stringify(value));
    } catch (error) {
        console.error('Error saving to localStorage:', error);
    }
}

function getFromLocalStorage(key) {
    try {
        const item = localStorage.getItem(key);
        return item ? JSON.parse(item) : null;
    } catch (error) {
        console.error('Error reading from localStorage:', error);
        return null;
    }
}

function removeFromLocalStorage(key) {
    try {
        localStorage.removeItem(key);
    } catch (error) {
        console.error('Error removing from localStorage:', error);
    }
}
