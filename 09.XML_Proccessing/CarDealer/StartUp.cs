using CarDealer.Data;
using CarDealer.ExportModelDtos;
using CarDealer.ModelDtos;
using CarDealer.Models;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args){}

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => !x.IsImporter)
                .Select(s => new SupplierExportModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count()
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(SupplierExportModel[]), new XmlRootAttribute("suppliers"));

            var textWriter = new StringWriter();

            var nameSpace = new XmlSerializerNamespaces();
            nameSpace.Add("", "");

            xmlSerializer.Serialize(textWriter, suppliers, nameSpace);

            return textWriter.ToString();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.Make == "BMW")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(c => new CarBMWExportModel
                { 
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CarBMWExportModel[]), new XmlRootAttribute("cars"));

            var textWriter = new StringWriter();

            var nameSpace = new XmlSerializerNamespaces();
            nameSpace.Add("", "");

            xmlSerializer.Serialize(textWriter, cars, nameSpace);

            return textWriter.ToString();
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.TravelledDistance > 2_000_000)
                .Select(c => new CarExportModel
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CarExportModel[]), new XmlRootAttribute("cars"));

            var textWriter = new StringWriter();

            var nameSpace = new XmlSerializerNamespaces();
            nameSpace.Add("", "");

            xmlSerializer.Serialize(textWriter, cars, nameSpace);

            return textWriter.ToString();
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var xmlSerialized = new XmlSerializer(typeof(SaleImportModel[]), new XmlRootAttribute("Sales"));

            var textRead = new StringReader(inputXml);

            var salesDto = xmlSerialized.Deserialize(textRead) as SaleImportModel[];

            var sales = salesDto
                .Where(x => x.CarId != null)
                .Select(x => new Sale
                {
                    CarId = (int)x.CarId,
                    CustomerId = x.CustomerId,
                    Discount = x.Discount
                })
                .ToList();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var xmlSerialized = new XmlSerializer(typeof(CustomerImportModel[]), new XmlRootAttribute("Customers"));

            var textRead = new StringReader(inputXml);

            var customersDto = xmlSerialized.Deserialize(textRead) as CustomerImportModel[];

            var customers = customersDto
                .Select(x => new Customer
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate,
                    IsYoungDriver = x.IsYoungDriver
                })
                .ToList();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var xmlSerialized = new XmlSerializer(typeof(CarImportModel[]), new XmlRootAttribute("Cars"));

            var textRead = new StringReader(inputXml);

            var carDto = xmlSerialized.Deserialize(textRead) as CarImportModel[];

            var cars = carDto
                .Distinct()
                .Select(x => new CarImportModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TraveledDistance = x.TraveledDistance,
                    Parts = x.Parts
                    .Where(x => x.Id != null)
                    .Select(p => new PartModel
                    {
                        Id = p.Id
                    })
                    .ToArray()
                })
                .ToList();

            context.Cars.AddRange((System.Collections.Generic.IEnumerable<Car>)cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlSerialized = new XmlSerializer(typeof(PartImportModel[]), new XmlRootAttribute("Parts"));

            var textRead = new StringReader(inputXml);

            var partDto = xmlSerialized.Deserialize(textRead) as PartImportModel[];

            var parts = partDto
                .Where(s => s.SupplierId != null)
                .Select(x => new Part
                {
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    SupplierId = (int)x.SupplierId
                })
                .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlSerialized = new XmlSerializer(typeof(SupplierImportModel[]), new XmlRootAttribute("Suppliers"));

            var textRead = new StringReader(inputXml);

            var supplierDto = xmlSerialized.Deserialize(textRead) as SupplierImportModel[];

            var suppliers = supplierDto
                .Select(x => new Supplier
                {
                    Name = x.Name,
                    IsImporter = x.IsImporter
                })
                .ToList();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }
    }
}