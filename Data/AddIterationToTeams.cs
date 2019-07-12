using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions;
using Microsoft.Extensions.Configuration;

namespace BlazorTestServerSide.TfsIterations
{
    public static class AddIterationToTeams
    {
        public static IConfiguration configuration;
        static AddIterationToTeams()
        {
            
            //todo fix configuration
         
        }

        public static async Task CreateIteration(IConfiguration configuration)
        {

            Util.WriteLog("BaseUrl", ConsoleColor.Cyan);
            Util.WriteLog(TfsVariables.BaseUrl.ToString());
            Util.WriteLog("Collection ", ConsoleColor.Cyan);
            Util.WriteLog(TfsVariables.Collection);
            Util.WriteLog("Personal Access Token", ConsoleColor.Cyan);
            Util.WriteLog(TfsVariables.PersonalAccessToken);
            Util.WriteLog("Azure DevOps project ", ConsoleColor.Cyan);
            Util.WriteLog(TfsVariables.Project);
            Util.WriteLog("Adding iterations to this teams", ConsoleColor.Cyan);
            TfsVariables.Teams.ForEach(o => Util.WriteLog("-" + o.Value));
            Util.WriteLog("Iterations to add to teams:", ConsoleColor.Cyan);
            TfsVariables.IterationToAddToTeams.ForEach(c => Util.WriteLog("-" + c.Key));


            Util.WriteLog("Press enter when ready to change iterations? If not ready press ctrl-c and start again.", ConsoleColor.Green);
            Console.ReadLine();


            var iterationIdentifier = new List<string>();
            var a = new Attributes();
            TfsVariables.IterationToAddToTeams.ForEach(iterationAppSettings =>
            {
                var iteration = TfsApi.GetIteration(TfsVariables.Project, iterationAppSettings.Key).GetAwaiter().GetResult();

                if (iteration == null)
                {
                    Util.WriteLog($"Creating iteration {iterationAppSettings.Key}");

                    IEnumerable<IConfigurationSection> section = configuration.GetSection(iterationAppSettings.Path + ":attributes").GetChildren();
                    
                    foreach (var attr in section)
                    {
                        switch (attr.Key)
                        {
                            case "startDate":
                                //a.startDate = DateTime.ParseExact(attr.Value, "yyyy-mm-dd", null);
                                a.startDate = attr.Value + "T00:00:00Z";
                                Console.WriteLine($"Creating iteration startdate {a.startDate}");
                                break;
                            case "finishDate":
                                a.finishDate = attr.Value + "T00:00:00Z";
                                Console.WriteLine($"Creating iteration startdate {a.finishDate}");
                                break;

                        }
                    }

                    var o = new CreateIterationBody
                    {
                        name = iterationAppSettings.Key,
                        attributes = a
                    };

                    var createdIteration = TfsApi.CreateIteration(TfsVariables.Project, o).GetAwaiter().GetResult();
                    iterationIdentifier.Add(createdIteration.identifier);
                }
                else
                {
                    Console.WriteLine($"Iteration {iterationAppSettings.Key} already exists");
                    iterationIdentifier.Add(iteration.identifier);
                }
            });


            HttpStatusCode statusCode;
            foreach (var configTeams in TfsVariables.Teams)
            {
                
                Console.WriteLine($"Adding iteration to team {configTeams.Value}");
                foreach (var identifier in iterationIdentifier)
                {
                    statusCode = TfsApi.TeamDefaultIterationPath(configTeams.Value).GetAwaiter().GetResult();
                    if (statusCode != HttpStatusCode.NotFound)
                    {
                        var addIteration = await TfsApi.AddIterationToTeam(configTeams.Value, identifier);
                    }
                    else
                    {
                        break;
                    }
                }

            }


        }
        //}
    }
}
