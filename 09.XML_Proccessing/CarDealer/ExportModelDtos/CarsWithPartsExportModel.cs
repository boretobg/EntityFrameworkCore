using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.ExportModelDtos
{
    [XmlType("car")]
    public class CarsWithPartsExportModel
    {
        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }

        [XmlElement("parts")]
        public List<PartsExportModel> Parts { get; set; }
    }
}
