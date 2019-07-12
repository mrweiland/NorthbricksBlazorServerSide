using System.Collections.Generic;

namespace BlazorTestServerSide.TfsIterations.Interfaces
{

    public class TfsProjects
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string state { get; set; }
        public int revision { get; set; }
        public string visibility { get; set; }
    }

    public class ProjectsResponse
    {
        public int count { get; set; }
        public List<TfsProjects> value { get; set; }
    }

}