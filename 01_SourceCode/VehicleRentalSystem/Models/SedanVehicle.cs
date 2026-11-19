using System;

namespace VehicleRentalSystem.Models
{
    /// <summary>
    /// Sedan kategorisindeki araçları temsil eder (Kalıtım: Vehicle -> SedanVehicle).
    /// Fiat (Egea, Linea), Volkswagen (Passat, Jetta), Ford (Focus, Mondeo).
    /// </summary>
    public class SedanVehicle : Vehicle
    {
        private int _trunkCapacityLiters;

        public int TrunkCapacityLiters
        {
            get { return _trunkCapacityLiters; }
            set
            {
                if (value < 200 || value > 900)
                    throw new ArgumentException("Bagaj hacmi 200-900 litre aralığında olmalıdır.");
                _trunkCapacityLiters = value;
            }
        }

        public override string CategoryName
        {
            get { return "Sedan"; }
        }

        public SedanVehicle(
            int id,
            string plateNumber,
            string brand,
            string model,
            VehicleColor color,
            int year,
            decimal dailyRate,
            int trunkCapacityLiters = 510)
            : base(id, plateNumber, brand, model, color, year, dailyRate)
        {
            TrunkCapacityLiters = trunkCapacityLiters;
        }

        /// <summary>
        /// Çok biçimlilik (Polymorphism): Sedan araçlar için uzun dönem paket indirimli ücret hesabı.
        /// 1 Gün: Standart (Günlük Ücret x Gün)
        /// 7+ Gün (1 Hafta): %10 uzun dönem indirimi
        /// 30+ Gün (1 Ay): %20 aylık kiralama indirimi
        /// </summary>
        public override decimal CalculateRentalPrice(int days)
        {
            if (days <= 0)
                throw new ArgumentException("Kiralama süresi en az 1 gün olmalıdır.");

            decimal baseTotal = DailyRate * days;
            if (days >= 30)
                return Math.Round(baseTotal * 0.80m, 2);
            if (days >= 7)
                return Math.Round(baseTotal * 0.90m, 2);

            return baseTotal;
        }

        public override string GetPriceBreakdown(int days)
        {
            decimal baseTotal = DailyRate * days;
            decimal finalTotal = CalculateRentalPrice(days);
            if (days >= 30)
                return string.Format("{0:N0} TL x {1} Gün = {2:N0} TL | Aylık Sedan İndirimi (-%20) -> {3:N0} TL", DailyRate, days, baseTotal, finalTotal);
            if (days >= 7)
                return string.Format("{0:N0} TL x {1} Gün = {2:N0} TL | Haftalık Sedan İndirimi (-%10) -> {3:N0} TL", DailyRate, days, baseTotal, finalTotal);

            return string.Format("{0:N0} TL x {1} Gün = {2:N0} TL (Standart Sedan Tarifesi)", DailyRate, days, finalTotal);
        }

        public override string GetVehicleDetails()
        {
            return string.Format("[SEDAN] {0} | Bagaj: {1}L", base.GetVehicleDetails(), TrunkCapacityLiters);
        }
    }
}
