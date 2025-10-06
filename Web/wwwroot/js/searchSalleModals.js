// Gestion des modals dans SearchSalle avec canvas
document.addEventListener('DOMContentLoaded', function () {
    // Écouter les changements de taille d'écran pour ajuster le comportement
    window.addEventListener('resize', function () {
        // Si un canvas est actuellement affiché, le redessiner avec les bonnes coordonnées
        const activeCanvas = document.querySelector('canvas[style*="display: block"]');
        if (activeCanvas && activeCanvas.img) {
            redrawCanvas(activeCanvas);
        }
    });
});

// Fonction pour fermer tous les modals
function closeAllModals() {
    const modalContainer = document.querySelector('#modalContainer');
    if (modalContainer) {
        modalContainer.innerHTML = '';
    }
    document.body.classList.remove('bodyBackDesable');
    // Restaurer la barre de défilement
    document.body.style.overflow = "auto";
}

// Fonction pour détecter si l'écran est en mode mobile
function isMobileScreen() {
    return window.innerWidth <= 850;
}

// Fonction pour ajuster les coordonnées de déplacement selon l'orientation
function adjustDragCoordinates(deltaX, deltaY) {
    if (isMobileScreen()) {
        // En mode mobile, le canvas est roté de -90deg, donc on inverse X et Y
        // et on inverse la direction du déplacement
        return {
            deltaX: -deltaY,
            deltaY: deltaX
        };
    }
    return { deltaX, deltaY };
}

// Input search salle
document.getElementById('searchSalleForm').addEventListener('submit', function (e) {
    e.preventDefault();
    performSearch();
});

// Recherche en temps réel au changement
document.getElementById('salleInput').addEventListener('input', function (e) {
    performSearch();
});

// Fonction de recherche
function performSearch() {
    var inputValue = document.getElementById('salleInput').value.trim();
    
    if (!inputValue) {
        // Si le champ est vide, réinitialiser l'affichage
        if (typeof resetAllFilters === 'function') {
            resetAllFilters();
        }
        return;
    }
    
    // Rechercher la salle correspondante
    const sallesClick = document.querySelectorAll('.salles_click');
    let salleTrouvee = false;
    
    // D'abord, chercher une correspondance exacte
    sallesClick.forEach(salle => {
        const salleNum = salle.dataset.sallenum;
        const salleNom = salle.dataset.sallenom;
        // Comparaison insensible à la casse pour les numéros et noms
        if ((salleNum && inputValue.toLowerCase() === salleNum.toLowerCase()) || 
            (salleNom && inputValue.toLowerCase() === salleNom.toLowerCase())) {
            // Utiliser la fonction de création de modal dynamique
            if (typeof showSalleModal === 'function') {
                showSalleModal(salle);
            }
            salleTrouvee = true;
        }
    });
    
    // Si aucune salle exacte trouvée, essayer de filtrer
    if (!salleTrouvee) {
        let sallesFiltrees = [];
        
        // Chercher des correspondances partielles dans les noms et numéros
        sallesClick.forEach(salle => {
            const salleNom = salle.dataset.sallenom;
            const salleNum = salle.dataset.sallenum;
            
            // Recherche dans le nom
            if (salleNom && salleNom.toLowerCase().includes(inputValue.toLowerCase())) {
                sallesFiltrees.push(salle);
            }
            // Recherche dans le numéro
            else if (salleNum && salleNum.toLowerCase().includes(inputValue.toLowerCase())) {
                sallesFiltrees.push(salle);
            }
        });
        
        // Si on trouve des correspondances, les afficher
        if (sallesFiltrees.length > 0) {
            sallesClick.forEach(salle => {
                salle.style.display = "none";
            });
            sallesFiltrees.forEach(salle => {
                salle.style.display = "block";
            });
        } else {
            // Sinon, essayer de filtrer par étage si c'est un numéro
            const premierChiffre = inputValue.charAt(0);
            if (!isNaN(premierChiffre)) {
                sallesClick.forEach(salle => {
                    const salleNum = salle.dataset.sallenum;
                    if (salleNum && salleNum.charAt(0) === premierChiffre) {
                        salle.style.display = "block";
                    } else {
                        salle.style.display = "none";
                    }
                });
            }
        }
    }
}

// Fonction pour initialiser les modals (maintenant gérée par searchSalle.js)
function initializeSearchSalleModals() {
    // Cette fonction n'est plus nécessaire car les modals sont créés dynamiquement
    // Les événements sont gérés dans searchSalle.js
}

// Fonction pour charger le canvas d'un modal
function loadModalCanvas(salleId) {
    const canvas = document.getElementById(`planCanvas_${salleId}`);
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    const img = new Image();
    
    // Récupérer les coordonnées depuis les attributs data
    const coordX = parseFloat(canvas.dataset.coordx);
    const coordY = parseFloat(canvas.dataset.coordy);

    img.onload = function() {
        // Ajuster la taille du canvas à l'image
        canvas.width = img.width;
        canvas.height = img.height;
        
        // Stocker l'image et les coordonnées sur le canvas pour les utiliser dans redrawCanvas
        canvas.img = img;
        canvas.pointX = coordX;
        canvas.pointY = coordY;
        
        // Initialiser les propriétés d'animation
        canvas.currentPointRadius = 20;
        canvas.currentColorIntensity = 0;
        
        // Dessiner l'image
        ctx.drawImage(img, 0, 0);
        
        // Dessiner et animer le point si les coordonnées sont fournies et valides
        if (coordX !== null && coordY !== null && !isNaN(coordX) && !isNaN(coordY)) {
            animatePoint(canvas.id, img, coordX, coordY);
        }
        
        // Ajouter les event listeners pour le zoom et le déplacement
        setupCanvasInteractions(canvas, img, coordX, coordY);
    };
    
    img.onerror = function () {
        console.error("Erreur de chargement de l'image pour le modal : ", canvas.dataset.imgpath);
        // Afficher un message d'erreur sur le canvas
        ctx.fillStyle = "#f8f9fa";
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        ctx.fillStyle = "#666";
        ctx.font = "16px Arial";
        ctx.textAlign = "center";
        ctx.fillText("Erreur de chargement de l'image", canvas.width / 2, canvas.height / 2);
    };
    
    img.src = canvas.dataset.imgpath;
}

// Variables globales pour le zoom
let currentScale = 1;
let currentOffsetX = 0;
let currentOffsetY = 0;
let isDragging = false;
let lastMouseX = 0;
let lastMouseY = 0;

// Fonction pour configurer les interactions du canvas (zoom et déplacement)
function setupCanvasInteractions(canvas, img, pointX, pointY) {
    // Supprimer les anciens event listeners s'ils existent
    canvas.removeEventListener('wheel', handleWheel);
    canvas.removeEventListener('mousedown', handleMouseDown);
    canvas.removeEventListener('mousemove', handleMouseMove);
    canvas.removeEventListener('mouseup', handleMouseUp);
    canvas.removeEventListener('mouseleave', handleMouseUp);
    canvas.removeEventListener('dblclick', handleDoubleClick);
    canvas.removeEventListener('click', handleClick);
    
    // Ajouter les nouveaux event listeners
    canvas.addEventListener('wheel', handleWheel);
    canvas.addEventListener('mousedown', handleMouseDown);
    canvas.addEventListener('mousemove', handleMouseMove);
    canvas.addEventListener('mouseup', handleMouseUp);
    canvas.addEventListener('mouseleave', handleMouseUp);
    canvas.addEventListener('dblclick', handleDoubleClick);
    canvas.addEventListener('click', handleClick);
    
    // Stocker les références pour les utiliser dans les handlers
    canvas.img = img;
    canvas.pointX = pointX;
    canvas.pointY = pointY;
    
    function handleClick(e) {
        e.stopPropagation();
    }
    
    function handleWheel(e) {
        e.preventDefault();
        e.stopPropagation();
        const delta = e.deltaY > 0 ? 0.9 : 1.1; // Réduire ou augmenter le zoom
        const newScale = Math.max(0.5, Math.min(5, currentScale * delta)); // Limiter le zoom entre 0.5x et 5x
        
        // Calculer le point de zoom (position de la souris)
        const rect = canvas.getBoundingClientRect();
        const mouseX = e.clientX - rect.left;
        const mouseY = e.clientY - rect.top;
        
        // Ajuster l'offset pour zoomer vers le point de la souris
        const scaleChange = newScale / currentScale;
        currentOffsetX = mouseX - (mouseX - currentOffsetX) * scaleChange;
        currentOffsetY = mouseY - (mouseY - currentOffsetY) * scaleChange;
        currentScale = newScale;
        
        redrawCanvas(canvas);
    }
    
    function handleMouseDown(e) {
        e.stopPropagation();
        isDragging = true;
        lastMouseX = e.clientX;
        lastMouseY = e.clientY;
        canvas.style.cursor = 'grabbing';
    }
    
    function handleMouseMove(e) {
        if (!isDragging) return;
        e.stopPropagation();
        
        const deltaX = e.clientX - lastMouseX;
        const deltaY = e.clientY - lastMouseY;
        const adjustedCoords = adjustDragCoordinates(deltaX, deltaY);
        
        currentOffsetX += adjustedCoords.deltaX;
        currentOffsetY += adjustedCoords.deltaY;
        
        lastMouseX = e.clientX;
        lastMouseY = e.clientY;
        
        redrawCanvas(canvas);
    }
    
    function handleMouseUp(e) {
        if (e) e.stopPropagation();
        isDragging = false;
        canvas.style.cursor = 'grab';
    }
    
    function handleDoubleClick(e) {
        e.preventDefault();
        e.stopPropagation();
        // Reset zoom et position
        currentScale = 1;
        currentOffsetX = 0;
        currentOffsetY = 0;
        redrawCanvas(canvas);
    }
}

function redrawCanvas(canvas) {
    const ctx = canvas.getContext('2d');
    const img = canvas.img;
    const pointX = canvas.pointX;
    const pointY = canvas.pointY;
    
    if (!img) return;
    
    // Effacer le canvas
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    
    // Sauvegarder le contexte
    ctx.save();
    
    // Appliquer les transformations
    ctx.translate(currentOffsetX, currentOffsetY);
    ctx.scale(currentScale, currentScale);
    
    // Dessiner l'image
    ctx.drawImage(img, 0, 0);
    
    // Dessiner le marqueur si les coordonnées sont valides
    if (pointX !== null && pointY !== null && !isNaN(pointX) && !isNaN(pointY)) {
        // Utiliser la taille actuelle du marqueur pour l'animation
        const markerSize = canvas.currentPointRadius || 20;
        drawMarker(ctx, pointX, pointY, markerSize);
    }
    
    // Restaurer le contexte
    ctx.restore();
}

function drawPointOnCanvas(canvasId, x, y, radius, color = "#FF0000") {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    
    const ctx = canvas.getContext('2d');
    drawPointOnCanvasWithContext(ctx, x, y, radius, color);
}

function drawPointOnCanvasWithContext(ctx, x, y, radius, color = "#FF0000") {
    ctx.save();
    
    // Taille du marqueur
    const markerSize = radius * 2;
    const halfSize = markerSize / 2;
    
    // Position du marqueur
    const markerX = x - halfSize;
    const markerY = y - markerSize; // Décaler vers le haut pour que la pointe soit sur la position
    
    // Effet de lueur d'abord (en arrière-plan)
    ctx.shadowColor = color;
    ctx.shadowBlur = 15;
    ctx.shadowOffsetX = 2;
    ctx.shadowOffsetY = 2;
    
    // Dessiner le marqueur SVG (forme de goutte/pin) - version ombrée
    ctx.beginPath();
    
    // Corps du marqueur (cercle en haut)
    ctx.arc(x, y - halfSize, halfSize, 0, 2 * Math.PI);
    
    // Pointe du marqueur (triangle en bas)
    ctx.moveTo(x - halfSize, y - halfSize);
    ctx.lineTo(x, y + halfSize);
    ctx.lineTo(x + halfSize, y - halfSize);
    ctx.closePath();
    
    // Remplir le marqueur avec l'ombre
    ctx.fillStyle = color;
    ctx.fill();
    
    // Maintenant dessiner le marqueur principal sans ombre
    ctx.shadowBlur = 0;
    ctx.shadowOffsetX = 0;
    ctx.shadowOffsetY = 0;
    
    // Dessiner le marqueur principal
    ctx.beginPath();
    
    // Corps du marqueur (cercle en haut)
    ctx.arc(x, y - halfSize, halfSize, 0, 2 * Math.PI);
    
    // Pointe du marqueur (triangle en bas)
    ctx.moveTo(x - halfSize, y - halfSize);
    ctx.lineTo(x, y + halfSize);
    ctx.lineTo(x + halfSize, y - halfSize);
    ctx.closePath();
    
    // Remplir complètement le marqueur
    ctx.fillStyle = color;
    ctx.fill();
    
    // Contour blanc épais
    ctx.strokeStyle = "#FFFFFF";
    ctx.lineWidth = 4;
    ctx.stroke();
    
    // Point central blanc pour plus de visibilité
    ctx.fillStyle = "#FFFFFF";
    ctx.beginPath();
    ctx.arc(x, y - halfSize, halfSize * 0.4, 0, 2 * Math.PI);
    ctx.fill();
    
    ctx.restore();
}

function animatePoint(canvasId, img, x, y) {
    let radius = 12; // Taille de base
    let growing = true;
    
    function updateRadius() {
        // Animation de taille simple
        if (growing) {
            radius += 0.3; // Animation lente
            if (radius >= 35) growing = false; // Taille max raisonnable
        } else {
            radius -= 0.3;
            if (radius <= 20) growing = true; // Taille min
        }
        
        const canvas = document.getElementById(canvasId);
        if (canvas && canvas.img) {
            // Stocker le rayon actuel pour l'utiliser dans redrawCanvas
            canvas.currentPointRadius = radius;
            redrawCanvas(canvas);
        }
        
        requestAnimationFrame(updateRadius);
    }
    
    updateRadius();
}

function drawMarker(ctx, x, y, size) {
    ctx.save();
    
    // Taille du marqueur en forme de goutte
    const markerWidth = size * 1.8;
    const markerHeight = size * 2.8;
    
    // Position du marqueur (la pointe de la goutte doit être sur la position exacte)
    const markerX = x;
    const markerY = y - markerHeight * 0.85; // Décaler vers le haut pour que la pointe soit sur la position
    
    // Fonction pour dessiner la forme de goutte
    function drawDropShape(ctx, x, y, width, height, isShadow = false) {
        const offset = isShadow ? 3 : 0;
        
        ctx.beginPath();
        
        // Corps principal (cercle en haut)
        const circleRadius = width / 2;
        const circleY = y + circleRadius;
        ctx.arc(x + offset, circleY + offset, circleRadius, 0, Math.PI * 2);
        
        // Pointe de la goutte (triangle en bas)
        ctx.moveTo(x - width/2 + offset, circleY + offset);
        ctx.lineTo(x + offset, y + height + offset);
        ctx.lineTo(x + width/2 + offset, circleY + offset);
        ctx.closePath();
    }
    
    // Dessiner l'ombre
    ctx.shadowColor = '#000000';
    ctx.shadowBlur = 10;
    ctx.shadowOffsetX = 3;
    ctx.shadowOffsetY = 3;
    
    // Ombre de la goutte
    ctx.fillStyle = '#000000';
    drawDropShape(ctx, markerX, markerY, markerWidth, markerHeight, true);
    ctx.fill();
    
    ctx.restore();
    ctx.save();
    
    // Marqueur principal - goutte
    ctx.shadowBlur = 0;
    ctx.shadowOffsetX = 0;
    ctx.shadowOffsetY = 0;
    
    // Goutte rouge
    ctx.fillStyle = '#FF0000';
    drawDropShape(ctx, markerX, markerY, markerWidth, markerHeight);
    ctx.fill();
    
    // Bordure blanche épaisse
    ctx.strokeStyle = '#FFFFFF';
    ctx.lineWidth = 4;
    ctx.stroke();
    
    // Bordure noire fine
    ctx.strokeStyle = '#000000';
    ctx.lineWidth = 1;
    ctx.stroke();
    
    ctx.restore();
}

