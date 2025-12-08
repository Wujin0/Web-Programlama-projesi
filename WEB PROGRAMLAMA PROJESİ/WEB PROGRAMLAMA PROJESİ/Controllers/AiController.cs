using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace FitnessApp.Controllers
{
    [Authorize]
    public class AiController : Controller
    {
        // Şifreyi okuyacak araç (Configuration)
        private readonly IConfiguration _configuration;

        // Constructor (Yapıcı Metot) - Kasaya erişimi sağlar
        public AiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key=";

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(double boy, double kilo, string cinsiyet, string hedef)
        {
            ViewBag.GirisBoy = boy;
            ViewBag.GirisKilo = kilo;

            // --- GÜVENLİK ADIMI ---
            // Şifreyi koddan değil, gizli kasadan okuyoruz
            string apiKey = _configuration["GoogleGeminiKey"];

            if (string.IsNullOrEmpty(apiKey) || apiKey.Contains("BURAYA_"))
            {
                ViewBag.Sonuc = "HATA: API Key bulunamadı. Lütfen 'Manage User Secrets' ayarını kontrol edin.";
                ViewBag.SonucVar = true;
                return View();
            }

            try
            {
                // PROMPT AYNI KALIYOR
                string cinsiyetTerimi = cinsiyet == "Erkek" ? "man" : "woman";
                string hedefTerimi = hedef == "fit" ? "fitness model, lean body" : "bodybuilder, muscular physique";

                string prompt = @$"Ben bir spor salonu üyesiyim. 
                                Boy: {boy} cm, Kilo: {kilo} kg, Cinsiyet: {cinsiyet}, Hedef: {hedef}.
                                
                                Bana iki parça halinde cevap ver:
                                PARÇA 1: Samimi bir spor hocası gibi 3 maddelik kısa özet (VKİ, Egzersiz, Beslenme). Emojiler kullan.
                                PARÇA 2: İnternetten, bu hedefe ulaşmış birinin ({cinsiyetTerimi}, {hedefTerimi}) motive edici, yüksek kaliteli bir fotoğrafının direkt linkini (URL) bul ve sadece linki yaz. 
                                
                                Cevabı şu formatta ver:
                                [METIN_BASLA]
                                ...buraya metin gelecek...
                                [METIN_BITTI]
                                [RESIM_LINKI]
                                ...buraya sadece https://... ile başlayan link gelecek...
                                [RESIM_BITTI]";

                var payload = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                string jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    // Şifreyi burada değişkenden alıp kullanıyoruz
                    var response = await client.PostAsync(ApiUrl + apiKey, content);
                    var resultJson = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        ViewBag.SonucMetni = $"HATA: {resultJson}";
                    }
                    else
                    {
                        using (JsonDocument doc = JsonDocument.Parse(resultJson))
                        {
                            if (doc.RootElement.TryGetProperty("candidates", out JsonElement candidates) && candidates.GetArrayLength() > 0)
                            {
                                var fullText = candidates[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

                                string metin = "";
                                string resimLinki = "";

                                int metinBasla = fullText.IndexOf("[METIN_BASLA]") + 13;
                                int metinBitti = fullText.IndexOf("[METIN_BITTI]");
                                if (metinBasla >= 13 && metinBitti > metinBasla)
                                {
                                    metin = fullText.Substring(metinBasla, metinBitti - metinBasla).Trim();
                                    metin = metin.Replace("**", "<b>").Replace("*", "<br>•");
                                }

                                int resimBasla = fullText.IndexOf("[RESIM_LINKI]") + 13;
                                int resimBitti = fullText.IndexOf("[RESIM_BITTI]");
                                if (resimBasla >= 13 && resimBitti > resimBasla)
                                {
                                    resimLinki = fullText.Substring(resimBasla, resimBitti - resimBasla).Trim();
                                }

                                if (string.IsNullOrEmpty(resimLinki) || !resimLinki.StartsWith("http"))
                                {
                                    resimLinki = "https://images.unsplash.com/photo-1517836357463-d25dfeac3438?q=80&w=1470&auto=format&fit=crop";
                                }

                                ViewBag.SonucMetni = metin;
                                ViewBag.ResimUrl = resimLinki;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { ViewBag.SonucMetni = $"KRİTİK HATA: {ex.Message}"; }

            ViewBag.SonucVar = true;
            return View();
        }
    }
}