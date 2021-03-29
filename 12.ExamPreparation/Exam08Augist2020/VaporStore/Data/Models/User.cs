using System;
using System.Collections.Generic;
using System.Text;

namespace VaporStore.Data.Models
{
    public class User
    {
        public User()
        {
            this.Cards = new HashSet<Card>();
        }

        public int Id { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public int Age { get; set; }

        public ICollection<Card> Cards { get; set; }
    }
}
