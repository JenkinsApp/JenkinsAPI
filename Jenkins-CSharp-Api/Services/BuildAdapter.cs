using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jenkins_CSharp_Api.Infrastructure;
using System.Xml.Linq;
using System.Diagnostics;

namespace Jenkins_CSharp_Api.Services
{
    public class BuildAdapter
    {
        HttpAdapter httpAdapter = new HttpAdapter();

        string jenkinsServerUrl = string.Empty;
        string projectName = string.Empty;        
        //string parameterizedQueueBuildUrl = "http://10.233.4.210:8080/job/TestProject1/build?token=sunnyday";
        string parameterizedQueueBuildUrl = "http://localhost:8080/job/JenkinsApp/build?token=JenkinsAPI";


        public string TriggerBuild(string tfsBuildNumber)
        {
            string response = httpAdapter.Post(
            this.parameterizedQueueBuildUrl,
            string.Concat("TfsBuildNumber=", tfsBuildNumber));
            return response;
        }

        internal string GenerateParameterizedQueueBuildUrl(string jenkinsServerUrl,string projectName)
        {
            string resultUrl = string.Empty;
            Uri result = default(Uri);
            if (Uri.TryCreate(
            string.Concat(jenkinsServerUrl, "/job/", projectName, "/buildWithParameters"),
            UriKind.Absolute,
            out result))
            {
                resultUrl = result.AbsoluteUri;
            }
            else
            {
                throw new ArgumentException(
                "The Parameterized Queue Build Url was not created correctly.");
            }
            return resultUrl;
        }

        internal string GenerateBuildStatusUrl(string jenkinsServerUrl, string projectName)
        {
            string resultUrl = string.Empty;
            Uri result = default(Uri);
            if (Uri.TryCreate(
            string.Concat(jenkinsServerUrl, "/job/", projectName, "/api/xml"),
            UriKind.Absolute,
            out result))
            {
                resultUrl = result.AbsoluteUri;
            }
            else
            {
                throw new ArgumentException(
                "The Build status Url was not created correctly.");
            }
            return resultUrl;
        }

        internal string GenerateSpecificBuildNumberStatusUrl(string buildNumber,string jenkinsServerUrl,string projectName)
        {
            string generatedUrl = string.Empty;
            Uri result = default(Uri);
            if (Uri.TryCreate(
            string.Concat(jenkinsServerUrl, "/job/", projectName, "/", buildNumber, "/api/xml"),
            UriKind.Absolute,
            out result))
            {
                generatedUrl = result.AbsoluteUri;
            }
            else
            {
                throw new ArgumentException(
                "The Specific Build Number Url was not created correctly.");
            }
            return generatedUrl;
        }

        internal string GetXmlNodeValue(string xmlContent, string xmlNodeName)
        {
            IEnumerable<XElement> foundElemenets =
            this.GetAllElementsWithNodeName(xmlContent, xmlNodeName);
            if (foundElemenets.Count() == 0)
            {
                throw new Exception(
                string.Format("No elements were found for node {0}", xmlNodeName));
            }
            string elementValue = foundElemenets.First().Value;
            return elementValue;
        }
        internal IEnumerable<XElement> GetAllElementsWithNodeName(
         string xmlContent,
         string xmlNodeName)
        {
            XDocument document = XDocument.Parse(xmlContent);
            XElement root = document.Root;
            IEnumerable<XElement> foundElemenets =
            from element in root.Descendants(xmlNodeName)
            select element;
            return foundElemenets;
        }

        public int GetQueuedBuildNumber(string xmlContent, string queuedBuildName)
        {
            IEnumerable<XElement> buildElements =
            this.GetAllElementsWithNodeName(xmlContent, "build");
            string nextBuildNumberStr = string.Empty;
            int nextBuildNumber = -1;
            foreach (XElement currentElement in buildElements)
            {
                nextBuildNumberStr = currentElement.Element("number").Value;
                string currentBuildSpecificUrl =
                this.GenerateSpecificBuildNumberStatusUrl(
                nextBuildNumberStr,
                this.jenkinsServerUrl,
                this.projectName);
                string newBuildStatus = this.httpAdapter.Get(currentBuildSpecificUrl);
                string currentBuildName = this.GetBuildTfsBuildNumber(newBuildStatus);
                if (queuedBuildName.Equals(currentBuildName))
                {
                    nextBuildNumber = int.Parse(nextBuildNumberStr);
                    Debug.WriteLine("The real build number is {0}", nextBuildNumber);
                    break;
                }
            }
            if (nextBuildNumber == -1)
            {
                throw new Exception(
                string.Format(
                "Build with name {0} was not find in the queued builds.",
                queuedBuildName));
            }
            return nextBuildNumber;
        }

        private string GenerateSpecificBuildNumberStatusUrl(string nextBuildNumberStr, string jenkinsServerUrl, object projectName)
        {
            throw new NotImplementedException();
        }

        public string GetBuildTfsBuildNumber(string xmlContent)
        {
            IEnumerable<XElement> foundElements =
            from el in this.GetAllElementsWithNodeName(xmlContent, "parameter").Elements()
            where el.Value == "TfsBuildNumber"
            select el;
            if (foundElements.Count() == 0)
            {
                throw new ArgumentException("The TfsBuildNumber was not set!");
            }
            string tfsBuildNumber =
            foundElements.First().NodesAfterSelf().OfType<XElement>().First().Value;
            return tfsBuildNumber;
        }
        public bool IsProjectBuilding(string xmlContent)
        {
            bool isBuilding = false;
            string isBuildingStr = this.GetXmlNodeValue(xmlContent, "building");
            isBuilding = bool.Parse(isBuildingStr);
            return isBuilding;
        }
        public string GetBuildResult(string xmlContent)
        {
            string buildResult = this.GetXmlNodeValue(xmlContent, "result");
            return buildResult;
        }
        public string GetNextBuildNumber(string xmlContent)
        {
            string nextBuildNumber = this.GetXmlNodeValue(xmlContent, "nextBuildNumber");
            return nextBuildNumber;
        }
        public string GetUserName(string xmlContent)
        {
            string userName = this.GetXmlNodeValue(xmlContent, "userName");
            return userName;
        }
    }
}
