using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTestServerSide.TfsIterations.Interfaces
{
    public class IterationsReponse
    {
        public int count { get; set; }
        public List<Iteration> value { get; set; }
    }
    public class Attributes
    {
        public DateTime? startDate { get; set; }
        public DateTime? finishDate { get; set; }
    }

    public class Iteration
    {
        public string id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public Attributes attributes { get; set; }
        public string url { get; set; }
    }


}
