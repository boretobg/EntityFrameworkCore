using MusicHub.Data;

namespace MusicHub
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new MusicHubDbContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
