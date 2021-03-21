using Microsoft.EntityFrameworkCore;
using RealEstates.Data;
using System;

namespace RealEstates.ConsoleApplication
{
    public class Program
    {
        static void Main(string[] args)
        {
            var db = new ApplicationDbContext();
            db.Database.Migrate();

            db.Districts.Add(new Models.District { Name = "Center"});
            db.SaveChanges();
        }
    }
}
