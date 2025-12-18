Spor Salonu (Fitness Center) Yönetim ve Randevu Sistemi
Bu proje, 2025-2026 Güz Dönemi **Web Programlama** dersi kapsamında geliştirilmiş, ASP.NET Core MVC tabanlı kapsamlı bir spor salonu yönetim sistemidir.

Proje Hakkında
Projenin temel amacı, spor salonlarının sunduğu hizmetleri, antrenörlerin uzmanlık alanlarını ve üyelerin randevu süreçlerini dijitalleştirmektir. 
Ayrıca sistem, yapay zeka entegrasyonu sayesinde üyelere kişiselleştirilmiş egzersiz ve diyet önerileri sunmaktadır.

Temel Özellikler
Admin:
     Salon çalışma saatleri, hizmet türleri (Fitness, Yoga, Pilates vb.) ve ücret tanımlamaları.
     Antrenör (Eğitmen) ekleme, uzmanlık alanı ve çalışma saati belirleme.
     Rol bazlı yetkilendirme (Admin ve Üye).
Antrenör Yönetimi:
     Antrenörlerin uzmanlık alanlarına göre filtrelenmesi.
     Müsaitlik takvimi yönetimi.
Üye ve Randevu Sistemi:
     Üyelerin uygun antrenör ve hizmete göre randevu alabilmesi.
     Dolu saatler için çakışma kontrolü ve uyarı sistemi.
     Geçmiş ve gelecek randevuların görüntülenmesi.
Yapay Zeka Destekli Asistan:
     Kullanıcıların vücut tipi, boy, kilo gibi verilerine dayalı olarak kişiye özel egzersiz/diyet programı oluşturulması.
REST API Entegrasyonu:
     Antrenör listeleme, randevu sorgulama gibi işlemler için LINQ destekli API uç noktaları.

Kullanılan Teknolojiler
Bu projede aşağıdaki teknolojiler ve araçlar kullanılmıştır:

Backend: ASP.NET Core MVC, C#
Veritabanı: SQL Server / PostgreSQL (Entity Framework Core - Code First)
Frontend:HTML5, CSS3, JavaScript, jQuery, Bootstrap 5
API & AI: RESTful API, Yapay Zeka Entegrasyonu (Gemini)

Kurulum ve Çalıştırma

Projeyi yerel ortamınızda çalıştırmak için aşağıdaki adımları izleyin:

1. Projeyi Klonlayın:
   bash
    git clone [https://github.com/kullaniciadi/proje-adi.git](https://github.com/kullaniciadi/proje-adi.git)
    cd proje-adi
   

2. Veritabanı Ayarları:
    appsettings.json dosyasındaki ConnectionStrings alanını kendi veritabanı sunucunuza göre düzenleyin.

3.  Veritabanını Oluşturun (Migration):
    Package Manager Console veya Terminal üzerinden aşağıdaki komutu çalıştırarak veritabanını ve tabloları oluşturun:
   bash
    dotnet ef database update


4.  API Anahtarları (Opsiyonel):
    Yapay zeka özelliklerinin çalışması için ilgili API anahtarını appsettings.json dosyasına ekleyin.

5.  Projeyi Başlatın:
    bash
    dotnet run
  

Giriş Bilgileri (Test İçin)

Proje gereksinimleri doğrultusunda tanımlanan varsayılan yönetici hesabı:

Admin Email: ogrencinumarasi@sakarya.edu.tr 
Admin Şifre: sau
Üye Girişi: Kayıt sayfasından yeni üye oluşturabilirsiniz.

Geliştirici: Sümeyye ÇETİNKAYA
Ders: Web Programlama - 2025/2026 Güz
