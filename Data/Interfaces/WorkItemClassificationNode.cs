using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorTestServerSide.TfsIterations.Interfaces
{

    public class WorkItemClassificationNode
    {
        public int id { get; set; }
        public string identifier { get; set; }
        public string name { get; set; }
        public string structureType { get; set; }
        public bool hasChildren { get; set; }
        public string url { get; set; }
    }

    public class RootIterationsNodeReponse
    {
        public int count { get; set; }
        public List<WorkItemClassificationNode> value { get; set; }
    }
}
