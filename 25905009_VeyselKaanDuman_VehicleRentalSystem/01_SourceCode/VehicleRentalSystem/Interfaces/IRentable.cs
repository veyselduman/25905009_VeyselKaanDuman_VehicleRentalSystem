using System;

namespace VehicleRentalSystem.Interfaces
{
    /// <summary>
    /// Kiralanabilir varlıklar için ortak sözleşmeyi tanımlar (Soyutlama / Abstraction).
    /// </summary>
    public interface IRentable
    {
        bool IsAvailable { get; }
        decimal DailyRate { get; }
        decimal CalculateRentalPrice(int days);
        void MarkAsRented();
        void MarkAsReturned();
    }
}
