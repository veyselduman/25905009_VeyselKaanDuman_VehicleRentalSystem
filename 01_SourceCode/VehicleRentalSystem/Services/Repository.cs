using System;
using System.Collections.Generic;
using System.Linq;

namespace VehicleRentalSystem.Services
{
    /// <summary>
    /// Bellek içi veri yönetimi sağlayan generic koleksiyon deposu (Generics / List<T>).
    /// </summary>
    public class Repository<T> where T : class
    {
        private readonly List<T> _items = new List<T>();

        public int Count
        {
            get { return _items.Count; }
        }

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            _items.Add(item);
        }

        public bool Remove(T item)
        {
            return _items.Remove(item);
        }

        public bool Replace(T oldItem, T newItem)
        {
            if (oldItem == null || newItem == null)
                return false;
            int index = _items.IndexOf(oldItem);
            if (index < 0)
                return false;
            _items[index] = newItem;
            return true;
        }

        public List<T> GetAll()
        {
            return new List<T>(_items);
        }

        public T Find(Predicate<T> match)
        {
            return _items.Find(match);
        }

        public List<T> Filter(Func<T, bool> predicate)
        {
            return _items.Where(predicate).ToList();
        }

        public bool Any(Func<T, bool> predicate)
        {
            return _items.Any(predicate);
        }
    }
}
