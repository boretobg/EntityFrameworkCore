using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.ModelDtos
{
    [XmlType("Supplier")]
    public class SupplierImportModel
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("isImporter")]
        public bool IsImporter { get; set; }
    }
}
