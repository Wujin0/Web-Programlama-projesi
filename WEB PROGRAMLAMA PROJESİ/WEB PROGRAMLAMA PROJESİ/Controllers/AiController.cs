using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace FitnessApp.Controllers
{
    [Authorize]
    public class AiController : Controller
    {
        private readonly IConfiguration _configuration;

        public AiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // SENİN İSTEDİĞİN MODEL: gemini-2.5-flash
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

            // Şifreyi User Secrets'tan (Gizli Kasa) alıyoruz
            string apiKey = _configuration["GoogleGeminiKey"];

            if (string.IsNullOrEmpty(apiKey) || apiKey.Contains("BURAYA_"))
            {
                ViewBag.SonucMetni = "HATA: API Key bulunamadı. Lütfen 'Kullanıcı Parolalarını Yönet' kısmını kontrol et.";
                ViewBag.SonucVar = true;
                return View();
            }

            try
            {
                string cinsiyetTerimi = cinsiyet == "Erkek" ? "man" : "woman";
                string hedefTerimi = hedef == "fit" ? "fitness model, lean body" : "bodybuilder, muscular physique";

                // Prompt: Hem metin hem resim linki istiyoruz
                string prompt = @$"Ben bir spor salonu üyesiyim. 
                                Boy: {boy} cm, Kilo: {kilo} kg, Cinsiyet: {cinsiyet}, Hedef: {hedef}.
                                
                                Bana iki parça halinde cevap ver:
                                PARÇA 1: Samimi bir spor hocası gibi 3 maddelik kısa özet (VKİ, Egzersiz, Beslenme). Emojiler kullan.
                                PARÇA 2: İnternetten, bu hedefe ulaşmış birinin ({cinsiyetTerimi}, {hedefTerimi}) motive edici fotoğraf linkini bul.
                                
                                Cevabı şu formatta ver:
                                [METIN_BASLA]
                                ...buraya metin gelecek...
                                [METIN_BITTI]
                                [RESIM_LINKI]
                                ...buraya sadece link gelecek...
                                [RESIM_BITTI]";

                var payload = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                string jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(ApiUrl + apiKey, content);
                    var resultJson = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        ViewBag.SonucMetni = $"BAĞLANTI HATASI: {resultJson}";
                    }
                    else
                    {
                        using (JsonDocument doc = JsonDocument.Parse(resultJson))
                        {
                            if (doc.RootElement.TryGetProperty("candidates", out JsonElement candidates) && candidates.GetArrayLength() > 0)
                            {
                                var textElement = candidates[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text");
                                var fullText = textElement.GetString();

                                if (fullText == null)
                                {
                                    ViewBag.SonucMetni = "AI'den geçerli yanıt alınamadı.";
                                    ViewBag.SonucVar = true;
                                    return View();
                                }

                                string metin = fullText; // Varsayılan olarak hepsini al
                                string resimLinki = "";

                                // Formatlı geldiyse parçala
                                if (fullText.Contains("[METIN_BASLA]"))
                                {
                                    int start = fullText.IndexOf("[METIN_BASLA]") + 13;
                                    int end = fullText.IndexOf("[METIN_BITTI]");
                                    if (end > start) metin = fullText.Substring(start, end - start).Trim();
                                }

                                if (fullText.Contains("[RESIM_LINKI]"))
                                {
                                    int start = fullText.IndexOf("[RESIM_LINKI]") + 13;
                                    int end = fullText.IndexOf("[RESIM_BITTI]");
                                    if (end > start) resimLinki = fullText.Substring(start, end - start).Trim();
                                }

                                // Resim linki bozuksa veya yoksa varsayılan koy
                                if (string.IsNullOrEmpty(resimLinki) || !resimLinki.StartsWith("http"))
                                {
                                    resimLinki = "https://images.unsplash.com/photo-1571019614242-c5c5dee9f50b?q=80&w=1470&auto=format&fit=crop";
                                }

                                // HTML formatına çevir
                                ViewBag.SonucMetni = metin?.Replace("**", "<b>").Replace("*", "<br>•") ?? "Yanıt işlenemedi.";
                                ViewBag.ResimUrl = resimLinki;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.SonucMetni = $"KRİTİK HATA: {ex.Message}";
            }

            ViewBag.SonucVar = true;
            return View();
        }
    }
}