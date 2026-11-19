using System;

namespace VehicleRentalSystem.Models
{
    /// <summary>
    /// SUV kategorisindeki araçları temsil eder (Kalıtım: Vehicle -> SuvVehicle).
    /// Volkswagen (Tiguan, Touareg), Ford (Kuga, Puma), Peugeot (2008, 3008).
    /// </summary>
    public class SuvVehicle : Vehicle
    {
        private bool _hasAllWheelDrive;
        private decimal _suvProtectionFeePerDay;

        public bool HasAllWheelDrive
        {
            get { return _hasAllWheelDrive; }
            set { _hasAllWheelDrive = value; }
        }

        public decimal SuvProtectionFeePerDay
        {
            get { return _suvProtectionFeePerDay; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("SUV koruma paketi ücreti negatif olamaz.");
                _suvProtectionFeePerDay = value;
            }
        }

        public override string CategoryName
        {
            get { return "SUV"; }
        }

        public SuvVehicle(
            int id,
            string plateNumber,
            string brand,
            string model,
            VehicleColor color,
            int year,
            decimal dailyRate,
            bool hasAllWheelDrive = true,
            decimal suvProtectionFeePerDay = 200m)
            : base(id, plateNumber, brand, model, color, year, dailyRate)
        {
            HasAllWheelDrive = hasAllWheelDrive;
            SuvProtectionFeePerDay = suvProtectionFeePerDay;
        }

        /// <summary>
        /// Çok biçimlilik (Polymorphism): SUV araçlarda günlük ücret + SUV güvence/konfor farkı uygulanır.
        /// 7+ Gün için %8, 30+ Gün için %15 uzun dönem indirimi yansıtılır.
        /// </summary>
        public override decimal CalculateRentalPrice(int days)
        {
            if (days <= 0)
                throw new ArgumentException("Kiralama süresi en az 1 gün olmalıdır.");

            decimal effectiveDaily = DailyRate + SuvProtectionFeePerDay;
            decimal grossTotal = effectiveDaily * days;

            if (days >= 30)
                return Math.Round(grossTotal * 0.85m, 2);
            if (days >= 7)
                return Math.Round(grossTotal * 0.92m, 2);

            return grossTotal;
        }

        public override string GetPriceBreakdown(int days)
        {
            decimal effectiveDaily = DailyRate + SuvProtectionFeePerDay;
            decimal grossTotal = effectiveDaily * days;
            decimal finalTotal = CalculateRentalPrice(days);
            string baseText = string.Format("({0:N0} TL + {1:N0} TL SUV Güvence) x {2} Gün = {3:N0} TL", DailyRate, SuvProtectionFeePerDay, days, grossTotal);

            if (days >= 30)
                return string.Format("{0} | Aylık SUV İndirimi (-%15) -> {1:N0} TL", baseText, finalTotal);
            if (days >= 7)
                return string.Format("{0} | Haftalık SUV İndirimi (-%8) -> {1:N0} TL", baseText, finalTotal);

            return baseText;
        }

        public override string GetVehicleDetails()
        {
            string driveText = HasAllWheelDrive ? "4x4" : "4x2";
            return string.Format("[SUV] {0} | Çekiş: {1}", base.GetVehicleDetails(), driveText);
        }
    }
}
