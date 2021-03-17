using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.ModelDtos
{
    [XmlType("Sale")]
    public class SaleImportModel
    {
        [XmlElement("carId")]
        public int? CarId { get; set; }

        [XmlElement("customerId")]
        public int CustomerId { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }
    }
}