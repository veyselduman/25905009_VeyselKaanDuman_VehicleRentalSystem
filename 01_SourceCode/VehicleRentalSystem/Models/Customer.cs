using System;
using VehicleRentalSystem.Services;

namespace VehicleRentalSystem.Models
{
    /// <summary>
    /// Müşteri bilgilerini kapsülleyerek yöneten sınıf.
    /// Ad, soyad, telefon (05XX XXX XX XX) ve ehliyet numarası (Örn: B-381045) doğrulamalarını uygular.
    /// </summary>
    public class Customer
    {
        private int _id;
        private string _firstName;
        private string _lastName;
        private string _phone;
        private string _driverLicenseNumber;

        public int Id
        {
            get { return _id; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Müşteri ID sıfırdan büyük olmalıdır.");
                _id = value;
            }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (!InputValidator.IsValidNamePart(value))
                    throw new ArgumentException("Ad alanı en az 2 harften oluşmalı ve yalnızca harf karakterleri içermelidir.");
                _firstName = InputValidator.NormalizeSpaces(value);
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (!InputValidator.IsValidNamePart(value))
                    throw new ArgumentException("Soyad alanı en az 2 harften oluşmalı ve yalnızca harf karakterleri içermelidir.");
                _lastName = InputValidator.NormalizeSpaces(value);
            }
        }

        public string FullName
        {
            get { return string.Format("{0} {1}", _firstName, _lastName).Trim(); }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                string formatted;
                if (!InputValidator.IsValidPhone(value, out formatted))
                    throw new ArgumentException("Geçersiz telefon numarası! Lütfen '05XX XXX XX XX' formatında 11 haneli geçerli bir cep telefonu numarası giriniz (Örn: 0532 410 22 11).");
                _phone = formatted;
            }
        }

        public string DriverLicenseNumber
        {
            get { return _driverLicenseNumber; }
            set
            {
                string formatted;
                if (!InputValidator.IsValidLicenseNumber(value, out formatted))
                    throw new ArgumentException("Geçersiz ehliyet numarası! Ehliyet numarası en az 4 rakam ve 5-15 karakter içermelidir (Örn: B-381045 veya 123456).");
                _driverLicenseNumber = formatted;
            }
        }

        public Customer(int id, string firstName, string lastName, string phone, string driverLicenseNumber)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            DriverLicenseNumber = driverLicenseNumber;
        }

        public override string ToString()
        {
            return string.Format("{0} (Tel: {1})", FullName, Phone);
        }
    }
}
