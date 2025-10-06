const formSelectEtage = document.querySelector("#etageSelect");
const formSelectFavorie = document.querySelector("#favoriSelect");
const formSelectType = document.querySelector("#typeSalle");
const formSelectAttribut = document.querySelector("#attributSalle");
const sortSelect = document.querySelector("#sortSelect");
const resetFiltersBtn = document.querySelector("#resetFilters");
const salleClick = document.querySelectorAll(".salles_click");
const scrollContainer = document.querySelector(".container_ListSalle");
const body = document.querySelector("body");
const modalContainer = document.querySelector("#modalContainer");

const box_iconFavori = document.querySelectorAll(".box_iconFavori");
const box_iconFavori_visu = document.querySelectorAll(".box_iconFavori_visuSalle");

const closeModal = document.querySelectorAll(".closeModal");

//ajouter des cookies
function setCookie(nomCookie, nouvelElement) {
    let listeStringRecuperee = getCookie(nomCookie);
    let listeRecuperee = listeStringRecuperee ? JSON.parse(listeStringRecuperee) : [];

    // Nettoyer la liste de toute valeur nulle ou indésirable
    listeRecuperee = listeRecuperee.filter(e => e !== null && e !== undefined && e !== "null" && e !== "undefined");

    if (!listeRecuperee.includes(nouvelElement)) {
        listeRecuperee.push(nouvelElement);
    } else {
        const index = listeRecuperee.indexOf(nouvelElement);
        listeRecuperee.splice(index, 1);
    }

    if (listeRecuperee.length === 0) {
        // Supprimer le cookie si la liste est vide
        document.cookie = `${nomCookie}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
    } else {
        let listeMiseAJourJson = JSON.stringify(listeRecuperee);
        // Date d'expiration fixée à l'an 3000
        let dateAn3000 = new Date('3000-01-01T00:00:00Z');
        document.cookie = `${nomCookie}=${listeMiseAJourJson}; expires=${dateAn3000.toUTCString()}; path=/`;
    }
}

//recuperer les cookies
function getCookie(nom) {
    let nomCookie = nom + "=";
    let cookies = document.cookie.split(';');
    for (let i = 0; i < cookies.length; i++) {
        let cookie = cookies[i];
        while (cookie.charAt(0) === ' ') {
            cookie = cookie.substring(1);
        }
        if (cookie.indexOf(nomCookie) === 0) {
            return cookie.substring(nomCookie.length, cookie.length);
        }
    }
    return "";
}

// formulaire de creation de salle adaptation
function showSpecificFields() {
    var typeSalle = document.getElementById('typeSalle').value;

    document.getElementById('reunionFields').style.display = 'none';
    document.getElementById('pauseFields').style.display = 'none';
    document.getElementById('bubbleFields').style.display = 'none';

    switch (typeSalle) {
        case '1': // Réunion
            document.getElementById('reunionFields').style.display = 'block';
            break;
        case '2': // Pause
            document.getElementById('pauseFields').style.display = 'block';
            break;
        case '3': // Bubble
            document.getElementById('bubbleFields').style.display = 'block';
            break;
    }
}
// Appeler la fonction au chargement de la page
window.onload = showSpecificFields;

// option en fonction du type
formSelectType.addEventListener('change', function () {
    var typeSalle = this.value;

    formSelectAttribut.innerHTML = '<option value="all">-- Tous les attributs --</option>';

    if (typeSalle === 'Pause') {
        var options = [
            { value: 'distributeur', text: 'Distributeur' },
            { value: 'frigo', text: 'Frigo' },
        ];
    } else if (typeSalle === 'Reunion') {
        var options = [
            { value: 'ecran', text: 'Ecran' },
            { value: 'camera', text: 'Camera' },
            { value: 'tableaublanc', text: 'Tableau blanc' },
            { value: 'systemeaudio', text: 'Système audio' }
        ];
    } else if (typeSalle === 'Bubble') {
        var options = [
            { value: 'priseelectrique', text: 'Prise électrique' }
        ];
    } else {
        var options = [
            { value: 'distributeur', text: 'Distributeur' },
            { value: 'frigo', text: 'Frigo' },
            { value: 'ecran', text: 'Ecran' },
            { value: 'camera', text: 'Camera' },
            { value: 'tableaublanc', text: 'Tableau blanc' },
            { value: 'systemeaudio', text: 'Système audio' },
            { value: 'priseelectrique', text: 'Prise électrique' },
        ]
    }

    if (options) {
        options.forEach(function (option) {
            var opt = document.createElement('option');
            opt.value = option.value;
            opt.text = option.text;
            formSelectAttribut.add(opt);
        });
    }
});

formSelectEtage.addEventListener("change", filterSalles);
formSelectType.addEventListener("change", filterSalles);
formSelectAttribut.addEventListener("change", filterSalles);
sortSelect.addEventListener("change", sortSalles);

formSelectFavorie.addEventListener("click", toggleFavoriteFilter);

function filterSalles() {
    const etageId = formSelectEtage.value;
    const typeSalle = formSelectType.value;
    const attributSalle = formSelectAttribut.value;
    const isFavorie = document.querySelector(".iconStar-filter");
    const salleFavorie = getCookie("mesSallesFavorites");

    salleClick.forEach(function (salle) {
        const matchesEtage = etageId === "all" || salle.dataset.etage == etageId;
        const matchesType = typeSalle === "all" || salle.dataset.type == typeSalle;

        let matchesAttribut = false;

        // Vérification de l'attribut sélectionné uniquement
        if (attributSalle === "all") {
            matchesAttribut = true;
        } else {
            // Vérification de l'attribut spécifique sélectionné
            matchesAttribut = salle.dataset[attributSalle] === attributSalle;
        }
        console.log(salle.dataset["priseelectrique"])
        let matchesFavorie = false;
        if (isFavorie.getAttribute("d") === dStarFull) {
            matchesFavorie = salleFavorie.includes(salle.dataset.salleclickid);
        } else {
            matchesFavorie = true;
        }

        if (matchesEtage && matchesType && matchesAttribut && matchesFavorie) {
            salle.style.display = "block";
        } else {
            salle.style.display = "none";
        }
    });
}

function sortSalles() {
    const sortValue = sortSelect.value;
    const container = document.querySelector('.container_ListSalle');
    const salles = Array.from(container.querySelectorAll('.salles_click'));
    
    // Filtrer seulement les salles visibles
    const visibleSalles = salles.filter(salle => salle.style.display !== 'none');
    
    if (sortValue === 'default') {
        // Remettre dans l'ordre original
        visibleSalles.forEach(salle => {
            container.appendChild(salle);
        });
        return;
    }
    
    // Trier les salles selon le critère sélectionné
    visibleSalles.sort((a, b) => {
        switch (sortValue) {
            case 'name-asc':
                return a.querySelector('p').textContent.localeCompare(b.querySelector('p').textContent);
            case 'name-desc':
                return b.querySelector('p').textContent.localeCompare(a.querySelector('p').textContent);
            case 'number-asc':
                return parseInt(a.querySelectorAll('p')[1].textContent) - parseInt(b.querySelectorAll('p')[1].textContent);
            case 'number-desc':
                return parseInt(b.querySelectorAll('p')[1].textContent) - parseInt(a.querySelectorAll('p')[1].textContent);
            case 'type-asc':
                return a.dataset.type.localeCompare(b.dataset.type);
            case 'type-desc':
                return b.dataset.type.localeCompare(a.dataset.type);
            case 'floor-asc':
                return parseInt(a.dataset.etage) - parseInt(b.dataset.etage);
            case 'floor-desc':
                return parseInt(b.dataset.etage) - parseInt(a.dataset.etage);
            default:
                return 0;
        }
    });
    
    // Réorganiser les salles dans le DOM
    visibleSalles.forEach(salle => {
        container.appendChild(salle);
    });
}

function toggleFavoriteFilter() {
    const isFavorie = document.querySelector(".iconStar-filter");

    if (isFavorie.getAttribute("d") === dStarEmpty) {
        isFavorie.setAttribute("d", dStarFull);
    } else {
        isFavorie.setAttribute("d", dStarEmpty);
    }
    filterSalles();
}

const dStarFull = "M3.612 15.443c-.386.198-.824-.149-.746-.592l.83-4.73L.173 6.765c-.329-.314-.158-.888.283-.95l4.898-.696L7.538.792c.197-.39.73-.39.927 0l2.184 4.327 4.898.696c.441.062.612.636.282.95l-3.522 3.356.83 4.73c.078.443-.36.79-.746.592L8 13.187l-4.389 2.256z";
const dStarEmpty = "M2.866 14.85c-.078.444.36.791.746.593l4.39-2.256 4.389 2.256c.386.198.824-.149.746-.592l-.83-4.73 3.522-3.356c.33-.314.16-.888-.282-.95l-4.898-.696L8.465.792a.513.513 0 0 0-.927 0L5.354 5.12l-4.898.696c-.441.062-.612.636-.283.95l3.523 3.356-.83 4.73zm4.905-2.767-3.686 1.894.694-3.957a.56.56 0 0 0-.163-.505L1.71 6.745l4.052-.576a.53.53 0 0 0 .393-.288L8 2.223l1.847 3.658a.53.53 0 0 0 .393.288l4.052.575-2.906 2.77a.56.56 0 0 0-.163.506l.694 3.957-3.686-1.894a.5.5 0 0 0-.461 0z";

const AddIconFavorie = function (container, nameDataset) {
    let listSalleFavorite = getCookie("mesSallesFavorites");

    const dStarFull = "M3.612 15.443c-.386.198-.824-.149-.746-.592l.83-4.73L.173 6.765c-.329-.314-.158-.888.283-.95l4.898-.696L7.538.792c.197-.39.73-.39.927 0l2.184 4.327 4.898.696c.441.062.612.636.282.95l-3.522 3.356.83 4.73c.078.443-.36.79-.746.592L8 13.187l-4.389 2.256z";
    const dStarEmpty = "M2.866 14.85c-.078.444.36.791.746.593l4.39-2.256 4.389 2.256c.386.198.824-.149.746-.592l-.83-4.73 3.522-3.356c.33-.314.16-.888-.282-.95l-4.898-.696L8.465.792a.513.513 0 0 0-.927 0L5.354 5.12l-4.898.696c-.441.062-.612.636-.283.95l3.523 3.356-.83 4.73zm4.905-2.767-3.686 1.894.694-3.957a.56.56 0 0 0-.163-.505L1.71 6.745l4.052-.576a.53.53 0 0 0 .393-.288L8 2.223l1.847 3.658a.53.53 0 0 0 .393.288l4.052.575-2.906 2.77a.56.56 0 0 0-.163.506l.694 3.957-3.686-1.894a.5.5 0 0 0-.461 0z";

    container.forEach(infosalle => {
        const salleID = infosalle.dataset[nameDataset];
        const iconSvg = infosalle.querySelector(".iconStar path");

        if (listSalleFavorite.includes(salleID)) {
            iconSvg.setAttribute("d", dStarFull);
        } else {
            iconSvg.setAttribute("d", dStarEmpty);
        }
    });
}

const updateIcon = function (){
    AddIconFavorie(box_iconFavori_visu, "salleinfoid");
    AddIconFavorie(box_iconFavori, "salleidfavorite");
}

updateIcon()

const iconsStar = document.querySelectorAll(".iconStar");

iconsStar.forEach(iconStar => {

    iconStar.addEventListener("click", function (e) {
        e.stopPropagation();
        var parentIcon = iconStar.parentNode;

        if (parentIcon.classList.contains("box_iconFavori_visuSalle")) {
            setCookie("mesSallesFavorites", parentIcon.dataset.salleinfoid);
            updateIcon()
        } else {
            setCookie("mesSallesFavorites", parentIcon.dataset.salleidfavorite);
            updateIcon()
        }

    })
})

// Fonction pour réinitialiser tous les filtres
function resetAllFilters() {
    // Réinitialiser les sélecteurs
    formSelectEtage.value = "all";
    formSelectType.value = "all";
    formSelectAttribut.value = "all";
    sortSelect.value = "default";
    
    // Réinitialiser le filtre favori
    const iconStarFilter = document.querySelector(".iconStar-filter");
    if (iconStarFilter) {
        iconStarFilter.setAttribute("d", dStarEmpty);
    }
    
    // Réinitialiser le champ de recherche
    const searchInput = document.querySelector("#salleInput");
    if (searchInput) {
        searchInput.value = "";
    }
    
    // Afficher toutes les salles
    salleClick.forEach(function (salle) {
        salle.style.display = "block";
    });
    
    // Remettre les salles dans l'ordre original
    const container = document.querySelector('.container_ListSalle');
    const salles = Array.from(container.querySelectorAll('.salles_click'));
    salles.forEach(salle => {
        container.appendChild(salle);
    });
    
    // Mettre à jour les icônes de favoris
    updateIcon();
}

// Ajouter l'écouteur d'événement pour le bouton de réinitialisation
resetFiltersBtn.addEventListener("click", resetAllFilters);

// Fonction pour créer le modal d'une salle
function createSalleModal(salleData) {
    const modal = document.createElement('div');
    modal.className = 'container_plan_info';
    modal.style.display = 'block';
    
    const typeSalle = salleData.type;
    let attributsHTML = '';
    
    // Générer les attributs selon le type de salle
    if (typeSalle === 'Pause') {
        attributsHTML = `
            <div class="attributs-container">
                <div class="attribut-item ${salleData.distributeur === 'distributeur' ? 'attribut-present' : 'attribut-absent'}" title="${salleData.distributeur === 'distributeur' ? 'Distributeur présent' : 'Pas de distributeur'}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M8 1a2 2 0 0 1 2 2v4H6V3a2 2 0 0 1 2-2zm3 6V3a3 3 0 0 0-6 0v4a2 2 0 0 0-2 2v5a2 2 0 0 0 2 2h6a2 2 0 0 0 2-2V9a2 2 0 0 0-2-2zM5 8h6a1 1 0 0 1 1 1v5a1 1 0 0 1-1 1H5a1 1 0 0 1-1-1V9a1 1 0 0 1 1-1z" />
                    </svg>
                    <span>Distributeur</span>
                </div>
                <div class="attribut-item" title="Micro-ondes: ${salleData.microondes}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M2 2a2 2 0 0 0-2 2v8a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V4a2 2 0 0 0-2-2H2zm12 1a1 1 0 0 1 1 1v6a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1V4a1 1 0 0 1 1-1h12z" />
                        <path d="M4 6a1 1 0 0 1 1-1h6a1 1 0 0 1 1 1v2a1 1 0 0 1-1 1H5a1 1 0 0 1-1-1V6z" />
                    </svg>
                    <span>Micro-ondes: ${salleData.microondes}</span>
                </div>
                <div class="attribut-item ${salleData.frigo === 'frigo' ? 'attribut-present' : 'attribut-absent'}" title="${salleData.frigo === 'frigo' ? 'Frigo présent' : 'Pas de frigo'}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M4 1.5H3a2 2 0 0 0-2 2V14a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V3.5a2 2 0 0 0-2-2h-1v1h1a1 1 0 0 1 1 1V14a1 1 0 0 1-1 1H3a1 1 0 0 1-1-1V3.5a1 1 0 0 1 1-1h1v-1z" />
                        <path d="M9.5 1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-3a.5.5 0 0 1-.5-.5v-1a.5.5 0 0 1 .5-.5h3zm-3-1A1.5 1.5 0 0 0 5 1.5v1A1.5 1.5 0 0 0 6.5 4h3A1.5 1.5 0 0 0 11 2.5v-1A1.5 1.5 0 0 0 9.5 0h-3z" />
                    </svg>
                    <span>Frigo</span>
                </div>
                <div class="attribut-item" title="Evier: ${salleData.evier}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M8 16A8 8 0 1 0 8 0a8 8 0 0 0 0 16zm.93-9.412-1 4.705c-.07.34.029.533.304.533.194 0 .487-.07.686-.246l-.088.416c-.287.346-.92.598-1.465.598-.703 0-1.002-.422-.808-1.319l.738-3.468c.064-.293.006-.399-.287-.47l-.451-.081.082-.381 2.29-.287zM8 5.5a1 1 0 1 1 0-2 1 1 0 0 1 0 2z" />
                    </svg>
                    <span>Evier: ${salleData.evier}</span>
                </div>
                <div class="attribut-item" title="Chaises: ${salleData.nbplaces}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M8 16A8 8 0 1 0 8 0a8 8 0 0 0 0 16zm.93-9.412-1 4.705c-.07.34.029.533.304.533.194 0 .487-.07.686-.246l-.088.416c-.287.346-.92.598-1.465.598-.703 0-1.002-.422-.808-1.319l.738-3.468c.064-.293.006-.399-.287-.47l-.451-.081.082-.381 2.29-.287zM8 5.5a1 1 0 1 1 0-2 1 1 0 0 1 0 2z" />
                    </svg>
                    <span>Chaises: ${salleData.nbplaces}</span>
                </div>
                <div class="attribut-item" title="Tables: ${salleData.nbtables}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M8 16A8 8 0 1 0 8 0a8 8 0 0 0 0 16zm.93-9.412-1 4.705c-.07.34.029.533.304.533.194 0 .487-.07.686-.246l-.088.416c-.287.346-.92.598-1.465.598-.703 0-1.002-.422-.808-1.319l.738-3.468c.064-.293.006-.399-.287-.47l-.451-.081.082-.381 2.29-.287zM8 5.5a1 1 0 1 1 0-2 1 1 0 0 1 0 2z" />
                    </svg>
                    <span>Tables: ${salleData.nbtables}</span>
                </div>
            </div>
        `;
    } else if (typeSalle === 'Reunion') {
        attributsHTML = `
            <div class="attributs-container">
                <div class="attribut-item ${salleData.ecran === 'ecran' ? 'attribut-present' : 'attribut-absent'}" title="${salleData.ecran === 'ecran' ? 'Écran présent' : 'Pas d\'écran'}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M2.5 13.5A.5.5 0 0 1 3 13h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zM13.991 3l.024.001a1.46 1.46 0 0 1 .538.143.757.757 0 0 1 .302.254c.067.1.145.277.145.602v5.991l-.001.024a1.464 1.464 0 0 1-.143.538.758.758 0 0 1-.254.302c-.1.067-.277.145-.602.145H2.009l-.024-.001a1.464 1.464 0 0 1-.538-.143.758.758 0 0 1-.302-.254C1.078 10.502 1 10.325 1 10V4.009l.001-.024a1.46 1.46 0 0 1 .143-.538.758.758 0 0 1 .254-.302C1.498 3.078 1.675 3 2 3h11.991zM14 2H2C0 2 0 4 0 4v6c0 2 2 2 2 2h12c2 0 2-2 2-2V4c0-2-2-2-2-2z" />
                    </svg>
                    <span>Écran</span>
                </div>
                <div class="attribut-item ${salleData.camera === 'camera' ? 'attribut-present' : 'attribut-absent'}" title="${salleData.camera === 'camera' ? 'Caméra présente' : 'Pas de caméra'}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M15 12a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1V6a1 1 0 0 1 1-1h1.172a3 3 0 0 0 2.12-.879l.83-.828A1 1 0 0 1 6.827 3h2.344a1 1 0 0 1 .707.293l.828.828A3 3 0 0 0 12.828 5H14a1 1 0 0 1 1 1v6zM2 4a2 2 0 0 0-2 2v6a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V6a2 2 0 0 0-2-2h-1.172a2 2 0 0 1-1.414-.586l-.828-.828A2 2 0 0 0 9.172 2H6.828a2 2 0 0 0-1.414.586l-.828-.828A2 2 0 0 1 3.172 4H2z" />
                        <path d="M8 11a2.5 2.5 0 1 1 0-5 2.5 2.5 0 0 1 0 5zm0 1a3.5 3.5 0 1 0 0-7 3.5 3.5 0 0 0 0 7zM3 6.5a.5.5 0 1 1-1 0 .5.5 0 0 1 1 0z" />
                    </svg>
                    <span>Caméra</span>
                </div>
                <div class="attribut-item ${salleData.tableaublanc === 'tableaublanc' ? 'attribut-present' : 'attribut-absent'}" title="${salleData.tableaublanc === 'tableaublanc' ? 'Tableau blanc présent' : 'Pas de tableau blanc'}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M14.5 3a.5.5 0 0 1 .5.5v9a.5.5 0 0 1-.5.5h-13a.5.5 0 0 1-.5-.5v-9a.5.5 0 0 1 .5-.5h13zm-13-1A1.5 1.5 0 0 0 0 3.5v9A1.5 1.5 0 0 0 1.5 14h13a1.5 1.5 0 0 0 1.5-1.5v-9A1.5 1.5 0 0 0 14.5 2h-13z" />
                        <path d="M7 5.5a.5.5 0 0 1 .5-.5h5a.5.5 0 0 1 0 1h-5a.5.5 0 0 1-.5-.5zm-1.496-.854a.5.5 0 0 1 0 .708l-1.5 1.5a.5.5 0 0 1-.708 0l-.5-.5a.5.5 0 1 1 .708-.708l.146.147 1.146-1.147a.5.5 0 0 1 .708 0zM7 9.5a.5.5 0 0 1 .5-.5h5a.5.5 0 0 1 0 1h-5a.5.5 0 0 1-.5-.5zm-1.496-.854a.5.5 0 0 1 0 .708l-1.5 1.5a.5.5 0 0 1-.708 0l-.5-.5a.5.5 0 0 1 .708-.708l.146.147 1.146-1.147a.5.5 0 0 1 .708 0z" />
                    </svg>
                    <span>Tableau blanc</span>
                </div>
                <div class="attribut-item ${salleData.systemeaudio === 'systemeaudio' ? 'attribut-present' : 'attribut-absent'}" title="${salleData.systemeaudio === 'systemeaudio' ? 'Système audio présent' : 'Pas de système audio'}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M3.5 6.5A.5.5 0 0 1 4 7v1a4 4 0 0 0 8 0V7a.5.5 0 0 1 1 0v1a5 5 0 0 1-4.5 4.975V15h3a.5.5 0 0 1 0 1h-7a.5.5 0 0 1 0-1h3v-2.025A5 5 0 0 1 3 8V7a.5.5 0 0 1 .5-.5z" />
                        <path d="M10 8a2 2 0 1 1-4 0V3a2 2 0 1 1 4 0v5zM8 0a3 3 0 0 0-3 3v5a3 3 0 0 0 6 0V3a3 3 0 0 0-3-3z" />
                    </svg>
                    <span>Système audio</span>
                </div>
                <div class="attribut-item" title="Chaises: ${salleData.nbplaces}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M8 16A8 8 0 1 0 8 0a8 8 0 0 0 0 16zm.93-9.412-1 4.705c-.07.34.029.533.304.533.194 0 .487-.07.686-.246l-.088.416c-.287.346-.92.598-1.465.598-.703 0-1.002-.422-.808-1.319l.738-3.468c.064-.293.006-.399-.287-.47l-.451-.081.082-.381 2.29-.287zM8 5.5a1 1 0 1 1 0-2 1 1 0 0 1 0 2z" />
                    </svg>
                    <span>Chaises: ${salleData.nbplaces}</span>
                </div>
                <div class="attribut-item" title="Tables: ${salleData.nbtables}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M8 16A8 8 0 1 0 8 0a8 8 0 0 0 0 16zm.93-9.412-1 4.705c-.07.34.029.533.304.533.194 0 .487-.07.686-.246l-.088.416c-.287.346-.92.598-1.465.598-.703 0-1.002-.422-.808-1.319l.738-3.468c.064-.293.006-.399-.287-.47l-.451-.081.082-.381 2.29-.287zM8 5.5a1 1 0 1 1 0-2 1 1 0 0 1 0 2z" />
                    </svg>
                    <span>Tables: ${salleData.nbtables}</span>
                </div>
            </div>
        `;
    } else if (typeSalle === 'Bubble') {
        attributsHTML = `
            <div class="attributs-container">
                <div class="attribut-item ${salleData.priseelectrique === 'priseelectrique' ? 'attribut-present' : 'attribut-absent'}" title="${salleData.priseelectrique === 'priseelectrique' ? 'Prise électrique présente' : 'Pas de prise électrique'}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M8 16A8 8 0 1 0 8 0a8 8 0 0 0 0 16zm.93-9.412-1 4.705c-.07.34.029.533.304.533.194 0 .487-.07.686-.246l-.088.416c-.287.346-.92.598-1.465.598-.703 0-1.002-.422-.808-1.319l.738-3.468c.064-.293.006-.399-.287-.47l-.451-.081.082-.381 2.29-.287zM8 5.5a1 1 0 1 1 0-2 1 1 0 0 1 0 2z" />
                    </svg>
                    <span>Prise électrique</span>
                </div>
                <div class="attribut-item" title="Chaises: ${salleData.nbplaces}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M8 16A8 8 0 1 0 8 0a8 8 0 0 0 0 16zm.93-9.412-1 4.705c-.07.34.029.533.304.533.194 0 .487-.07.686-.246l-.088.416c-.287.346-.92.598-1.465.598-.703 0-1.002-.422-.808-1.319l.738-3.468c.064-.293.006-.399-.287-.47l-.451-.081.082-.381 2.29-.287zM8 5.5a1 1 0 1 1 0-2 1 1 0 0 1 0 2z" />
                    </svg>
                    <span>Chaises: ${salleData.nbplaces}</span>
                </div>
                <div class="attribut-item" title="Tables: ${salleData.nbtables}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M8 16A8 8 0 1 0 8 0a8 8 0 0 0 0 16zm.93-9.412-1 4.705c-.07.34.029.533.304.533.194 0 .487-.07.686-.246l-.088.416c-.287.346-.92.598-1.465.598-.703 0-1.002-.422-.808-1.319l.738-3.468c.064-.293.006-.399-.287-.47l-.451-.081.082-.381 2.29-.287zM8 5.5a1 1 0 1 1 0-2 1 1 0 0 1 0 2z" />
                    </svg>
                    <span>Tables: ${salleData.nbtables}</span>
                </div>
            </div>
        `;
    }

    modal.innerHTML = `
        <span class="closeModal">
            <svg xmlns="http://www.w3.org/2000/svg" width="25" height="25" fill="currentColor" class="bi bi-x-lg iconCLose" viewBox="0 0 16 16">
                <path d="M2.146 2.854a.5.5 0 1 1 .708-.708L8 7.293l5.146-5.147a.5.5 0 0 1 .708.708L8.707 8l5.147 5.146a.5.5 0 0 1-.708.708L8 8.707l-5.146 5.147a.5.5 0 0 1-.708-.708L7.293 8z" />
            </svg>
        </span>
        <div class="section_infoSalle">
            <div class="box_imgSalle">
                <img src="${salleData.salleimg}" alt="Salle" class="imgSalle" />
            </div>
            <div class="box_iconFavori" data-salleidfavorite="${salleData.salleclickid}">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-star-fill iconFavoriFill iconStar" viewBox="0 0 16 16">
                    <path class="iconStar" d="" />
                </svg>
            </div>
            <div class="list-info">
                <div class="box_infoPrin_salle_modal">
                    <p class="p_infoSalle salle-nom">${salleData.sallenom}</p>
                    <p class="p_infoSalle salle-numero">${salleData.sallenum}</p>
                    <p class="p_infoSalle salle-etage">${salleData.salleetage}</p>
                    <p class="p_infoSalle salle-type">${salleData.type}</p>
                    <a href="mailto:hugomeuiel@gmail.com">Signaler</a>
                </div>
                <div class="box_infoPrin_salle_modal box_info_type">
                    ${attributsHTML}
                </div>
            </div>
        </div>
        <div class="section_imgPlan">
            <canvas id="planCanvas_${salleData.salleclickid}"
                    class="img-plan"
                    data-imgpath="${salleData.planimg}"
                    data-coordx="${salleData.coordx}"
                    data-coordy="${salleData.coordy}">
            </canvas>
        </div>
    `;

    return modal;
}

// Fonction pour afficher le modal d'une salle
function showSalleModal(salleElement) {
    // Supprimer l'ancien modal s'il existe
    const existingModal = modalContainer.querySelector('.container_plan_info');
    if (existingModal) {
        existingModal.remove();
    }

    // Récupérer les données de la salle
    const salleData = {
        salleclickid: salleElement.dataset.salleclickid,
        sallenom: salleElement.dataset.sallenom,
        sallenum: salleElement.dataset.sallenum,
        salleetage: salleElement.dataset.salleetage,
        salleimg: salleElement.dataset.salleimg,
        planimg: salleElement.dataset.planimg,
        coordx: salleElement.dataset.coordx,
        coordy: salleElement.dataset.coordy,
        type: salleElement.dataset.type,
        distributeur: salleElement.dataset.distributeur,
        frigo: salleElement.dataset.frigo,
        microondes: salleElement.dataset.microondes,
        evier: salleElement.dataset.evier,
        nbplaces: salleElement.dataset.nbplaces,
        nbtables: salleElement.dataset.nbtables,
        ecran: salleElement.dataset.ecran,
        camera: salleElement.dataset.camera,
        tableaublanc: salleElement.dataset.tableaublanc,
        systemeaudio: salleElement.dataset.systemeaudio,
        priseelectrique: salleElement.dataset.priseelectrique
    };

    // Créer et afficher le nouveau modal
    const modal = createSalleModal(salleData);
    modalContainer.appendChild(modal);

    // Ajouter la classe pour désactiver le body
    body.classList.add('bodyBackDesable');
    document.body.style.overflow = "hidden";

    // Mettre à jour les icônes de favoris
    updateIcon();

    // Ajouter l'événement de fermeture sur le bouton X
    const closeBtn = modal.querySelector('.closeModal');
    closeBtn.addEventListener('click', closeSalleModal);

    // Fermer le modal en cliquant à l'extérieur
    modal.addEventListener('click', function(e) {
        if (e.target === modal) {
            closeSalleModal();
        }
    });

    // Ajouter un gestionnaire global pour fermer le modal en cliquant à l'extérieur
    const handleOutsideClick = function(e) {
        if (!modal.contains(e.target) && !e.target.closest('.salles_click')) {
            closeSalleModal();
            document.removeEventListener('click', handleOutsideClick);
        }
    };
    
    // Attendre un peu avant d'ajouter l'événement pour éviter la fermeture immédiate
    setTimeout(() => {
        document.addEventListener('click', handleOutsideClick);
    }, 100);

    // Initialiser le canvas
    setTimeout(() => {
        const canvas = modal.querySelector('canvas');
        if (canvas && typeof loadModalCanvas === 'function') {
            loadModalCanvas(salleData.salleclickid);
        }
    }, 100);
}

// Fonction pour fermer le modal
function closeSalleModal() {
    const modal = modalContainer.querySelector('.container_plan_info');
    if (modal) {
        modal.remove();
        body.classList.remove('bodyBackDesable');
        document.body.style.overflow = "auto";
    }
}

// Ajouter les événements de clic sur les salles
salleClick.forEach(salle => {
    salle.addEventListener('click', function(e) {
        // Empêcher l'ouverture du modal si on clique sur l'icône de favori
        if (e.target.closest('.box_iconFavori_visuSalle')) {
            e.preventDefault();
            e.stopPropagation();
            return;
        }
        
        e.preventDefault();
        showSalleModal(this);
    });
});








