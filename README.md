# 🏋️‍♂️ Fitness Salonu Yönetim ve Randevu Sistemi (FitnessApp)

Bu proje, **Web Programlama** dersi kapsamında geliştirilmiş; spor salonu üyelerinin randevu alabildiği, yöneticilerin salonu yönetebildiği ve yapay zeka destekli antrenman tavsiyelerinin sunulduğu kapsamlı bir **ASP.NET Core MVC** web uygulamasıdır.

## 🚀 Proje Hakkında
**FitnessApp**, klasik spor salonu yönetimini dijitalleştirmeyi hedefler. Kullanıcılar (üyeler) sisteme kayıt olup giriş yaptıktan sonra, diledikleri antrenörden ve hizmet türünden randevu alabilirler. Sistem, karmaşık iş kuralları (Business Logic) sayesinde hatalı veya çakışan randevuları engeller. Ayrıca entegre edilen **Google Gemini AI** sayesinde üyelere kişisel antrenman programı önerileri sunar.

## 🛠️ Kullanılan Teknolojiler
* **Platform:** .NET 8.0
* **Mimari:** ASP.NET Core MVC
* **Veritabanı:** Microsoft SQL Server (Entity Framework Core / Code-First)
* **Frontend:** HTML5, CSS3, Bootstrap 5, JavaScript (SweetAlert2)
* **AI Servisi:** Google Gemini 2.5 Flash API
* **Güvenlik:** ASP.NET Core Identity & User Secrets

## ✨ Temel Özellikler

### 1. Kullanıcı ve Yetki Yönetimi
* Güvenli Üyelik Sistemi (Kayıt Ol, Giriş Yap, Çıkış Yap).
* Rol Yönetimi (Admin ve Member rolleri).
* Admin paneli erişim kısıtlamaları.

### 2. Akıllı Randevu Sistemi
Sistem, randevu alınırken şu kontrolleri **otomatik** yapar:
* ✅ **Uzmanlık Kontrolü:** Seçilen hoca, seçilen dersi (Örn: Pilates) veriyor mu?
* ✅ **Mesai Saati Kontrolü:** Randevu, hocanın çalışma saatleri (Örn: 09:00 - 17:00) içinde mi?
* ✅ **Çakışma (Conflict) Kontrolü:** Hocanın o saatte başka bir öğrencisi var mı?
* ✅ **Geçmiş Tarih Kontrolü:** Geçmişe dönük randevu engelleme.

### 3. Yönetim Paneli (CRUD İşlemleri)
* **Eğitmen Yönetimi:** Yeni antrenör ekleme, uzmanlık alanı belirleme, çalışma saatlerini düzenleme, fotoğraf yükleme.
* **Hizmet Yönetimi:** Yeni ders türleri (Fitness, Yoga vb.) ve fiyatlandırma ekleme.

### 4. Yapay Zeka (AI) Modülü 🤖
* Google Gemini API entegrasyonu.
* Kullanıcının Boy, Kilo ve Hedef (Kilo verme/Kas yapma) bilgilerine göre;
    * 3 maddelik kişisel tavsiye metni.
    * Motive edici görsel önerisi.

### 5. Arayüz (UI/UX)
* **Premium Dark Gold Tema:** Siyah ve Altın sarısı tonlarında modern tasarım.
* Mobil uyumlu (Responsive) yapı.
* Kullanıcı dostu hata ve başarı bildirimleri (SweetAlert).


---
**Geliştirici:** [ertuğrul kabaoğlu]
**Ders:** Web Programlama
