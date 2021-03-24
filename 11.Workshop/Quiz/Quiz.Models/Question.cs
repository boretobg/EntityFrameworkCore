using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Models
{
    public class Question
    {
        public Question()
        {
            this.Answers = new HashSet<Answer>();
        }

        public int  Id { get; set; }

        public string Title { get; set; }

        public ICollection<Answer> Answers { get; set; }
    }
}
