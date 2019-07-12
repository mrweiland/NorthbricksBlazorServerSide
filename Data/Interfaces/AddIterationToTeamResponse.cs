using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorTestServerSide.TfsIterations.Interfaces
{


    public class Project
    {
        public string href { get; set; }
    }


    public class TeamSettings
    {
        public string href { get; set; }
    }

    public class TeamIterations
    {
        public string href { get; set; }
    }

    public class Capacity
    {
        public string href { get; set; }
    }

    public class ClassificationNode
    {
        public string href { get; set; }
    }

    public class TeamDaysOff
    {
        public string href { get; set; }
    }

    //public class Links
    //{
    //    public Self self { get; set; }
    //    public Project project { get; set; }
    //    public Team team { get; set; }
    //    public TeamSettings teamSettings { get; set; }
    //    public TeamIterations teamIterations { get; set; }
    //    public Capacity capacity { get; set; }
    //    public ClassificationNode classificationNode { get; set; }
    //    public TeamDaysOff teamDaysOff { get; set; }
    //}

    public class AddIterationToTeamResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public Attributes attributes { get; set; }
        public string url { get; set; }
        public Links _links { get; set; }
    }
}
