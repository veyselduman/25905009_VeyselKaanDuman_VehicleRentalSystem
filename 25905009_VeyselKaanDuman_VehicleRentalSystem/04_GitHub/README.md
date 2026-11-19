# Duman Rent a Car — Araç Kiralama Yönetim Sistemi (Vehicle Rental Management System)

- **Ders Kodu ve Adı:** BGY203 / YBS223 — Nesne Tabanlı Programlama
- **Öğretim Elemanı:** Dr. Öğr. Üyesi Tohid YOUSEFİ
- **Dönem / Ödev Türü:** Güz Dönemi / Bireysel Proje Ödevi
- **Geliştirici Öğrenci:** Veysel Kaan Düman
- **Okul Numarası:** 25905009
- **Teslim Tarihi:** 20/11/2026
- **GitHub Repository:** `https://github.com/veyselduman/25905009_VeyselKaanDuman_VehicleRentalSystem`

---

## 1. Proje Hakkında ve Amacı
**Duman Rent a Car — Araç Kiralama Yönetim Sistemi**, C# programlama dili ve Windows Forms (WinForms) grafiksel kullanıcı arayüzü teknolojisi kullanılarak Nesne Tabanlı Programlama (OOP) ilkelerine (**Kapsülleme**, **Kalıtım**, **Çok Biçimlilik**, **Soyutlama**) tam uyumlu biçimde geliştirilmiş modüler bir masaüstü uygulamasıdır.

### Temel Özellikler
- **Müşteri Girişi ve Doğrulama (`LoginForm`):** Ad Soyad alanında harf harici karakter girişi (rakam ve semboller) olay tabanlı filtreleme (`KeyPress` ve `TextChanged`) ile engellenir.
- **Kiralama Danışmanı Seçimi:** Sistemde yetkili iki kiralama danışmanı bulunur (**Veysel Duman** ve **Kaan Duman**).
- **Ana Sayfa / Dinamik Dashboard (`MainDashboardForm`):** Toplam araç, müsait araç, kiradaki araç, toplam müşteri ve aktif kiralama sayıları bellek içi `List<T>` koleksiyonlarından dinamik olarak hesaplanır; canlı filo durum grafiği ve modül erişim menüsü sunulur.
- **2 Ayrı Araç Kategorisi ve 12 Model (Toplam 36 Araçlık Filo):**
  - **Sedan Kategorisi (`SedanVehicle : Vehicle`):** Fiat (*Fiat Egea*, *Fiat Linea*), Volkswagen (*Volkswagen Passat*, *Volkswagen Jetta*), Ford (*Ford Focus*, *Ford Mondeo*)
  - **SUV Kategorisi (`SuvVehicle : Vehicle`):** Volkswagen (*Volkswagen Tiguan*, *Volkswagen Touareg*), Ford (*Ford Kuga*, *Ford Puma*), Peugeot (*Peugeot 2008*, *Peugeot 3008*)
- **Renk Seçenekleri ve Orijinal Araç Görselleri:** Tüm modellerde **Siyah**, **Beyaz** ve **Gri** renk seçenekleri ile orijinal araç görseli önizlemesi sunulur.
- **Esnek Kiralama Süresi ve Çok Biçimli Ücretlendirme (`RentalWizardForm`):** **1 Gün**, **1 Hafta (7 Gün)** veya **1 Ay (30 Gün)** paketleri ile senkronize tarih seçimi, geçersiz tarih doğrulaması ve `SedanVehicle` / `SuvVehicle` sınıflarında `CalculateRentalPrice(int days)` çok biçimli (polymorphic) ücret hesabı yapılır.
- **Araç Yönetimi (`VehicleManagementForm`):** Yeni araç ekleme, listeleme, güncelleme, silme, standart Türkiye plakası doğrulaması (`38 DUM 101`), plaka tekrarı engeli ve kiralama kaydı bulunan aracın silinmesini engelleme.
- **Müşteri Yönetimi (`CustomerManagementForm`):** Müşteri ekleme, listeleme, güncelleme, silme, zorunlu alan doğrulaması (`05XX XXX XX XX` telefon ve `B-381045` ehliyet formatı) ve aktif kiralaması bulunan müşterinin silinmesini engelleme.
- **Araç İade (`ReturnVehicleForm`):** Aktif kiralamaların listelenmesi, seçilen kiralamanın iade alınarak kiralama durumunun **Tamamlandı**, araç durumunun ise yeniden **Müsait** olarak güncellenmesi.

---

## 2. OOP İlkeleri ve C# Yapılarının Uygulamadaki Karşılıkları

| OOP İlkesi / Yapı | Uygulandığı Sınıf ve Metotlar | Açıklama |
| :--- | :--- | :--- |
| **Kapsülleme (Encapsulation)** | `Vehicle`, `SedanVehicle`, `SuvVehicle`, `Customer`, `Rental`, `RentalConsultant` | Tüm alanlar `private` tanımlanmış; erişimler doğrulamalı (`DailyRate > 0`, geçerli plaka, ad/soyad, telefon, tarih kontrolü) `public` property'ler üzerinden sağlanmıştır. |
| **Kalıtım (Inheritance)** | `abstract class Vehicle` -> `SedanVehicle`, `SuvVehicle` | Ortak araç özellikleri temel `Vehicle` sınıfında toplanmış; `SedanVehicle` (`TrunkCapacityLiters`) ve `SuvVehicle` (`HasAllWheelDrive`, `SuvProtectionFeePerDay`) alt sınıfları türetilmiştir. |
| **Çok Biçimlilik (Polymorphism)** | `CalculateRentalPrice(int days)`, `GetPriceBreakdown(int days)`, `GetVehicleDetails()` | `Vehicle` sınıfındaki soyut ve sanal metotlar `SedanVehicle` ve `SuvVehicle` alt sınıflarında `override` edilerek kategoriye özgü indirim ve güvence hesaplamaları uygulanmıştır. |
| **Soyutlama (Abstraction)** | `interface IRentable`, `abstract class Vehicle` | Kiralanabilir nesneler için `IRentable` sözleşmesi ve doğrudan örneklendirilemeyen soyut `Vehicle` sınıfı tanımlanmıştır. |
| **Collections & Generics** | `Repository<T>` (`List<T>`) | Araç, müşteri ve kiralama kayıtları `List<T>` tabanlı generic `Repository<T>` sınıfı ile bellekte yönetilmektedir. |
| **Olaylar ve Delegeler** | `RentalEventHandler`, `DataChanged` ve WinForms Olayları | `RentalManager.DataChanged` olayı ile veri değişimlerinde Dashboard anlık güncellenir; `Click`, `SelectedIndexChanged`, `CheckedChanged`, `ValueChanged`, `KeyPress`, `TextChanged` olayları aktif kullanılmıştır. |
| **Hata Yönetimi** | `try-catch`, `ArgumentException`, `InvalidOperationException` | Tüm kullanıcı girdileri ve iş kuralları `try-catch` bloklarıyla yakalanarak kullanıcı dostu uyarı mesajlarıyla yönetilmiştir. |

---

## 3. Kullanılan Teknolojiler ve Geliştirme Ortamı
- **Programlama Dili:** C# (.NET Framework 4.x / Windows Forms)
- **Grafik Arayüz (GUI):** Windows Forms (WinForms) ve `System.Drawing` (GDI+)
- **Geliştirme Ortamı:** Visual Studio (`VehicleRentalSystem.sln` / `VehicleRentalSystem.csproj`)
- **Veri Yapıları:** Generic `Repository<T>` ve `List<T>` bellek içi koleksiyonları
- **Olaylar ve Delegeler:** Özel `RentalEventHandler` delegesi ve `DataChanged` olayı

---

## 4. Klasör ve Dosya Yapısı
```text
25905009_VeyselKaanDuman_VehicleRentalSystem/
├── 01_SourceCode/
│   ├── VehicleRentalSystem.sln
│   └── VehicleRentalSystem/
│       ├── Assets/
│       │   └── Vehicles/                 (Orijinal araç görselleri)
│       ├── Interfaces/
│       │   └── IRentable.cs              (Kiralanabilir nesne sözleşmesi)
│       ├── Models/
│       │   ├── Enums.cs                  (VehicleStatus, VehicleColor, RentalStatus)
│       │   ├── Vehicle.cs                (Abstract temel sınıf)
│       │   ├── SedanVehicle.cs           (Vehicle alt sınıfı - Sedan)
│       │   ├── SuvVehicle.cs             (Vehicle alt sınıfı - SUV)
│       │   ├── Customer.cs               (Müşteri sınıfı)
│       │   ├── RentalConsultant.cs       (Kiralama danışmanı sınıfı)
│       │   └── Rental.cs                 (Kiralama işlem sınıfı)
│       ├── Services/
│       │   ├── Repository.cs             (Generic List<T> veri deposu)
│       │   ├── RentalManager.cs          (İş mantığı ve olay yönetimi)
│       │   ├── InputValidator.cs         (Girdi doğrulama servisi)
│       │   └── CarGraphicsRenderer.cs    (Araç ve marka görsel işleme servisi)
│       ├── Forms/
│       │   ├── LoginForm.cs              (Müşteri giriş ve danışman seçim ekranı)
│       │   ├── MainDashboardForm.cs      (Ana Sayfa / Dinamik Dashboard)
│       │   ├── RentalWizardForm.cs       (4 adımlı araç kiralama ve ödeme sihirbazı)
│       │   ├── VehicleManagementForm.cs  (Araç yönetimi CRUD ekranı)
│       │   ├── CustomerManagementForm.cs (Müşteri yönetimi CRUD ekranı)
│       │   └── ReturnVehicleForm.cs      (Araç iade ekranı)
│       ├── Program.cs
│       └── VehicleRentalSystem.csproj
├── 02_Application/
│   └── Release/
│       ├── VehicleRentalSystem.exe       (Doğrudan çalıştırılabilir uygulama)
│       └── Assets/Vehicles/              (Araç görselleri)
├── 03_Report/
│   ├── Proje_Raporu.pdf                  (Proje raporu - PDF formatı)
│   ├── Proje_Raporu.docx                 (Proje raporu - DOCX formatı)
│   └── Screenshots/                      (Uygulama ekran görüntüleri ve UML diyagramı)
├── 04_GitHub/
│   ├── README.md
│   └── GitHub_Link.txt
└── README.md
```

---

## 5. Projeyi Çalıştırma Adımları
1. **Doğrudan Çalıştırma (Kurulum Gerektirmez):**
   - `02_Application/Release/VehicleRentalSystem.exe` dosyasına çift tıklayarak uygulamayı doğrudan başlatabilirsiniz.
2. **Visual Studio ile Kaynak Koddan Derleme ve Çalıştırma:**
   - `01_SourceCode/VehicleRentalSystem.sln` dosyasını **Visual Studio** ile açın.
   - Üst menüden yapılandırmayı **Release** (veya **Debug**) olarak seçip **Build Solution** (`Ctrl + Shift + B`) ile derleyin.
   - **Start** (`F5`) tuşuyla uygulamayı çalıştırın.

---

## 6. Uygulama Ekran Görüntüleri
Uygulamanın çalışmasına ait tüm ekran görüntüleri ve adım adım açıklamaları `03_Report/Proje_Raporu.pdf`, `03_Report/Proje_Raporu.docx` raporlarında ve `03_Report/Screenshots/` klasöründe yer almaktadır:
- `03_Report/Screenshots/00_uml_sinif_diyagrami.png` — UML Sınıf Mimarisi Diyagramı
- `03_Report/Screenshots/01_giris_ekrani.png` — Müşteri Girişi ve Danışman Seçimi
- `03_Report/Screenshots/02_ana_sayfa_dashboard.png` — Ana Sayfa (Dinamik Dashboard)
- `03_Report/Screenshots/03_kategori_secimi.png` — Araç Kategorisi Seçimi (Sedan / SUV)
- `03_Report/Screenshots/04_marka_model_renk_sedan.png` — Marka, Model ve Renk Seçimi (Sedan)
- `03_Report/Screenshots/05_marka_model_renk_suv.png` — Marka, Model ve Renk Seçimi (SUV)
- `03_Report/Screenshots/06_sure_ve_ucret_hesaplama.png` — Süre Paketi, Tarih Doğrulama ve Çok Biçimli Ücret Hesaplama
- `03_Report/Screenshots/07_odeme_onayi.png` — Ödeme Onayı ve Kiralama Özeti
- `03_Report/Screenshots/08_odeme_tamamlandi_makbuz.png` — Kiralama Makbuzu ve Güncellenen Dashboard
- `03_Report/Screenshots/09_arac_yonetimi.png` — Araç Yönetimi (CRUD ve Plaka Tekrarı Koruması)
- `03_Report/Screenshots/10_musteri_yonetimi.png` — Müşteri Yönetimi (CRUD ve Silme Koruması)
- `03_Report/Screenshots/11_arac_iade.png` — Araç İade İşlemleri
