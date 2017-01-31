using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Jenkins_CSharp_Api.Infrastructure
{
    public class HttpAdapter
    {

        #region Commented code

        public string Get(string url)
        {
            string responseString = string.Empty;
            var request = (HttpWebRequest)WebRequest.Create(url);
            var httpResponse = (HttpWebResponse)request.GetResponse();
            Stream resStream = httpResponse.GetResponseStream();
            var reader = new StreamReader(resStream);
            responseString = reader.ReadToEnd();
            resStream.Close();
            reader.Close();
            return responseString;
        }
        //public string Post(string url, string postData)
        //{
        //    try
        //    {
        //        string apiToken = "3dc4cd8404264a76ebb317571b87b5a4";
        //        string username = "k.kumaraswamy";

        //        HttpWebRequest request = null;
        //        HttpWebResponse response = null;
        //        request = (HttpWebRequest)WebRequest.Create(url);
        //        request.Credentials = new NetworkCredential(username, apiToken);
        //        request.Method = WebRequestMethods.Http.Get;
        //        byte[] credentialBuffer = new UTF8Encoding().GetBytes(username + ":" + apiToken);
        //        byte[] byteArray = new UTF8Encoding().GetBytes(username + ":" + apiToken);
        //        request.ContentType = "application/x-www-form-urlencoded;";
        //        request.ContentLength = byteArray.Length;
        //        Stream dataStream = request.GetRequestStream();
        //        dataStream.Write(byteArray, 0, byteArray.Length);
        //        dataStream.Close();

        //        response = (HttpWebResponse)request.GetResponse();
        //        string responseFromServer = string.Empty;
        //        if (request != null) request.GetRequestStream().Close();
        //        if (response != null)
        //        {
        //            dataStream = response.GetResponseStream();
        //            var reader = new StreamReader(dataStream);
        //            responseFromServer = reader.ReadToEnd();
        //            reader.Close();
        //            dataStream.Close();
        //            response.Close();
        //        }
        //        return responseFromServer;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public string Post(string URL, string postData)
        {
            string url = URL;
            //string apiToken = "9b2d406a6c019fe2a4cdff12da0814b3";
            //string username = "g.madanagopal";

            string apiToken = "3dc4cd8404264a76ebb317571b87b5a4";
            string username = "k.kumaraswamy";
            string responseFromServer = string.Empty;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = WebRequestMethods.Http.Get;
            byte[] credentialBuffer =
                new UTF8Encoding().GetBytes(
                 username + ":" +
                apiToken);

            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(credentialBuffer);
            //NetworkCredential nc = new NetworkCredential(username, apiToken);
            //httpWebRequest.Credentials = nc;
            httpWebRequest.PreAuthenticate = true;

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    responseFromServer = streamReader.ReadToEnd();
                }
            }
            catch (WebException excp)
            {
                using (var streamReader = new StreamReader(excp.Response.GetResponseStream()))
                {
                    responseFromServer = streamReader.ReadToEnd();
                }
            }
            return responseFromServer;
        }

        #endregion

        #region Trigger build

        static string server = "http://localhost:8080/";
        static string apiToken = "3dc4cd8404264a76ebb317571b87b5a4";
        static string username = "k.kumaraswamy";
        static string responseFromServer = string.Empty;

        public string TriggerBuild(string jobName, string projectToken)
        {
            string response = JenkinsTriggerBuild("http://localhost:8080/job/" + jobName + "/build?token=" + projectToken, jobName);
            return response;
        }

        public static string JenkinsTriggerBuild(string url,string jobName)
        {
            string nextBuildNumber = GetNextBuildNumber(jobName);

            var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = WebRequestMethods.Http.Get;
            byte[] credentialBuffer = new UTF8Encoding().GetBytes(username + ":" + apiToken);

            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(credentialBuffer);
            httpWebRequest.PreAuthenticate = true;

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                var isbuilding = WaitUntilBuildFinish(jobName, nextBuildNumber);                
                if (!isbuilding)
                    responseFromServer = GetBuildResult(jobName, nextBuildNumber);
            }
            catch (WebException excp)
            {
                using (var streamReader = new StreamReader(excp.Response.GetResponseStream()))
                {
                    responseFromServer = streamReader.ReadToEnd();
                }
            }
            return responseFromServer;
        }

        public static bool IsProjectBuilding(string jobName, string nextBuildNumber)
        {
            bool isBuilding = false;
            string apiUrl = server + "job/" + jobName + "/" + nextBuildNumber + "/api/xml";
            string isBuildingStr = GetXmlNodeValue(apiUrl, "building");
            isBuilding = bool.Parse(isBuildingStr);
            return isBuilding;
        }

        public static string GetXmlNodeValue(string apiUrl, string xmlNodeName)
        {
            IEnumerable<XElement> foundElemenets = GetXMLNodeValue(apiUrl, xmlNodeName);
            if (foundElemenets.Count() == 0)
            {
                throw new Exception(
                string.Format("No elements were found for node {0}", xmlNodeName));
            }
            string elementValue = foundElemenets.First().Value;
            return elementValue;
        }

        public static string GetBuildResult(string jobName, string nextBuildNumber)
        {
            string buildResult = string.Empty;
            try
            {
                string apiUrl = server + "job/" + jobName + "/" + nextBuildNumber + "/api/xml";
                buildResult = GetXmlNodeValue(apiUrl, "result");
            }
            catch (Exception)
            {

                throw;
            }

            return buildResult;
        }

        public static string GetNextBuildNumber(string jobName)
        {
            string nextBuildNumber = string.Empty;
            try
            {
                string apiUrl = server + "job/" + jobName + "/api/xml";
                nextBuildNumber = GetXmlNodeValue(apiUrl, "nextBuildNumber");
            }
            catch (Exception)
            {

                throw;
            }

            return nextBuildNumber;
        }

        public static IEnumerable<XElement> GetXMLNodeValue(string apiUrl, string xmlNodeName)
        {
            IEnumerable<XElement> foundElemenets;

            try
            {                
                var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(apiUrl);
                httpWebRequest.ContentType = "application/xml";
                httpWebRequest.Method = WebRequestMethods.Http.Get;
                byte[] credentialBuffer = new UTF8Encoding().GetBytes(username + ":" + apiToken);

                httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(credentialBuffer);
                httpWebRequest.PreAuthenticate = true;

                if (httpWebRequest != null)
                {
                    using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            XDocument document = XDocument.Load(stream);
                            XElement root = document.Root;
                            foundElemenets = from element in root.Descendants(xmlNodeName) select element;
                        }
                    }
                }
                else
                    foundElemenets = null;
            }
            catch (Exception)
            {
                throw;
            }
            return foundElemenets;
        }

        public static bool WaitUntilBuildFinish(string jobName, string nextBuildNumber)
        {
            bool shouldContinue = false;
            string buildStatus = string.Empty;

            //Wait for build creation
            Thread.Sleep(10000);

            do
            {
                bool isProjectBuilding = IsProjectBuilding(jobName, nextBuildNumber);
                if (isProjectBuilding)
                {
                    shouldContinue = true;
                    Debug.WriteLine("Waits 5 seconds before the new check if the build is completed...");
                    Thread.Sleep(5000);
                }
                else
                {
                    shouldContinue = false;
                }
            }
            while (shouldContinue);

            return shouldContinue;
        }

        #endregion

    }
}