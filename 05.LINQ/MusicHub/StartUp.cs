namespace MusicHub
{
    using System;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Producers
               .FirstOrDefault(x => x.Id == producerId)
               .Albums
               .Select(a => new
               {
                   AlbumName = a.Name,
                   ReleaseDate = a.ReleaseDate,
                   ProducerName = a.Producer.Name,
                   AlbumSongs = a.Songs.Select(s => new
                   {
                       SongName = s.Name,
                       SongPrice = s.Price,
                       SongWriter = s.Writer.Name
                   })
                   .OrderByDescending(x => x.SongName)
                   .ThenBy(x => x.SongWriter)
                   .ToList(),
                   AlbumPrice = a.Price
               })
               .OrderByDescending(x => x.AlbumPrice)
               .ToList(); ;

            StringBuilder sb = new StringBuilder();

            foreach (var album in albums)
            {
                sb.AppendLine($"-AlbumName: {album.AlbumName}")
                  .AppendLine($"-ReleaseDate: {album.ReleaseDate:MM/dd/yyyy}")
                  .AppendLine($"-ProducerName: {album.ProducerName}")
                  .AppendLine($"-Songs:");

                int count = 1;

                foreach (var song in album.AlbumSongs)
                {
                    sb.AppendLine($"---#{count++}")
                      .AppendLine($"---SongName: {song.SongName}")
                      .AppendLine($"---Price: {song.SongPrice:f2}")
                      .AppendLine($"---Writer: {song.SongWriter}");
                }

                sb.AppendLine($"-AlbumPrice: {album.AlbumPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs
                .Include(x => x.Album)
                .ThenInclude(x => x.Producer)
                .Include(x => x.Writer)
                .ToList()   
                .Where(x => x.Duration.TotalSeconds > duration)
                .Select(x => new
                {
                    SongName = x.Name,
                    PerformerFullName = x.SongPerformers.Select(x => $"{x.Performer.FirstName} {x.Performer.LastName}")
                    .FirstOrDefault(),
                    WriterName = x.Writer.Name,
                    AlbumProducer = x.Album.Producer.Name,
                    x.Duration
                })
                .OrderBy(x => x.SongName)
                .ThenBy(x => x.WriterName)
                .ThenBy(x => x.PerformerFullName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            int count = 1;

            foreach (var song in songs)
            {
                sb.AppendLine($"-Song #{count++}")
                  .AppendLine($"---SongName: {song.SongName}")
                  .AppendLine($"---Writer: {song.WriterName}")
                  .AppendLine($"---Performer: {song.PerformerFullName}")
                  .AppendLine($"---AlbumProducer: {song.AlbumProducer}")
                  .AppendLine($"---Duration: {song.Duration:c}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
