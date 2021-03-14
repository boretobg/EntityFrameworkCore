using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TravelledDistance
                    },
                    parts = c.PartCars
                    .Select(p => new
                    { 
                        Name = p.Part.Name,
                        Price = $"{p.Part.Price:f2}"
                    })
                })
                .ToList();

            var result = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return result;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    PartsCount = x.Parts.Count()
                })
                .ToList();

            var result = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return result;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.Make == "Toyota")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x => new
                {
                    x.Id,
                    x.Make,
                    x.Model,
                    x.TravelledDistance
                })
                .ToList();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .Select(x => new
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = x.IsYoungDriver
                })
                .ToList();

            var result = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return result;
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<IEnumerable<Sale>>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";

        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<IEnumerable<Customer>>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<IEnumerable<Car>>(inputJson);

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count()}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var suppliers = context.Suppliers.Select(x => x.Id).ToList();

            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson).Where(x => suppliers.Contains(x.SupplierId)).ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";
        }
    }
}