using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTestServerSide.TfsIterations
{
    public class TfsVariables
    {
        public static string Collection { get; set; }
        public static List<IConfigurationSection> Teams { get; set; }
        public static string BaseUrl { get; internal set; }
        public static string Project { get; internal set; }
        public static string PersonalAccessToken { get; set; }
        public static List<IConfigurationSection> IterationToAddToTeams { get; set; }

        public static string TeamDefaultBacklogIteration { get; set; }

    }
}
