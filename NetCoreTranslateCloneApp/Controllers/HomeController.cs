using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NetCoreTranslateCloneApp.DTOs;
using NetCoreTranslateCloneApp.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace TranslateGPT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        // Mevcut dilleri burdan girdim, API ile direk çevrimiçi alabilirdim ama stringler üzerinde herhangi bir işlem yapmayacağım için maliyeti kıstım.
        private readonly List<string> mostUsedLanguages = new List<string>()
        {
            "Abkhaz","Afar","Afrikaans","Akan","Albanian","Amharic","Arabic","Aragonese","Armenian","Assamese",
            "Avaric","Aymara","Azerbaijani","Bambara","Bashkir","Basque","Belarusian","Bengali","Bihari","Bislama",
            "Bosnian","Breton","Bulgarian","Burmese","Catalan","Chamorro","Chechen","Chichewa","Chinese","Chuvash",
            "Cornish","Corsican","Cree","Croatian","Czech","Danish","Divehi","Dutch","Dzongkha","English",
            "Esperanto","Estonian","Ewe","Faroese","Fijian","Finnish","French","Fula","Galician","Georgian",
            "German","Gikuyu","Greek","Guarani","Gujarati","Haitian Creole","Hausa","Hebrew","Herero","Hindi",
            "Hiri Motu","Hungarian","Icelandic","Ido","Igbo","Indonesian","Interlingua","Interlingue","Inuktitut","Inupiak",
            "Irish","Italian","Japanese","Javanese","Kannada","Kanuri","Kashmiri","Kazakh","Khmer","Kikuyu",
            "Kinyarwanda","Kirghiz","Kirundi","Komi","Kongo","Korean","Kurdish","Kwanyama","Lao","Latin",
            "Latvian","Letzeburgesch","Lingala","Lithuanian","Luba-Katanga","Luxembourgish","Macedonian","Malagasy","Malay","Malayalam",
            "Maltese","Manx","Maori","Marathi","Marshallese","Mongolian","Nauru","Navajo","Ndonga","Nepali",
            "North Ndebele","Northern Sami","Norwegian","Norwegian Bokmål","Norwegian Nynorsk","Nuosu","Occitan","Ojibwa","Oriya","Oromo",
            "Ossetian","Pali","Pashto","Persian","Polish","Portuguese","Punjabi","Quechua","Romanian","Romansh",
            "Russian","Samoan","Sango","Sanskrit","Sardinian","Scottish Gaelic","Serbian","Shona","Sindhi","Sinhala",
            "Slovak","Slovenian","Somali","Sotho","Southern Ndebele","Spanish","Sundanese","Swahili","Swati","Swedish",
            "Tagalog","Tahitian","Tajik","Tamil","Tatar","Telugu","Thai","Tibetan","Tigrinya","Tonga",
            "Tsonga","Tswana","Turkish","Turkmen","Tuvalu","Uighur","Ukrainian","Urdu","Uzbek","Venda",
            "Vietnamese","Volapük","Walloon","Welsh","Wolof","Western Frisian","Xhosa","Yiddish","Yoruba","Zulu",
            "Balochi","Chuvash","Tuvan","Sami","Kalaallisut","Quechua","Guarani","Mapudungun","Nahuatl","Mayan",
            "Inupiak","Cherokee","Greenlandic","Maldivian","Tok Pisin","Hmong","Cham","Dzongkha","Fijian Hindi","Tibetan (Simplified)",
            "Tibetan (Traditional)","Shan","Hmong Daw","Kpelle","Lingala-Kongo","Fula-Pulaar","Haitian","Sranan Tongo"
        };

        // Constructor'da bağımlılıkları alıyorum: Logger, Config ve HttpClient
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger; // Logger ayarlıyorum
            _configuration = configuration; // Config üzerinden API key gibi ayarları alıyorum
            _httpClient = httpClient; // API çağrıları için HttpClient kullanıyorum
        }

        // Ana sayfa için GET metodu
        public IActionResult Index()
        {
            // Dropdown listesi için dilleri ViewBag'e atıyorum
            ViewBag.Languages = new SelectList(mostUsedLanguages);
            // Index view'ini döndürüyorum
            return View();
        }

        // Çeviri işlemi POST metodu
        [HttpPost]
        public async Task<IActionResult> Translate(string query, string selectedLanguage)
        {
            // OpenAI API key'ini alıyorum
            var openAPIKey = _configuration["OpenAI:ApiKey"];

            // HttpClient header'larını temizliyorum ve Authorization ekliyorum
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAPIKey}");

            // API'ye göndereceğim payload'u hazırlıyorum
            var payload = new
            {
                model = "gpt-4", // Kullanacağım model
                messages = new object[]
                {
                    new { role = "system", content = $"Translate to {selectedLanguage}" }, // Sisteme dil talimatı
                    new { role = "user", content = query } // Kullanıcının girdiği metin
                },
                temperature = 0, // Deterministik sonuç için
                max_tokens = 256 // Maks token sayısı
            };

            // Payload'u JSON string'e çeviriyorum
            string jsonPayload = JsonConvert.SerializeObject(payload);

            // HttpContent objesi oluşturuyorum
            HttpContent httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // POST isteğini API'ye gönderiyorum
            var responseMessage = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", httpContent);

            // Dönen cevabı string olarak alıyorum
            var responseMessageJson = await responseMessage.Content.ReadAsStringAsync();

            // JSON'u DTO'ya çeviriyorum
            var response = JsonConvert.DeserializeObject<ResponseDTO>(responseMessageJson);

            // Çeviri sonucunu ViewBag'e koyuyorum, eğer yoksa default mesaj
            ViewBag.Result = response?.Choices?[0]?.Message?.Content ?? "Çeviri alınamadı.";

            // Dropdown'da seçili dili korumak için tekrar ViewBag.Languages'ı ayarlıyorum
            ViewBag.Languages = new SelectList(mostUsedLanguages);

            // Tekrar Index view'ini döndürüyorum
            return View("Index");
        }

        // Hata sayfası için metod
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Hata view'ine RequestId ile yönlendiriyorum
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
