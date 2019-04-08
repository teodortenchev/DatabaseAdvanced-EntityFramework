using System;

namespace CarDealer.Models
{
    public class PartCar
    {
        public int PartId { get; set; }
        public Part Part { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as PartCar;
            if (other == null)
            {
                return false;
            }
            return PartId == other.PartId && CarId == other.CarId;
        }

        public override int GetHashCode()
        {
            return PartId ^ CarId;
        }
    }
}
