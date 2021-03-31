using BookShop.Data.Models.Enums;
using System;
using System.Collections.Generic;

namespace BookShop.Data.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Genre Genre { get; set; }

        public decimal Price { get; set; }

        public int Pages { get; set; }

        public DateTime PublishedOn { get; set; }

        public ICollection<AuthorBook> AuthorsBooks { get; set; }
    }
}
