// Variables globales
let canvas, ctx, currentImage;
let clickedCoordinates = null;

// Initialisation quand le DOM est chargé
document.addEventListener('DOMContentLoaded', function() {
    initializeCanvas();
});

function initializeCanvas() {
    canvas = document.getElementById('imageCanvas');
    if (!canvas) return;
    
    ctx = canvas.getContext('2d');
const addEtageToSalle = document.getElementById('addEtageToSalle');
    
    if (addEtageToSalle) {
        // Écouter les changements du select pour charger l'image correspondante
        addEtageToSalle.addEventListener('change', function() {
            const selectedOption = addEtageToSalle.options[addEtageToSalle.selectedIndex];
            const imagePath = selectedOption.dataset.imgpath;
            if (imagePath) {
                loadImageOnCanvas(imagePath);
            }
        });
        
        // Charger l'image par défaut si une option est sélectionnée
        if (addEtageToSalle.selectedIndex >= 0) {
            const selectedOption = addEtageToSalle.options[addEtageToSalle.selectedIndex];
            const imagePath = selectedOption.dataset.imgpath;
            if (imagePath) {
                loadImageOnCanvas(imagePath);
            }
        }
    }
    
    // Écouter les clics sur le canvas
    canvas.addEventListener('click', handleCanvasClick);
}

function loadImageOnCanvas(imagePath) {
    const img = new Image();
    img.crossOrigin = "anonymous";
    
    img.onload = function() {
        currentImage = img;

        // Ajuster la taille du canvas à l'image
        canvas.width = img.width;
        canvas.height = img.height;
        
        drawImage();
    };
    
    img.onerror = function() {
        console.error("Erreur de chargement de l'image : ", imagePath);
        alert("Erreur de chargement de l'image. Veuillez vérifier le chemin de l'image.");
    };

        img.src = imagePath;
}

function drawImage() {
    if (!currentImage) return;

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.save();

    // Dessiner l'image
    ctx.drawImage(currentImage, 0, 0);

    // Dessiner le point cliqué s'il existe
    if (clickedCoordinates) {
        drawClickedPoint();
    }

    ctx.restore();
}

function drawClickedPoint() {
    if (!clickedCoordinates) return;

    ctx.save();
    ctx.fillStyle = "#FF0000";
    ctx.strokeStyle = "#FFFFFF";
    ctx.lineWidth = 3;
    
    const radius = 12;
    const x = clickedCoordinates.x;
    const y = clickedCoordinates.y;
    
    // Dessiner le point
    ctx.beginPath();
    ctx.arc(x, y, radius, 0, Math.PI * 2);
    ctx.fill();
    ctx.stroke();
    
    ctx.restore();
}

function handleCanvasClick(event) {
    const rect = canvas.getBoundingClientRect();
    const scaleX = canvas.width / rect.width;
    const scaleY = canvas.height / rect.height;
    const x = (event.clientX - rect.left) * scaleX;
    const y = (event.clientY - rect.top) * scaleY;
    
    // Les coordonnées sont directement celles de l'image
    const imageX = x;
    const imageY = y;
    
    // Stocker les coordonnées cliquées
    clickedCoordinates = { x: imageX, y: imageY };
    
    // Mettre à jour les champs cachés pour l'envoi au serveur
    updateHiddenInputs(imageX, imageY);
    
    // Redessiner le canvas avec le point
    drawImage();
    
    console.log(`Point cliqué - Coordonnées image: x: ${imageX}, y: ${imageY}`);
}

function updateHiddenInputs(x, y) {
    // Mettre à jour les champs cachés pour les coordonnées
    let coordXInput = document.getElementById('coordonneeX');
    let coordYInput = document.getElementById('coordonneeY');
    
    if (coordXInput && coordYInput) {
        coordXInput.value = x;
        coordYInput.value = y;
    }
}

// Les fonctions drawPointOnCanvas et loadImageWithPoint restent pour SearchSalle
function drawPointOnCanvas(canvasId, x, y, color = "#FF0000") {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    
    const ctx = canvas.getContext('2d');
    ctx.save();
    
    ctx.fillStyle = color;
    ctx.strokeStyle = "#FFFFFF";
    ctx.lineWidth = 3;
    
    const radius = 12;
    
    ctx.beginPath();
    ctx.arc(x, y, radius, 0, Math.PI * 2);
    ctx.fill();
    ctx.stroke();
    
    ctx.restore();
}

function loadImageWithPoint(canvasId, imagePath, pointX, pointY) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    
    const ctx = canvas.getContext('2d');
    const img = new Image();
    
    img.onload = function() {
        canvas.width = img.width;
        canvas.height = img.height;
        
        // Dessiner l'image
        ctx.drawImage(img, 0, 0);
        
        // Dessiner le point si les coordonnées sont fournies
        if (pointX !== null && pointY !== null) {
            drawPointOnCanvas(canvasId, pointX, pointY);
        }
    };
    
    img.src = imagePath;
}

