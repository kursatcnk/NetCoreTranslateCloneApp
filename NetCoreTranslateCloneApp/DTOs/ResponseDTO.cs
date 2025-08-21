namespace NetCoreTranslateCloneApp.DTOs
{
    // Bu sınıfı, API'den dönen tüm çeviri yanıtlarını tek bir nesnede
    // toplamak için oluşturdum. Böylece View'a veya başka katmanlara
    // veri taşırken düzenli ve tip güvenli bir yapı kullanabiliyorum.
    public class ResponseDTO
    {
        public string ID { get; set; }      // API yanıtının benzersiz kimliği
        public string Object { get; set; }  // Dönen nesnenin tipi (örn. "text_completion")
        public string Model { get; set; }   // Hangi modelin çeviri yaptığını saklamak için
        public int Created { get; set; }     // Yanıtın oluşturulma zamanını timestamp olarak tutmak için
        public List<Choice> Choices { get; set; } // Çeviri seçeneklerini saklamak için bir liste
    }

    // Bu sınıfı, ResponseDTO içindeki Choices listesinin her bir elemanını
    // temsil etmesi için oluşturdum. Her Choice bir çeviri sonucu veya mesaj olabilir.
    public class Choice
    {
        public int Index { get; set; }          // Seçeneğin sırası (0,1,2 gibi)
        public Message Message { get; set; }    // Asıl mesaj içeriğini tutan nesne
        public string FinishReason { get; set; } // API'nin işlemi neden sonlandırdığını anlamak için
    }

    // Bu sınıfı, Choice içindeki mesaj içeriğini saklamak için oluşturdum.
    // Böylece mesajın rolünü ve metin içeriğini ayrı ayrı yönetebiliyorum.
    public class Message
    {
        public string Role { get; set; }    // Mesajın rolü (örn. "user" veya "assistant")
        public string Content { get; set; } // Mesajın asıl çeviri metni veya içerik
    }
}
