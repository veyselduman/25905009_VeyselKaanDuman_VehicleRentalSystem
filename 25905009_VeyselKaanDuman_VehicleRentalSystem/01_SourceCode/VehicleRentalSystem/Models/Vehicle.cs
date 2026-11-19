using System;
using VehicleRentalSystem.Interfaces;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Models
{
    /// <summary>
    /// Tüm araç tipleri için temel soyut sınıf (Abstract Class).
    /// Kapsülleme (private alanlar + kontrollü property'ler) ve soyutlama ilkelerini uygular.
    /// </summary>
    public abstract class Vehicle : IRentable
    {
        private int _id;
        private string _plateNumber;
        private string _brand;
        private string _model;
        private VehicleColor _color;
        private int _year;
        private decimal _dailyRate;
        private VehicleStatus _status;

        public int Id
        {
            get { return _id; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Araç ID sıfırdan büyük olmalıdır.");
                _id = value;
            }
        }

        public string PlateNumber
        {
            get { return _plateNumber; }
            set
            {
                string formattedPlate;
                if (!InputValidator.IsValidPlate(value, out formattedPlate))
                    throw new ArgumentException("Geçersiz plaka formatı! Lütfen Türkiye standartlarında geçerli bir plaka giriniz (Örn: 38 DUM 101 veya 34 ABC 123).");
                _plateNumber = formattedPlate;
            }
        }

        public string Brand
        {
            get { return _brand; }
            set
            {
                if (!InputValidator.IsValidBrand(value))
                    throw new ArgumentException("Marka alanı yalnızca harflerden oluşmalı ve en az 2 karakter olmalıdır (Örn: Fiat, Volkswagen, Ford, Peugeot).");
                _brand = InputValidator.NormalizeSpaces(value);
            }
        }

        public string Model
        {
            get { return _model; }
            set
            {
                if (!InputValidator.IsValidModel(value))
                    throw new ArgumentException("Model alanı harf ile başlamalı, en az 2 harf içermeli ve en fazla 4 rakam barındırmalıdır (Örn: Fiat Egea, Peugeot 2008, Ford Focus).");
                _model = InputValidator.NormalizeSpaces(value);
            }
        }

        public VehicleColor Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public int Year
        {
            get { return _year; }
            set
            {
                if (value < 1995 || value > DateTime.Now.Year + 1)
                    throw new ArgumentException("Üretim yılı 1995 ile güncel yıl aralığında olmalıdır.");
                _year = value;
            }
        }

        public decimal DailyRate
        {
            get { return _dailyRate; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Günlük kira ücreti sıfırdan büyük olmalıdır.");
                _dailyRate = value;
            }
        }

        public VehicleStatus Status
        {
            get { return _status; }
            protected set { _status = value; }
        }

        public bool IsAvailable
        {
            get { return _status == VehicleStatus.Musait; }
        }

        public string StatusText
        {
            get { return _status == VehicleStatus.Musait ? "Müsait" : "Kirada"; }
        }

        public string ColorText
        {
            get { return _color.ToString(); }
        }

        /// <summary>
        /// Alt sınıfların kendi kategorisini döndürmesi zorunlu soyut özellik.
        /// </summary>
        public abstract string CategoryName { get; }

        protected Vehicle(int id, string plateNumber, string brand, string model, VehicleColor color, int year, decimal dailyRate)
        {
            Id = id;
            PlateNumber = plateNumber;
            Brand = brand;
            Model = model;
            Color = color;
            Year = year;
            DailyRate = dailyRate;
            _status = VehicleStatus.Musait;
        }

        /// <summary>
        /// Çok biçimlilik (Polymorphism): Her araç türü kendi kiralama ücretini hesaplar.
        /// </summary>
        public abstract decimal CalculateRentalPrice(int days);

        /// <summary>
        /// Alt sınıflarda özelleştirilebilen sanal (virtual) özet bilgisi.
        /// </summary>
        public virtual string GetVehicleDetails()
        {
            return string.Format("{0} {1} ({2}) - {3} | {4}", Brand, Model, Year, ColorText, PlateNumber);
        }

        /// <summary>
        /// Fiyat kırılımını kullanıcıya göstermek için sanal açıklama metodu.
        /// </summary>
        public virtual string GetPriceBreakdown(int days)
        {
            return string.Format("Günlük Ücret ({0:N0} TL) x {1} Gün = {2:N0} TL", DailyRate, days, CalculateRentalPrice(days));
        }

        public void MarkAsRented()
        {
            if (_status == VehicleStatus.Kirada)
                throw new InvalidOperationException("Bu araç zaten kirada durumundadır.");
            _status = VehicleStatus.Kirada;
        }

        public void MarkAsReturned()
        {
            _status = VehicleStatus.Musait;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} - {2} [{3}] ({4})", Brand, Model, ColorText, PlateNumber, StatusText);
        }
    }
}
