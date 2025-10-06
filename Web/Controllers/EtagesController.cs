using Azure;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Web.Models;
using Web.ViewModel;
namespace Web.Controllers
{
    public class EtagesController : Controller
    {
        private readonly IRepository<Etage> _etageRepository;
        private readonly IRepository<Salle> _salleRepository;
        private readonly ISalleManager _salleManager;

        public EtagesController(IRepository<Etage> etageRepository, 
            IRepository<Salle> salleRepository, 
            ISalleManager salleManager)
        {
            _etageRepository = etageRepository;
            _salleRepository = salleRepository;
            _salleManager = salleManager;
        }

        public IActionResult Create()
        {
            return View();
        }

        #region Affiche Salle && Etage

        public async Task<IActionResult> SearchSalle()
        {
            var viewModel = new SalleViewModel
            {
                salles = await _salleRepository.GetAllAsync(),
                etages = await _etageRepository.GetAllAsync(),

            };

            return View("SearchSalle", viewModel);
        }
            

    
        #endregion



        public IActionResult CreateEtage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEtage(int Niveau, string Nom, IFormFile ImgPlanEtagePath)
        {
            if (ImgPlanEtagePath != null && ImgPlanEtagePath.Length > 0)
            {
                // Chemin de base où les fichiers seront stockés
                var assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets/PlansEtages/");

                // Chemin complet du fichier
                var filePath = Path.Combine(assetsPath, ImgPlanEtagePath.FileName);
                

                // Sauvegarde du fichier
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImgPlanEtagePath.CopyToAsync(stream);
                }

                // Création de l'objet Etage et ajout à la base de données
                var etage = new Etage { Niveau = Niveau, Nom = Nom, ImgPlanEtagePath = "assets/PlansEtages/" + ImgPlanEtagePath.FileName };
                await _etageRepository.AddAsync(etage);
                await _etageRepository.SaveChangesAsync();
            }

            return RedirectToAction(nameof(SearchSalle));
        }

        // -------------------------------------- Created Salle -----------------------------------------


        public async Task<IActionResult> CreateSalle()
        {
            ViewBag.EtageId = await _etageRepository.GetAllAsync();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateSalle(
        int Numero,
        string? Nom,
        int EtageId,
        IFormFile ImgSallePath,
        TypeSalle TypeSalle,
        // Coordonnées sur le plan
        string CoordonneeX,
        string CoordonneeY,
        int? NbTables,
        int? NbPlaces,
        // Paramètres pour SalleReunion
        bool Ecran = false,
        bool Camera = false,
        bool TableauBlanc = false,
        bool SystemeAudio = false,
        // Paramètres pour SallePause
        int MicroOndes = 0,
        bool Frigo = false,
        int Evier = 0,
        bool Distributeur = false,
        // Paramètres pour SalleBubble
        bool PriseElectrique = false)
        {
            string imagePath = "assets/Salles/DefautSalle.png";

            if (ImgSallePath != null && ImgSallePath.Length > 0)
            {
                var assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets/Salles/");
                var filePath = Path.Combine(assetsPath, ImgSallePath.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImgSallePath.CopyToAsync(stream);
                }
                imagePath = "assets/Salles/" + ImgSallePath.FileName;
            }
                
       
            Salle salle = TypeSalle switch
            {
                TypeSalle.Reunion => new SalleReunion
                {
                    Numero = Numero,
                    Nom = Nom,
                    EtageId = EtageId,
                    ImgSallePath = imagePath,
                    TypeSalle = TypeSalle,
                    CoordonneeX = CoordonneeX,
                    CoordonneeY = CoordonneeY,
                    NbTables = NbTables,
                    NbPlaces = NbPlaces,
                    Ecran = Ecran,
                    Camera = Camera,
                    TableauBlanc = TableauBlanc,
                    SystemeAudio = SystemeAudio,
                },
                TypeSalle.Pause => new SallePause
                {
                    Numero = Numero,
                    Nom = Nom,
                    EtageId = EtageId,
                    ImgSallePath = imagePath,
                    TypeSalle = TypeSalle,
                    CoordonneeX = CoordonneeX,
                    CoordonneeY = CoordonneeY,
                    NbTables = NbTables,
                    NbPlaces = NbPlaces,
                    MicroOndes = MicroOndes,
                    Frigo = Frigo,
                    Evier = Evier,
                    Distributeur = Distributeur,

                },
                TypeSalle.Bubble => new SalleBubble
                {
                    Numero = Numero,
                    Nom = Nom,
                    EtageId = EtageId,
                    ImgSallePath = imagePath,
                    TypeSalle = TypeSalle,
                    CoordonneeX = CoordonneeX,
                    CoordonneeY = CoordonneeY,
                    NbTables = NbTables,
                    NbPlaces = NbPlaces,
                    PriseElectrique = PriseElectrique,
                },
                _ => new Salle
                {
                    Numero = Numero,
                    Nom = Nom,
                    EtageId = EtageId,
                    ImgSallePath = imagePath,
                    TypeSalle = TypeSalle,
                    CoordonneeX = CoordonneeX,
                    CoordonneeY = CoordonneeY,
                    NbTables = NbTables,
                    NbPlaces = NbPlaces,

                }
            };
            
            await _salleRepository.AddAsync(salle);
            await _salleRepository.SaveChangesAsync();

            return RedirectToAction(nameof(SearchSalle));
        }




    }
}


