using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorTestServerSide.TfsIterations.Interfaces
{
    public class Self
    {
        public string href { get; set; }
    }

    public class Parent
    {
        public string href { get; set; }
    }

    public class Links
    {
        public Self self { get; set; }
        public Parent parent { get; set; }
    }

    public class GetIterationResponse
    {
        public int id { get; set; }
        public string identifier { get; set; }
        public string name { get; set; }
        public string structureType { get; set; }
        public bool hasChildren { get; set; }
        public Links _links { get; set; }
        public string url { get; set; }
        public Attributes attributes { get; set; }
    }
}
public class Attributes
{
    public DateTime? startDate { get; set; }
    public DateTime? finishDate { get; set; }
}



