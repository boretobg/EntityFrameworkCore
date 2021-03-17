using CarDealer.Data;
using CarDealer.ModelDtos;
using CarDealer.Models;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {

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