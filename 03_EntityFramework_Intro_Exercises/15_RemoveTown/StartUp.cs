namespace SoftUni
{
    using SoftUni.Data;
    using System;
    using System.Linq;

    public class StartUp
    {
        static void Main(string[] args)
        {
            using (var context = new SoftUniContext())
            {
                string result = RemoveTown(context);
                Console.WriteLine(result);
            }
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var town = context.Towns.FirstOrDefault(t => t.Name == "Seattle");

            var townId = context.Towns
                .Where(t => t.Name == "Seattle")
                .Select(t => t.TownId)
                .FirstOrDefault();

            int addressId = context.Addresses
                .Where(a => a.TownId == townId)
                .Select(x => x.AddressId).FirstOrDefault();
                

            var addressesWithTown = context.Addresses
                .Where(t => t.TownId == townId)
                .ToList();

            var employees = context.Employees.Where(e => e.Address.TownId == townId).ToList();

            employees.ForEach(e => e.AddressId = null);

            context.Addresses.RemoveRange(addressesWithTown);
            context.Towns.Remove(town);


            context.SaveChanges();



            return $"{addressesWithTown.Count} addresses in Seattle were deleted."; 
        }
    }
}
