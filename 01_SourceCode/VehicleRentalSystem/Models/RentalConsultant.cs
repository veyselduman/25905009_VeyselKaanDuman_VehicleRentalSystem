using System;

namespace VehicleRentalSystem.Models
{
    /// <summary>
    /// Duman Rent a Car kiralama danışmanı varlığı (Veysel Duman, Kaan Duman).
    /// </summary>
    public class RentalConsultant
    {
        private int _id;
        private string _fullName;
        private string _title;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Danışman adı boş olamaz.");
                _fullName = value.Trim();
            }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public RentalConsultant(int id, string fullName, string title)
        {
            Id = id;
            FullName = fullName;
            Title = title;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", FullName, Title);
        }
    }
}
