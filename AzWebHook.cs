using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace Contoso.Adapter
{
    public static class AzWebHook
    {
        [FunctionName("AzWebHook")]
        public static async Task<IActionResult> Run(
            // we only set post because Azure DevOps webhooks are POST only
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                // on this example we will save the project, title and markdown to a json object

                // get the json from the request
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                // get the project name
                string project = data.resource.project.name;
                // get the title
                string title = data.resource.title;
                // get the markdown
                string markdown = data.resource.content.markdown;
                // create a json object
                dynamic json = new
                {
                    project = project,
                    title = title,
                    markdown = markdown
                };

                return new OkObjectResult(json);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error in AzWebHook");
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
