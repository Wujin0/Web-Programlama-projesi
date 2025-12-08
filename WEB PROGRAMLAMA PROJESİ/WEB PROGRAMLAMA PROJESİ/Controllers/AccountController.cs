using FitnessApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Kullanıcıyı bul
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                // Şifreyi kontrol et. "false" parametresi "beni hatırla" kapalı demek.
                // Son parametre "false" -> hatalı girişte hesabı kilitleme demek.
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Email veya şifre hatalı!";
            return View();
        }

        // Çıkış Yapma (Logout)
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ==========================================
        // KAYIT OLMA (REGISTER) İŞLEMLERİ
        // ==========================================

        // 1. Kayıt Sayfasını Göster (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // 2. Kayıt İşlemini Yap (POST)
        [HttpPost]
        public async Task<IActionResult> Register(string ad, string soyad, string email, string password)
        {
            // Basit validasyon
            if (string.IsNullOrEmpty(ad) || string.IsNullOrEmpty(soyad) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Lütfen tüm alanları doldurunuz.";
                return View();
            }

            // Yeni kullanıcı nesnesi oluştur
            var user = new AppUser
            {
                UserName = email, // Kullanıcı adı email olsun
                Email = email,
                Ad = ad,
                Soyad = soyad,
                EmailConfirmed = true, // Email doğrulama ile uğraşmamak için direkt onaylıyoruz
                DogumTarihi = DateTime.Now // Zorunlu alan hatası vermesin diye şimdilik atıyoruz
            };

            // Kullanıcıyı oluştur (Şifreyi hashleyerek kaydeder)
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Başarılıysa kullanıcıya "Uye" rolünü ver
                await _userManager.AddToRoleAsync(user, "Uye");

                // Otomatik giriş yaptır
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Anasayfaya gönder
                return RedirectToAction("Index", "Home");
            }

            // Hata varsa (örn: şifre çok basitse) hatayı ekrana yaz
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }
    }
}