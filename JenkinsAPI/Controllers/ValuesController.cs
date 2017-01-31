using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Jenkins_CSharp_Api;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace JenkinsAPI.Controllers
{
    //[Authorize]
    public class ValuesController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}
        // GET api/values

        JenkinsDotNet.JenkinsServer _jenkinsServer = new JenkinsDotNet.JenkinsServer("http://localhost:8080", "k.kumaraswamy", "3dc4cd8404264a76ebb317571b87b5a4");

        // GET api/values        
        public async Task<IHttpActionResult> GetJobDetails()
        {
            JenkinsDotNet.Model.Node _node = new JenkinsDotNet.Model.Node();            
            _node = await _jenkinsServer.GetNodeDetailsAsync();
            return Ok(_node.Jobs);
        }

        // GET api/values/{projectToken}
        public IHttpActionResult GetTriggerBuild(string jobName, string projectToken)
        {
            Jenkins_CSharp_Api.Infrastructure.HttpAdapter _httpAdapter = new Jenkins_CSharp_Api.Infrastructure.HttpAdapter();
            string response = _httpAdapter.TriggerBuild(jobName, projectToken);            
            return Ok(response);
        }

        public async Task<IHttpActionResult> GetBuildDetails(string jobName)
        {
            JenkinsDotNet.Model.Job _job = new JenkinsDotNet.Model.Job();
            _job = await _jenkinsServer.GetJobDetailsAsync(jobName);
            return Ok(_job.Builds);
        }

        // POST api/values
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/values/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        

    }
}
