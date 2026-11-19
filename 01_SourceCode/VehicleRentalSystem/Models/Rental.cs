using System;

namespace VehicleRentalSystem.Models
{
    /// <summary>
    /// Kiralama kaydını, müşteri, araç, danışman, tarih aralığı, ödeme ve durum bilgisini tutar.
    /// </summary>
    public class Rental
    {
        private int _id;
        private Customer _customer;
        private Vehicle _vehicle;
        private RentalConsultant _consultant;
        private DateTime _startDate;
        private DateTime _endDate;
        private string _durationPackage;
        private decimal _totalPrice;
        private RentalStatus _status;
        private string _paymentMethod;
        private bool _isPaymentApproved;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Customer Customer
        {
            get { return _customer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Customer");
                _customer = value;
            }
        }

        public Vehicle Vehicle
        {
            get { return _vehicle; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Vehicle");
                _vehicle = value;
            }
        }

        public RentalConsultant Consultant
        {
            get { return _consultant; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Consultant");
                _consultant = value;
            }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value.Date; }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (value.Date < _startDate)
                    throw new ArgumentException("Bitiş tarihi başlangıç tarihinden önce olamaz.");
                _endDate = value.Date;
            }
        }

        public string DurationPackage
        {
            get { return _durationPackage; }
            set { _durationPackage = string.IsNullOrWhiteSpace(value) ? "Özel" : value; }
        }

        public int TotalDays
        {
            get { return Math.Max(1, (int)(_endDate - _startDate).TotalDays); }
        }

        public decimal TotalPrice
        {
            get { return _totalPrice; }
            private set { _totalPrice = value; }
        }

        public RentalStatus Status
        {
            get { return _status; }
            private set { _status = value; }
        }

        public string StatusText
        {
            get { return _status == RentalStatus.Aktif ? "Aktif" : "Tamamlandı"; }
        }

        public string PaymentMethod
        {
            get { return _paymentMethod; }
            set { _paymentMethod = value; }
        }

        public bool IsPaymentApproved
        {
            get { return _isPaymentApproved; }
            private set { _isPaymentApproved = value; }
        }

        public Rental(
            int id,
            Customer customer,
            Vehicle vehicle,
            RentalConsultant consultant,
            DateTime startDate,
            DateTime endDate,
            string durationPackage,
            string paymentMethod)
        {
            Id = id;
            Customer = customer;
            Vehicle = vehicle;
            Consultant = consultant;
            StartDate = startDate;
            EndDate = endDate;
            DurationPackage = durationPackage;
            PaymentMethod = string.IsNullOrWhiteSpace(paymentMethod) ? "Kredi Kartı" : paymentMethod;
            TotalPrice = vehicle.CalculateRentalPrice(TotalDays);
            Status = RentalStatus.Aktif;
            IsPaymentApproved = true;
        }

        public void CompleteReturn()
        {
            if (_status == RentalStatus.Tamamlandi)
                throw new InvalidOperationException("Bu kiralama kaydı zaten tamamlanmıştır.");

            _status = RentalStatus.Tamamlandi;
            _vehicle.MarkAsReturned();
        }
    }
}
