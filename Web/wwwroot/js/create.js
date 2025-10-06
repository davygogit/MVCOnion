

document.addEventListener('DOMContentLoaded', function() {
    const loginForm = document.getElementById('loginForm');
    const errorMessage = document.getElementById('errorMessage');
    const authContainer = document.querySelector('.auth-container');
    const createContainer = document.getElementById('createContainer');
    const logoutBtn = document.getElementById('logoutBtn');
    
    // Vérifier si l'utilisateur est déjà connecté
    if (sessionStorage.getItem('isAuthenticated') === 'true') {
        showCreateContainer();
    }
    
    loginForm.addEventListener('submit', function(e) {
        e.preventDefault();
        
        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;
        
        // Vérifier les identifiants (admin/admin)
        if (username === 'admin' && password === 'admin') {
            sessionStorage.setItem('isAuthenticated', 'true');
            showCreateContainer();
            errorMessage.style.display = 'none';
        } else {
            errorMessage.style.display = 'block';
            document.getElementById('password').value = '';
        }
    });
    
    logoutBtn.addEventListener('click', function() {
        sessionStorage.removeItem('isAuthenticated');
        showAuthForm();
    });
    
    function showCreateContainer() {
        authContainer.style.display = 'none';
        createContainer.style.display = 'flex';
    }
    
    function showAuthForm() {
        authContainer.style.display = 'flex';
        createContainer.style.display = 'none';
        loginForm.reset();
        errorMessage.style.display = 'none';
    }
});

