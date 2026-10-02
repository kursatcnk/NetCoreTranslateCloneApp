# Translate Clone

Google Translate benzeri, çeviriyi OpenAI'ye yaptıran küçük bir ASP.NET Core MVC uygulaması. Metni yazıp hedef dili seçiyorsun, sonuç aynı sayfada çıkıyor.

Kaynak dili algılamayı ve çeviriyi modele bıraktım; uygulama sadece isteği hazırlayıp cevabı gösteriyor. Dil listesini API'den çekmek yerine sabit tuttum, her açılışta fazladan istek atmaya gerek yok.

## Özellikler

- 200'den fazla dil arasından hedef dil seçimi
- Kaynak dil otomatik algılanır
- `temperature = 0` ile aynı metin için tutarlı çeviri
- API anahtarı koddan ayrı, yapılandırmadan okunur

## Çalıştırma

Gerekenler: .NET 9 SDK ve bir OpenAI API anahtarı.

```bash
git clone https://github.com/kursatcnk/NetCoreTranslateCloneApp.git
cd NetCoreTranslateCloneApp/NetCoreTranslateCloneApp
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "sk-..."
dotnet run
```

Anahtarı `appsettings.json` içine yazmak yerine user-secrets kullanırsan yanlışlıkla commit'lenmez.

## Lisans

[MIT](LICENSE)
