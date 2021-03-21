using Microsoft.EntityFrameworkCore;
using RealEstates.Data;
using RealEstates.Services;
using System;

namespace RealEstates.ConsoleApplication
{
    public class Program
    {
        static void Main(string[] args)
        {
            var db = new ApplicationDbContext();
            db.Database.Migrate();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Choose an option:");
                Console.WriteLine("0. EXIT");
                Console.WriteLine("1. Property search");
                Console.WriteLine("2. Most expensive districts");

                bool parsed = int.TryParse(Console.ReadLine(), out int option);

                if (parsed && option == 0)
                {
                    break;
                }

                if (parsed && (option >= 1 && option <= 2))
                {
                    switch (option)
                    {
                        case 1:
                            PropertySearch(db);
                            break;
                        case 2:
                            MostExpensiveDistrict(db);
                            break;
                        default:
                            break;
                    }

                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private static void MostExpensiveDistrict(ApplicationDbContext db)
        {
            IDistricsService districsService = new DistrictsService(db);
            var districts = districsService.GetMostExpensiveDistricts(20);

            foreach (var dists in districts)
            {
                Console.WriteLine($"{dists.Name} => {dists.AveragePricePerSquareMeter}eu/m2 ({dists.PropertiesCount})");
            }
        }

        private static void PropertySearch(ApplicationDbContext db)
        {
            Console.Write("Min price: ");
            int minPrice = int.Parse(Console.ReadLine());

            Console.Write("Max price: ");
            int maxPrice = int.Parse(Console.ReadLine());

            Console.Write("Min size: ");
            int minSize = int.Parse(Console.ReadLine());

            Console.Write("Max size: ");
            int maxSize = int.Parse(Console.ReadLine());

            IPropertiesService service = new PropertiesService(db);
            var properties = service.Search(minPrice, maxPrice, minSize, maxSize);

            foreach (var prop in properties)
            {
                Console.WriteLine($"-- {prop.DistrictName}; {prop.BuildingType}; {prop.PropertyType} => {prop.Price}eu => {prop.Size}m2");
            }
        }
    }
}
