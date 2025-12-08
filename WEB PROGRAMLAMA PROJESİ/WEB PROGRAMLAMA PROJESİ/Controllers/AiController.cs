using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace FitnessApp.Controllers
{
    [Authorize]
    public class AiController : Controller
    {
    
        private const string ApiKey = "AIzaSyBkyaik3cQujkvCbuyjVryPiWkSQeB0iaI";

        // GÜNCELLEME: Arkadaşının önerdiği en yeni 'gemini-2.5-flash' modelini kullanıyoruz.
        // Bu model Aralık 2025 itibarıyla en kararlı çalışan versiyondur.
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

            try
            {
                string prompt = @$"Ben bir spor salonu üyesiyim. 
                                Boy: {boy} cm, Kilo: {kilo} kg, Cinsiyet: {cinsiyet}, Hedef: {hedef}.
                                Bana 3 maddede kısa özet ver: 1. VKİ Durumu, 2. Egzersiz Önerisi, 3. Beslenme Önerisi.
                                Samimi bir spor hocası gibi konuş, emojiler kullan.";

                var payload = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = prompt } } }
                    }
                };

                string jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(ApiUrl + ApiKey, content);
                    var resultJson = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        ViewBag.Sonuc = $"BAĞLANTI HATASI: {resultJson}";
                    }
                    else
                    {
                        using (JsonDocument doc = JsonDocument.Parse(resultJson))
                        {
                            if (doc.RootElement.TryGetProperty("candidates", out JsonElement candidates) && candidates.GetArrayLength() > 0)
                            {
                                var text = candidates[0]
                                    .GetProperty("content")
                                    .GetProperty("parts")[0]
                                    .GetProperty("text")
                                    .GetString();

                                text = text.Replace("**", "<b>").Replace("*", "<br>•");
                                ViewBag.Sonuc = text;
                            }
                            else
                            {
                                ViewBag.Sonuc = "Yapay zeka cevap döndüremedi.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Sonuc = $"KRİTİK HATA: {ex.Message}";
            }

            ViewBag.SonucVar = true;
            return View();
        }
    }
}