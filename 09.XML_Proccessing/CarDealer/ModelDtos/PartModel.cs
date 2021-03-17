using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.ModelDtos
{
    [XmlType("partId")]
    public class PartModel
    {
        [XmlAttribute("id")]
        public int? Id { get; set; }
    }
}
