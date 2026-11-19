using System;
using System.Collections.Generic;
using System.Linq;
using VehicleRentalSystem.Models;

namespace VehicleRentalSystem.Services
{
    /// <summary>
    /// Uygulama olay verilerini taşıyan EventArgs alt sınıfı.
    /// </summary>
    public class RentalEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public DateTime Timestamp { get; private set; }

        public RentalEventArgs(string message)
        {
            Message = message;
            Timestamp = DateTime.Now;
        }
    }

    /// <summary>
    /// Özel delege tanımı (Olaylar ve Delegeler gereksinimi).
    /// </summary>
    public delegate void RentalEventHandler(object sender, RentalEventArgs e);

    /// <summary>
    /// Duman Rent a Car iş mantığını, araç/müşteri/kiralama koleksiyonlarını ve doğrulamalarını yönetir.
    /// </summary>
    public class RentalManager
    {
        public const string CompanyName = "Duman Rent a Car";
        public const string StudentName = "Veysel Kaan Düman";
        public const string StudentNumber = "25905009";

        private readonly Repository<Vehicle> _vehicles = new Repository<Vehicle>();
        private readonly Repository<Customer> _customers = new Repository<Customer>();
        private readonly Repository<Rental> _rentals = new Repository<Rental>();
        private readonly List<RentalConsultant> _consultants = new List<RentalConsultant>();

        private int _nextVehicleId = 1;
        private int _nextCustomerId = 1;
        private int _nextRentalId = 1001;

        public Customer ActiveCustomer { get; set; }
        public RentalConsultant ActiveConsultant { get; set; }

        /// <summary>
        /// Araç, müşteri veya kiralama verisi değiştiğinde tetiklenen olay.
        /// </summary>
        public event RentalEventHandler DataChanged;

        public RentalManager()
        {
            SeedConsultants();
            SeedVehicles();
            SeedCustomers();
        }

        private void OnDataChanged(string message)
        {
            RentalEventHandler handler = DataChanged;
            if (handler != null)
            {
                handler(this, new RentalEventArgs(message));
            }
        }

        private void SeedConsultants()
        {
            _consultants.Add(new RentalConsultant(1, "Veysel Duman", "Kiralama Danışmanı"));
            _consultants.Add(new RentalConsultant(2, "Kaan Duman", "Kiralama Danışmanı"));
            ActiveConsultant = _consultants[0];
        }

        private class SedanSeedSpec
        {
            public string Brand;
            public string Model;
            public int Year;
            public decimal Rate;
            public int Trunk;

            public SedanSeedSpec(string brand, string model, int year, decimal rate, int trunk)
            {
                Brand = brand;
                Model = model;
                Year = year;
                Rate = rate;
                Trunk = trunk;
            }
        }

        private class SuvSeedSpec
        {
            public string Brand;
            public string Model;
            public int Year;
            public decimal Rate;
            public bool Awd;

            public SuvSeedSpec(string brand, string model, int year, decimal rate, bool awd)
            {
                Brand = brand;
                Model = model;
                Year = year;
                Rate = rate;
                Awd = awd;
            }
        }

        /// <summary>
        /// İstenen tüm Sedan ve SUV marka/modellerini Siyah, Beyaz ve Gri renk seçenekleriyle filoya ekler.
        /// </summary>
        private void SeedVehicles()
        {
            VehicleColor[] colors = { VehicleColor.Siyah, VehicleColor.Beyaz, VehicleColor.Gri };

            // Sedan Modelleri: Fiat (Egea, Linea), Volkswagen (Passat, Jetta), Ford (Focus, Mondeo)
            SedanSeedSpec[] sedanCatalog = new SedanSeedSpec[]
            {
                new SedanSeedSpec("Fiat", "Fiat Egea", 2024, 1350m, 520),
                new SedanSeedSpec("Fiat", "Fiat Linea", 2022, 1150m, 500),
                new SedanSeedSpec("Volkswagen", "Volkswagen Passat", 2024, 2400m, 586),
                new SedanSeedSpec("Volkswagen", "Volkswagen Jetta", 2023, 1850m, 510),
                new SedanSeedSpec("Ford", "Ford Focus", 2024, 1750m, 511),
                new SedanSeedSpec("Ford", "Ford Mondeo", 2023, 2250m, 550)
            };

            // SUV Modelleri: Volkswagen (Tiguan, Touareg), Ford (Kuga, Puma), Peugeot (2008, 3008)
            SuvSeedSpec[] suvCatalog = new SuvSeedSpec[]
            {
                new SuvSeedSpec("Volkswagen", "Volkswagen Tiguan", 2025, 2850m, true),
                new SuvSeedSpec("Volkswagen", "Volkswagen Touareg", 2025, 4200m, true),
                new SuvSeedSpec("Ford", "Ford Kuga", 2024, 2600m, true),
                new SuvSeedSpec("Ford", "Ford Puma", 2024, 2150m, false),
                new SuvSeedSpec("Peugeot", "Peugeot 2008", 2024, 2200m, false),
                new SuvSeedSpec("Peugeot", "Peugeot 3008", 2025, 2750m, true)
            };

            int plateSeq = 101;
            foreach (SedanSeedSpec s in sedanCatalog)
            {
                foreach (VehicleColor c in colors)
                {
                    string plate = string.Format("38 DUM {0}", plateSeq++);
                    _vehicles.Add(new SedanVehicle(_nextVehicleId++, plate, s.Brand, s.Model, c, s.Year, s.Rate, s.Trunk));
                }
            }

            foreach (SuvSeedSpec u in suvCatalog)
            {
                foreach (VehicleColor c in colors)
                {
                    string plate = string.Format("38 DUM {0}", plateSeq++);
                    _vehicles.Add(new SuvVehicle(_nextVehicleId++, plate, u.Brand, u.Model, c, u.Year, u.Rate, u.Awd, 200m));
                }
            }
        }

        private void SeedCustomers()
        {
            _customers.Add(new Customer(_nextCustomerId++, "Ahmet", "Yılmaz", "0532 410 22 11", "B-381045"));
            _customers.Add(new Customer(_nextCustomerId++, "Berat", "Kaya", "0543 612 88 34", "B-384920"));
            _customers.Add(new Customer(_nextCustomerId++, "Cengiz", "Demir", "0505 719 45 90", "B-387312"));
        }

        // --- Danışmanlar ---
        public List<RentalConsultant> GetConsultants()
        {
            return new List<RentalConsultant>(_consultants);
        }

        // --- Dashboard İstatistikleri (Koleksiyonlardan Dinamik Hesaplanır) ---
        public int TotalVehicleCount
        {
            get { return _vehicles.Count; }
        }

        public int AvailableVehicleCount
        {
            get { return _vehicles.Filter(v => v.IsAvailable).Count; }
        }

        public int RentedVehicleCount
        {
            get { return _vehicles.Filter(v => !v.IsAvailable).Count; }
        }

        public int TotalCustomerCount
        {
            get { return _customers.Count; }
        }

        public int ActiveRentalCount
        {
            get { return _rentals.Filter(r => r.Status == RentalStatus.Aktif).Count; }
        }

        /// <summary>
        /// Yeni eklenecek araçlar için çakışmayan benzersiz bir plaka önerisi üretir.
        /// </summary>
        public string GenerateUniquePlate()
        {
            int seq = 101;
            string candidate = string.Format("38 DUM {0}", seq);
            while (_vehicles.Any(v => v.PlateNumber.Equals(candidate, StringComparison.OrdinalIgnoreCase)))
            {
                seq++;
                candidate = string.Format("38 DUM {0}", seq);
            }
            return candidate;
        }

        // --- Araç Yönetimi CRUD ---
        public List<Vehicle> GetAllVehicles()
        {
            return _vehicles.GetAll();
        }

        public List<Vehicle> GetAvailableVehicles()
        {
            return _vehicles.Filter(v => v.IsAvailable);
        }

        public List<Vehicle> GetAvailableVehiclesByCategory(string category)
        {
            return _vehicles.Filter(v =>
                v.IsAvailable &&
                v.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        public List<Vehicle> GetVehiclesBySpec(string category, string brand, string model)
        {
            return _vehicles.Filter(v =>
                v.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase) &&
                v.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase) &&
                v.Model.Equals(model, StringComparison.OrdinalIgnoreCase));
        }

        public List<string> GetBrandsByCategory(string category)
        {
            return _vehicles
                .Filter(v => v.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase))
                .Select(v => v.Brand)
                .Distinct()
                .ToList();
        }

        public List<string> GetModelsByCategoryAndBrand(string category, string brand)
        {
            return _vehicles
                .Filter(v => v.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase) &&
                             v.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase))
                .Select(v => v.Model)
                .Distinct()
                .ToList();
        }

        public Vehicle FindAvailableVehicle(string category, string brand, string model, VehicleColor color)
        {
            return _vehicles.Find(v =>
                v.IsAvailable &&
                v.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase) &&
                v.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase) &&
                v.Model.Equals(model, StringComparison.OrdinalIgnoreCase) &&
                v.Color == color);
        }

        public Vehicle FindAnyVehicleBySpec(string category, string brand, string model, VehicleColor color)
        {
            return _vehicles.Find(v =>
                v.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase) &&
                v.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase) &&
                v.Model.Equals(model, StringComparison.OrdinalIgnoreCase) &&
                v.Color == color);
        }

        public Vehicle AddVehicle(string category, string plate, string brand, string model, VehicleColor color, int year, decimal dailyRate)
        {
            string formattedPlate;
            if (!InputValidator.IsValidPlate(plate, out formattedPlate))
                throw new ArgumentException("Geçersiz plaka formatı! Lütfen Türkiye standartlarında geçerli bir plaka giriniz (Örn: 38 DUM 101 veya 34 ABC 123).");

            if (_vehicles.Any(v => v.PlateNumber.Equals(formattedPlate, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException(string.Format("'{0}' plakalı araç sistemde zaten kayıtlıdır. Plaka tekrarına izin verilmez.", formattedPlate));

            Vehicle vehicle = category.Equals("SUV", StringComparison.OrdinalIgnoreCase)
                ? (Vehicle)new SuvVehicle(_nextVehicleId++, formattedPlate, brand, model, color, year, dailyRate)
                : new SedanVehicle(_nextVehicleId++, formattedPlate, brand, model, color, year, dailyRate);

            _vehicles.Add(vehicle);
            OnDataChanged(string.Format("Yeni araç eklendi: {0} {1} ({2})", vehicle.Brand, vehicle.Model, vehicle.PlateNumber));
            return vehicle;
        }

        public void UpdateVehicle(int vehicleId, string plate, string brand, string model, VehicleColor color, int year, decimal dailyRate)
        {
            Vehicle existing = _vehicles.Find(v => v.Id == vehicleId);
            if (existing == null)
                throw new InvalidOperationException("Güncellenecek araç bulunamadı.");

            UpdateVehicle(vehicleId, existing.CategoryName, plate, brand, model, color, year, dailyRate);
        }

        public void UpdateVehicle(int vehicleId, string category, string plate, string brand, string model, VehicleColor color, int year, decimal dailyRate)
        {
            Vehicle existing = _vehicles.Find(v => v.Id == vehicleId);
            if (existing == null)
                throw new InvalidOperationException("Güncellenecek araç bulunamadı.");

            string formattedPlate;
            if (!InputValidator.IsValidPlate(plate, out formattedPlate))
                throw new ArgumentException("Geçersiz plaka formatı! Lütfen Türkiye standartlarında geçerli bir plaka giriniz (Örn: 38 DUM 101 veya 34 ABC 123).");

            if (_vehicles.Any(v => v.Id != vehicleId && v.PlateNumber.Equals(formattedPlate, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException(string.Format("'{0}' plakası başka bir araca aittir.", formattedPlate));

            string targetCat = string.IsNullOrWhiteSpace(category) ? existing.CategoryName : category.Trim();
            if (!existing.CategoryName.Equals(targetCat, StringComparison.OrdinalIgnoreCase))
            {
                Vehicle replacement = targetCat.Equals("SUV", StringComparison.OrdinalIgnoreCase)
                    ? (Vehicle)new SuvVehicle(existing.Id, formattedPlate, brand, model, color, year, dailyRate)
                    : new SedanVehicle(existing.Id, formattedPlate, brand, model, color, year, dailyRate);

                if (!existing.IsAvailable)
                {
                    replacement.MarkAsRented();
                }

                _vehicles.Replace(existing, replacement);
                foreach (Rental r in _rentals.GetAll())
                {
                    if (r.Vehicle != null && r.Vehicle.Id == vehicleId)
                    {
                        r.Vehicle = replacement;
                    }
                }
            }
            else
            {
                existing.PlateNumber = formattedPlate;
                existing.Brand = brand;
                existing.Model = model;
                existing.Color = color;
                existing.Year = year;
                existing.DailyRate = dailyRate;
            }

            OnDataChanged(string.Format("Araç güncellendi: {0}", formattedPlate));
        }

        public void DeleteVehicle(int vehicleId)
        {
            Vehicle existing = _vehicles.Find(v => v.Id == vehicleId);
            if (existing == null)
                throw new InvalidOperationException("Silinecek araç bulunamadı.");

            if (_rentals.Any(r => r.Vehicle.Id == vehicleId))
                throw new InvalidOperationException("Kiralama kaydı bulunan araçlar sistemden silinemez.");

            _vehicles.Remove(existing);
            OnDataChanged(string.Format("Araç silindi: {0}", existing.PlateNumber));
        }

        // --- Müşteri Yönetimi CRUD ---
        public List<Customer> GetAllCustomers()
        {
            return _customers.GetAll();
        }

        public Customer RegisterOrGetLoginCustomer(string fullName)
        {
            if (!InputValidator.IsValidFullName(fullName))
                throw new ArgumentException("Lütfen geçerli bir Ad ve Soyad bilgisi giriniz.");

            Tuple<string, string> parts = InputValidator.SplitFullName(fullName);
            string firstName = parts.Item1;
            string lastName = parts.Item2;

            Customer existing = _customers.Find(c =>
                c.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
                c.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                ActiveCustomer = existing;
                return existing;
            }

            string autoPhone = string.Format("0530 {0:000} {1:00} {2:00}", 100 + _nextCustomerId, 20 + _nextCustomerId, 10 + _nextCustomerId);
            string autoLicense = string.Format("B-38{0}", 1000 + _nextCustomerId);
            Customer created = new Customer(_nextCustomerId++, firstName, lastName, autoPhone, autoLicense);
            _customers.Add(created);
            ActiveCustomer = created;
            OnDataChanged(string.Format("Müşteri girişi ve kaydı oluşturuldu: {0}", created.FullName));
            return created;
        }

        public Customer AddCustomer(string firstName, string lastName, string phone, string licenseNo)
        {
            Customer customer = new Customer(_nextCustomerId++, firstName, lastName, phone, licenseNo);
            _customers.Add(customer);
            OnDataChanged(string.Format("Yeni müşteri eklendi: {0}", customer.FullName));
            return customer;
        }

        public void UpdateCustomer(int customerId, string firstName, string lastName, string phone, string licenseNo)
        {
            Customer existing = _customers.Find(c => c.Id == customerId);
            if (existing == null)
                throw new InvalidOperationException("Güncellenecek müşteri bulunamadı.");

            existing.FirstName = firstName;
            existing.LastName = lastName;
            existing.Phone = phone;
            existing.DriverLicenseNumber = licenseNo;
            OnDataChanged(string.Format("Müşteri güncellendi: {0}", existing.FullName));
        }

        public void DeleteCustomer(int customerId)
        {
            Customer existing = _customers.Find(c => c.Id == customerId);
            if (existing == null)
                throw new InvalidOperationException("Silinecek müşteri bulunamadı.");

            if (_rentals.Any(r => r.Customer.Id == customerId && r.Status == RentalStatus.Aktif))
                throw new InvalidOperationException("Aktif kiralaması bulunan müşteri silinemez.");

            _customers.Remove(existing);
            if (ActiveCustomer != null && ActiveCustomer.Id == customerId)
            {
                ActiveCustomer = _customers.GetAll().FirstOrDefault();
            }
            OnDataChanged(string.Format("Müşteri silindi: {0}", existing.FullName));
        }

        // --- Kiralama ve İade İşlemleri ---
        public List<Rental> GetAllRentals()
        {
            return _rentals.GetAll();
        }

        public List<Rental> GetActiveRentals()
        {
            return _rentals.Filter(r => r.Status == RentalStatus.Aktif);
        }

        public Rental CreateRental(
            Customer customer,
            Vehicle vehicle,
            RentalConsultant consultant,
            DateTime startDate,
            DateTime endDate,
            string durationPackage,
            string paymentMethod)
        {
            if (customer == null)
                throw new ArgumentException("Kiralama için geçerli bir müşteri seçilmelidir.");
            if (vehicle == null)
                throw new ArgumentException("Kiralama için geçerli bir araç seçilmelidir.");
            if (consultant == null)
                throw new ArgumentException("Kiralama danışmanı seçilmelidir.");
            if (!vehicle.IsAvailable)
                throw new InvalidOperationException("Seçilen araç şu anda kirada olduğu için yeniden kiralanamaz.");
            if (endDate.Date < startDate.Date)
                throw new ArgumentException("Bitiş tarihi başlangıç tarihinden önce olamaz.");

            Rental rental = new Rental(
                _nextRentalId++,
                customer,
                vehicle,
                consultant,
                startDate,
                endDate,
                durationPackage,
                paymentMethod);

            vehicle.MarkAsRented();
            _rentals.Add(rental);
            OnDataChanged(string.Format("Kiralama #{0} onaylandı: {1} {2} -> {3}", rental.Id, vehicle.Brand, vehicle.Model, customer.FullName));
            return rental;
        }

        public void ReturnVehicle(int rentalId)
        {
            Rental rental = _rentals.Find(r => r.Id == rentalId);
            if (rental == null)
                throw new InvalidOperationException("İade edilecek kiralama kaydı bulunamadı.");

            rental.CompleteReturn();
            OnDataChanged(string.Format("Kiralama #{0} iade alındı. Araç tekrar Müsait durumda: {1}", rental.Id, rental.Vehicle.PlateNumber));
        }
    }
}
