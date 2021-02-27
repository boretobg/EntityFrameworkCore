using P01_StudentSystem.Data;
using System;

namespace P01_StudentSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new StudentSystemContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
