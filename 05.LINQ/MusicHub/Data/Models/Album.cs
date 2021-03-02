using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MusicHub.Data.Models
{
    public class Album
    {
        private ICollection<Song> songs;

        public Album()
        {
            this.Songs = new HashSet<Song>();
        }

        public int Id { get; set; }

        [MaxLength(40), Required]
        public string Name { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        public decimal Price 
        {
            get
            {
                return this.Songs.Sum(x => x.Price);
            }
            set
            {
                this.songs = Songs;
            }
        } 

        //todo 	Price – calculated property(the sum of all song prices in the album)

        public int ProducerId { get; set; }
        public Producer Producer { get; set; }

        public ICollection<Song> Songs { get; set; }
    }
}
