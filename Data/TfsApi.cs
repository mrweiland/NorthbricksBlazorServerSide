using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BlazorTestServerSide.TfsIterations.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace BlazorTestServerSide.TfsIterations
{

    public class TfsApi
    {
        private static readonly HttpClient Client = new HttpClient();
        private static List<WorkItemClassificationNode> _rootIteration = null;
        public static IConfiguration configuration;
        public TfsApi(IConfiguration configuration)
        {
            TfsVariables.Teams = configuration.GetSection("teams").GetChildren().ToList();
            TfsVariables.Collection = configuration["collection"];
            TfsVariables.BaseUrl = configuration["baseUrl"];
            TfsVariables.TeamDefaultBacklogIteration = configuration["teamDefaultBacklogIteration"];
            TfsVariables.Project = configuration["project"];
            TfsVariables.PersonalAccessToken = configuration["personalAccessToken"];
            TfsVariables.IterationToAddToTeams = configuration.GetSection("iterationToAddToTeams").GetChildren().ToList();


            Client.BaseAddress = new Uri(TfsVariables.BaseUrl);
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", "", TfsVariables.PersonalAccessToken))));

            if (_rootIteration == null)
            {
                _rootIteration = TfsApi.GetRootNodes(TfsVariables.Project).GetAwaiter().GetResult();
                if (_rootIteration == null)
                {
                    throw new Exception("Could not found root node");
                }
            }
        }

        public static async Task<GetIterationResponse> GetIteration(string project, string iteration)
        {
            GetIterationResponse objResponse = null;
            Util.WriteLog($"Checking if iteration {iteration} already exists in project {project}");
            using (HttpResponseMessage response = await Client.GetAsync(
                $"/tfs/{TfsVariables.Collection}/{project}/_apis/wit/classificationnodes/Iterations/{iteration}/"))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Util.WriteLog("No iteration found.");
                    return null;
                }
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    objResponse = JsonConvert.DeserializeObject<GetIterationResponse>(content);
                    Util.WriteLog($"Iteration already exist. {objResponse.name} - Has children - {objResponse.hasChildren}");
                }
            }

            return objResponse;
        }

        public static async Task<List<WorkItemClassificationNode>> GetRootNodes(string project)
        {
            RootIterationsNodeReponse objResponse = null;
            List<WorkItemClassificationNode> objWorkItemClassificationNode = null;
            Util.WriteLog($"Checking root node on project {project}");
            using (HttpResponseMessage response = await Client.GetAsync(
                $"/tfs/{TfsVariables.Collection}/{project}/_apis/wit/classificationnodes/"))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Util.WriteLog("No root iteration found.");
                    return null;
                }
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    objResponse = JsonConvert.DeserializeObject<RootIterationsNodeReponse>(content);
                    Util.WriteLog($"Root node exist. ");
                    objWorkItemClassificationNode = objResponse.value;
                    objResponse.value.ForEach(z =>
                    {
                        Util.WriteLog(z.structureType);
                    });
                    
                }
            }

            return objWorkItemClassificationNode;
        }

        public static async Task<GetIterationResponse> CreateIteration(string project, CreateIterationBody body)
        {
            var newIterationPath = String.Empty;
            var iterationPath = body.name.Split('/');
            GetIterationResponse objResponse = null;

            Util.WriteLog("Starting create of iteration " + body.name);

            foreach (var path in iterationPath)
            {
                body.name = path;
                var bodyContent = JsonConvert.SerializeObject(body);
                var stringContent = new StringContent(bodyContent, Encoding.UTF8, "application/json");
                using (HttpResponseMessage response = await Client.PostAsync(
                    $"/tfs/{TfsVariables.Collection}/{project}/_apis/wit/classificationnodes/iterations/{newIterationPath}?api-version=4.1", stringContent))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        Util.WriteLog($"Created iteration path {newIterationPath}");
                        string content = await response.Content.ReadAsStringAsync();
                        objResponse = JsonConvert.DeserializeObject<GetIterationResponse>(content);
                        newIterationPath += path + "/";
                    }
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        Util.WriteLog($"Iteration path already exists {newIterationPath}. Skip create iteration and move on to next. ");
                        //Path exist - but we need to see if subpath exists
                        newIterationPath += path + "/";
                    }
                }
            }
            return objResponse;
        }
        public static async Task<AddIterationToTeamResponse> AddIterationToTeam(string team, string iterationIdentifier)
        {

            AddIterationToTeamResponse objResponse = null;
            var bodyContent = JsonConvert.SerializeObject(new IterationId { id = iterationIdentifier });
            var stringContent = new StringContent(bodyContent, Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await Client.PostAsync(
                $"/tfs/{TfsVariables.Collection}/{TfsVariables.Project}/{team}/_apis/work/teamsettings/Iterations?api-version=3.1", stringContent))
            {
                if (response.IsSuccessStatusCode)
                {

                    string content = await response.Content.ReadAsStringAsync();
                    objResponse = JsonConvert.DeserializeObject<AddIterationToTeamResponse>(content);
                    Util.WriteLog($"Adding iteration to team - identifier {objResponse.path}");
                }
                else
                {
                    Util.WriteLog($"Skip ad iteration to team {team}. Probably already exist.. Identifier - ({iterationIdentifier}) ");
                }
            }

            return objResponse;
        }


        public static async Task<HttpStatusCode> TeamDefaultIterationPath(string team)
        {

            var bodyContent = JsonConvert.SerializeObject(new TeamSettings { defaultIterationMacro="@CurrentIteration", backlogIteration = _rootIteration.Find(i=>i.structureType == "iteration").identifier, bugsBehavior= "asRequirements", workingDays= new List<string>(new string[] { "monday", "tuesday" }) });
            var stringContent = new StringContent(bodyContent, Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await Client.PatchAsync(
                $"/tfs/{TfsVariables.Collection}/{TfsVariables.Project}/{team}/_apis/work/teamsettings?api-version=4.1", stringContent))
            {
                if (response.IsSuccessStatusCode)
                {
                    Util.WriteLog($"Teamsettings done.");
                    return response.StatusCode;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Util.WriteLog($"Does {team} exist? - Error in TeamDefaultIterationPath - {response.StatusCode}");
                    return response.StatusCode;
                }
            }

            return HttpStatusCode.NoContent;
        }


        public static async Task<ProjectsResponse> GetProjects()
        {
            ProjectsResponse objResponse = null;
            using (HttpResponseMessage response = await Client.GetAsync(
                $"/tfs/{TfsVariables.Collection}/_apis/projects/"))
            {
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                objResponse = JsonConvert.DeserializeObject<ProjectsResponse>(content);
                Console.WriteLine($"Projects collected - {objResponse.count}");
            }

            return objResponse;
        }

        public static async Task<IterationsReponse> GetAllIterationsInProject(string project)
        {
            IterationsReponse objResponse = null;
            using (HttpResponseMessage response = await Client.GetAsync(
                $"/tfs/{TfsVariables.Collection}/{project}/_apis/work/teamsettings/iterations"))
            {
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                objResponse = JsonConvert.DeserializeObject<IterationsReponse>(content);
                foreach (var item in objResponse.value)
                {
                    Console.WriteLine($"All iterations in project {project} - {item.path}");
                }
            }

            return objResponse;
        }
        public static async Task<TeamsResponse> GetTeamsInProject(string project)
        {
            TeamsResponse objResponse = null;
            using (HttpResponseMessage response = await Client.GetAsync(
                $"/tfs/{TfsVariables.Collection}/_apis/projects/{project}/teams"))
            {
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                objResponse = JsonConvert.DeserializeObject<TeamsResponse>(content);
                foreach (var item in objResponse.value)
                {
                    Console.WriteLine($"All teams in project {project} - {item.name}");
                }
            }

            return objResponse;
        }

        public static async Task<IterationsReponse> GetTeamsIterations(string project, string team)
        {
            IterationsReponse objResponse = null;
            using (HttpResponseMessage response = await Client.GetAsync(
                $"/tfs/{TfsVariables.Collection}/{project}/{team}/_apis/work/teamsettings/iterations"))
            {
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                objResponse = JsonConvert.DeserializeObject<IterationsReponse>(content);
                foreach (var item in objResponse.value)
                {
                    Console.WriteLine($"Team {team} iterations - {item.name}");
                }
            }

            return objResponse;
        }
        public static async Task<HttpStatusCode> DeleteTeamsIterations(string project, string team, string iterationId)
        {
            using (HttpResponseMessage response = await Client.DeleteAsync(
                $"/tfs/{TfsVariables.Collection}/{project}/{team}/_apis/work/teamsettings/iterations/{iterationId}?api-version=3.1"))
            {
                response.EnsureSuccessStatusCode();
                return response.StatusCode;
            }


        }
    }
    public class Attributes
    {
        //public DateTime? startDate { get; set; }
        //public DateTime? finishDate { get; set; }'
        public string startDate { get; set; }
        public string finishDate { get; set; }
    }

    public class CreateIterationBody
    {
        public string name { get; set; }
        public Attributes attributes { get; set; }
    }

    public class IterationId
    {
        public string id { get; set; }
    }

    public class TeamSettings
    {
        public string bugsBehavior { get; set; }
        public List<string> workingDays { get; set; }
        public string defaultIteration { get; set; }
        public string backlogIteration { get; set; }
        public string defaultIterationMacro { get; set; }
    }
}
