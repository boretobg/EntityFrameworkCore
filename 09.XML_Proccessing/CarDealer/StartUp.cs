﻿using CarDealer.Data;
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