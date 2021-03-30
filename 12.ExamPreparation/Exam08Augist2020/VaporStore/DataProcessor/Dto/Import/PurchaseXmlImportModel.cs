using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    [XmlType("Purchase")]
    public class PurchaseXmlImportModel
    {
        [XmlAttribute("title")]
        public string GameName { get; set; }

        [Required]
        [XmlElement("Type")]
        public PurchaseType? PurchaseType { get; set; }

        [Required]
        [RegularExpression("[A-z0-9]{4}-[A-z0-9]{4}-[A-z0-9]{4}")]
        [XmlElement("Key")]
        public string Key { get; set; }

        [Required]
        [RegularExpression(@"\d{4} \d{4} \d{4} \d{4}")]
        [XmlElement("Card")]
        public string Card { get; set; }

        [Required]
        public string Date { get; set; }
    }
}
