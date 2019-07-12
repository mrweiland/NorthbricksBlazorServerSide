using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTestServerSide.TfsIterations.Interfaces
{
    public class TeamsResponse
    {
        public List<Team> value { get; set; }
        public int count { get; set; }
    }
    public class Team
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string identityUrl { get; set; }
    }


}
