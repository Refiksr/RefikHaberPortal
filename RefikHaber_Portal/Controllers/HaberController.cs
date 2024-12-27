using RefikHaber.Models;
using RefikHaber.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RefikHaber.Repostories;
using Microsoft.AspNetCore.SignalR;
using RefikHaber.Hubs;
using RefikHaber.Models;


namespace habercPortali1.Controllers
{
    public class HaberController : Controller
    {
        private readonly HaberRepository _haberRepository;
        private readonly HaberTuruRepository _haberTuruRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<GeneralHub> _hubContext;

        public HaberController(HaberRepository haberRepository,
                               HaberTuruRepository haberTuruRepository,
                               IWebHostEnvironment webHostEnvironment,
                               IHubContext<GeneralHub> hubContext)
        {
            _haberRepository = haberRepository;
            _haberTuruRepository = haberTuruRepository;
            _webHostEnvironment = webHostEnvironment;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Admin,Kullanici")]
        public IActionResult Index()
        {
            List<Haber> objHaberList = _haberRepository.GetAll(includeProps: "HaberTuru").ToList();
            return View(objHaberList);
        }

        [Authorize(Roles = "Admin,Kullanici")]
        public IActionResult Privacy()
        {
            List<Haber> objHaberList = _haberRepository.GetAll(includeProps: "HaberTuru").ToList();
            return View(objHaberList);
        }

        [Authorize(Roles = UserRoles.Role_Admin)]
        public IActionResult EkleGuncelle(int? id)
        {
            IEnumerable<SelectListItem> HaberTuruList = _haberTuruRepository.GetAll()
                .Select(k => new SelectListItem
                {
                    Text = k.Ad,
                    Value = k.Id.ToString(),
                });

            ViewBag.HaberTuruList = HaberTuruList;

            if (id == null || id == 0)
            {
                return View();
            }
            else
            {
                Haber? haberVt = _haberRepository.Get(u => u.Id == id);
                if (haberVt == null)
                {
                    return NotFound();
                }
                return View(haberVt);
            }
        }

        [Authorize(Roles = UserRoles.Role_Admin)]
        [HttpPost]
        public async Task<IActionResult> EkleGuncelle(Haber haber, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string haberPath = Path.Combine(wwwRootPath, @"img");

                if (file != null)
                {
                    using (var fileStream = new FileStream(Path.Combine(haberPath, file.FileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    haber.ResimUrl = @"\img\" + file.FileName;
                }

                if (haber.Id == 0)
                {
                    _haberRepository.Ekle(haber);
                    TempData["basarili"] = "Yeni Haber Başarıyla Oluşturuldu!";
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"Yeni bir haber eklendi: {haber.Baslik}");
                }
                else
                {
                    _haberRepository.Guncelle(haber);
                    TempData["basarili"] = "Haber Güncelleme Başarılı!";
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"Haber güncellendi: {haber.Baslik}");
                }

                _haberRepository.Kaydet();
                return RedirectToAction("Index", "Haber");
            }
            return View();
        }

        [Authorize(Roles = UserRoles.Role_Admin)]
        public IActionResult Sil(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Haber? haberVt = _haberRepository.Get(u => u.Id == id);
            if (haberVt == null)
            {
                return NotFound();
            }
            return View(haberVt);
        }

        [Authorize(Roles = UserRoles.Role_Admin)]
        [HttpPost, ActionName("Sil")]
        public IActionResult SilPOST(int? id)
        {
            Haber? haber = _haberRepository.Get(u => u.Id == id);
            if (haber == null)
            {
                return NotFound();
            }
            _haberRepository.Sil(haber);
            _haberRepository.Kaydet();
            TempData["basarili"] = "Haber Başarıyla Silindi";
            return RedirectToAction("Index", "Haber");
        }

        [Authorize(Roles = "Admin,Kullanici")]
        public IActionResult Goruntule(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Haber? haber = _haberRepository.Get(h => h.Id == id, includeProps: "HaberTuru");

            if (haber == null)
            {
                return NotFound();
            }

            return View(haber);
        }
        [Route("api/v1/haber/")]
        [HttpGet]
        public JsonResult GetAllNews()
        {
            var haberler = _haberRepository.GetAll();
            return Json(haberler);
        }
    }
}

