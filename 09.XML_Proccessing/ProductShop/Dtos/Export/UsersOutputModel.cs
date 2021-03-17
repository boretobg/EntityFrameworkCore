using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("User")]
    public class UsersOutputModel
    {

        public UsersOutputModel()
        {
            this.SoldProducts = new List<ProductsUsersExportModel>();
        }

        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("soldProducts")]
        public List<ProductsUsersExportModel> SoldProducts { get; set; }
    }
}
