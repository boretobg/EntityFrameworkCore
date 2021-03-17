using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.ModelDtos
{
    [XmlType("Car")]
    public class CarImportModel
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        public int TraveledDistance { get; set; }

        [XmlElement("parts")]
        public PartModel[] Parts { get; set; }
    }
}
