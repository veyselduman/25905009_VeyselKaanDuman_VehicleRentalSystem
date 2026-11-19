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

## 6. Mimari Diyagram ve Uygulama Ekran Görüntüleri
Uygulamanın sınıf mimarisi (UML) ve çalışmasına ait tüm ekran görüntüleri (`03_Report/Proje_Raporu.pdf` / `03_Report/Proje_Raporu.docx` raporundaki **Şekil 1 – Şekil 12**) aşağıda adım adım açıklamalarıyla yer almaktadır:

### Şekil 1: Duman Rent a Car UML Sınıf Diyagramı
> `IRentable` arayüzü, `abstract Vehicle` temel sınıfı, `SedanVehicle` ve `SuvVehicle` alt sınıfları, `Customer`, `RentalConsultant`, `Rental`, generic `Repository<T>` ve `RentalManager` servis sınıfları arasındaki kalıtım ve ilişki yapısı.

![Şekil 1 - Duman Rent a Car UML Sınıf Diyagramı](03_Report/Screenshots/00_uml_sinif_diyagrami.png)

---

### Şekil 2: Başlangıç ve Müşteri Giriş Ekranı (1. – 3. Adım)
> Sol bölümde Sedan ve SUV araç görselleri ile anlaşmalı markalar (Fiat, Volkswagen, Ford, Peugeot) yer alır. **1. Adımda** müşteri Ad Soyad alanına giriş yapar (harf harici karakterler engellenir). **2. Adımda** yetkili danışmanlardan Veysel Duman veya Kaan Duman seçilir. **3. Adımda** *Sisteme Giriş Yap* butonuna tıklanır.

![Şekil 2 - Başlangıç ve Müşteri Giriş Ekranı](03_Report/Screenshots/01_giris_ekrani.png)

---

### Şekil 3: Ana Sayfa / Dashboard Ekranı (4. – 5. Adım)
> **4. Adımda** koleksiyonlardan anlık hesaplanan Toplam Araç (36), Müsait Araç (36), Kiradaki Araç (0), Toplam Müşteri (4) ve Aktif Kiralama (0) kartları ile Canlı Filo Durum Grafiği görüntülenir. **5. Adımda** sol menüden *Araç Kiralama Sihirbazı* açılır.

![Şekil 3 - Ana Sayfa Dashboard Ekranı](03_Report/Screenshots/02_ana_sayfa_dashboard.png)

---

### Şekil 4: Araç Türü / Kategori Seçimi — Sedan ve SUV (6. – 7. Adım)
> **6. Adımda** müşteri araç görsellerini inceleyerek Sedan (Fiat Egea/Linea, VW Passat/Jetta, Ford Focus/Mondeo) veya SUV (VW Tiguan/Touareg, Ford Kuga/Puma, Peugeot 2008/3008) kategorisinden birini seçer. **7. Adımda** *Sonraki Adım* ile ilerlenir.

![Şekil 4 - Araç Türü Kategori Seçimi](03_Report/Screenshots/03_kategori_secimi.png)

---

### Şekil 5: Marka, Model ve Renk Seçimi — Sedan Örneği (8. – 11. Adım)
> **8. Adımda** Marka (Volkswagen), **9. Adımda** Model (Volkswagen Passat), **10. Adımda** Siyah, Beyaz veya Gri renklerinden biri seçilir. Sağ tarafta seçilen aracın görseli, plaka bilgisi (`38 DUM 107`), günlük ücreti ve *MÜSAİT* durumu görüntülenir. **11. Adımda** sonraki aşamaya geçilir.

![Şekil 5 - Marka Model ve Renk Seçimi Sedan](03_Report/Screenshots/04_marka_model_renk_sedan.png)

---

### Şekil 6: Marka, Model ve Renk Seçimi — SUV Örneği (Peugeot 3008 / Gri)
> SUV kategorisi seçildiğinde marka listesi Volkswagen, Ford ve Peugeot olarak filtrelenir. Peugeot 3008 ve Gri renk seçildiğinde aracın görseli, plaka bilgisi (`38 DUM 136`) ve günlük ücret + SUV güvence tutarı anlık olarak güncellenir.

![Şekil 6 - Marka Model ve Renk Seçimi SUV](03_Report/Screenshots/05_marka_model_renk_suv.png)

---

### Şekil 7: Kiralama Süresi (1 Gün / 1 Hafta / 1 Ay) ve Ücret Hesabı (12. – 14. Adım)
> **12. Adımda** müşteri 1 Gün, 1 Hafta (7 Gün) veya 1 Ay (30 Gün) paketlerinden birini seçer (örnekte 1 Hafta). **13. Adımda** başlangıç (`16.11.2026`) ve bitiş (`23.11.2026`) tarihleri doğrulanır. **14. Adımda** çok biçimli `CalculateRentalPrice(7)` metoduyla hesaplanan toplam tutar (`15.120 TL`) kontrol edilip ödeme adımına geçilir.

![Şekil 7 - Kiralama Süresi ve Ücret Hesabı](03_Report/Screenshots/06_sure_ve_ucret_hesaplama.png)

---

### Şekil 8: Ödeme Onayı ve Sözleşme Özeti Ekranı (15. – 17. Adım)
> **15. Adımda** müşteri, danışman (Veysel Duman), araç (Volkswagen Passat - Siyah - `38 DUM 107`), süre ve toplam tutar (`15.120 TL`) incelenir. **16. Adımda** ödeme yöntemi (Kredi Kartı, Banka Kartı, Havale/EFT, Nakit) ve ödeme bilgileri doğrulanır. **17. Adımda** *Ödemeyi Onayla ve Tamamla* butonuna tıklanır.

![Şekil 8 - Ödeme Onayı ve Sözleşme Özeti](03_Report/Screenshots/07_odeme_onayi.png)

---

### Şekil 9: Ödeme Onay Makbuzu ve Güncellenen Dashboard (18. – 19. Adım)
> **18. Adımda** `#1001` numaralı kiralama ve ödeme onay makbuzu ekrana yansır. **19. Adımda** `DataChanged` olayı tetiklenerek Dashboard üzerindeki Müsait Araç sayacı 35'e, Kiradaki Araç ve Aktif Kiralama sayaçları 1'e güncellenir.

![Şekil 9 - Ödeme Onay Makbuzu ve Güncellenen Dashboard](03_Report/Screenshots/08_odeme_tamamlandi_makbuz.png)

---

### Şekil 10: Araç Yönetimi Modülü (20. – 21. Adım)
> **20. Adımda** sol panel üzerinden yeni araç ekleme, seçili aracı güncelleme veya silme işlemleri yürütülür; plaka tekrarı ve kiralama kayıtlı araç silme engellenir. **21. Adımda** sağ tabloda kiralanan Volkswagen Passat (`38 DUM 107`) aracının durumu *Kirada* olarak takip edilir.

![Şekil 10 - Araç Yönetimi Modülü](03_Report/Screenshots/09_arac_yonetimi.png)

---

### Şekil 11: Müşteri Yönetimi Modülü (22. – 23. Adım)
> **22. Adımda** müşteri ekleme, güncelleme ve silme işlemleri yapılır; boş alanlar ve aktif kiralamalı müşteri silme engellenir. **23. Adımda** sisteme giriş yapan Veysel Kaan Düman dâhil tüm kayıtlı müşteriler tabloda listelenir.

![Şekil 11 - Müşteri Yönetimi Modülü](03_Report/Screenshots/10_musteri_yonetimi.png)

---

### Şekil 12: Araç İade Modülü (24. – 25. Adım)
> **24. Adımda** aktif kiralamalar listesinden `#1001` numaralı kiralama kaydı seçilir. **25. Adımda** *Seçili Aracı İade Al (Tamamla)* butonuna tıklanır; kiralama durumu *Tamamlandı*, `38 DUM 107` numaralı aracın durumu ise yeniden *Müsait* olur.

![Şekil 12 - Araç İade Modülü](03_Report/Screenshots/11_arac_iade.png)
