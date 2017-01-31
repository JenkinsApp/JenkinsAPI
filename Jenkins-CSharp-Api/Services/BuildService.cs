using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jenkins_CSharp_Api.Services
{
    class BuildService
    {
        BuildAdapter buildAdapter = new BuildAdapter();
        //public string Run(string tfsBuildNumber)
        //{
        //    if (string.IsNullOrEmpty(tfsBuildNumber))
        //    {
        //        tfsBuildNumber = Guid.NewGuid().ToString();
        //    }
        //    string nextBuildNumber = buildAdapter.GetNextBuildNumber();
        //    TriggerBuild(tfsBuildNumber, nextBuildNumber);
        //    buildAdapter.WaitUntilBuildStarts(nextBuildNumber);
        //    string realBuildNumber = buildAdapter.GetRealBuildNumber(tfsBuildNumber);
        //    buildAdapter.InitializeSpecificBuildUrl(realBuildNumber);
        //    this.WaitUntilBuildFinish(realBuildNumber);
        //    string buildResult = buildAdapter.GetBuildStatus(realBuildNumber);
        //    return buildResult;
        //}

        //internal string TriggerBuild(string tfsBuildNumber, string nextBuildNumber)
        //{
        //    string buildStatus = string.Empty;
        //    bool isAlreadyBuildTriggered = false;
        //    try
        //    {
        //        buildStatus = buildAdapter.GetSpecificBuildStatusXml(nextBuildNumber);
        //        Debug.WriteLine(buildStatus);
        //    }
        //    catch (WebException ex)
        //    {
        //        if (!ex.Message.Equals("The remote server returned an error: (404) Not Found."))
        //        {
        //            isAlreadyBuildTriggered = true;
        //        }
        //    }
        //    if (isAlreadyBuildTriggered)
        //    {
        //        throw new Exception("Another build with the same build number is already triggered.");
        //    }
        //    string response = this.buildAdapter.TriggerBuild(tfsBuildNumber);
        //    return response;
        //}
        //internal void WaitUntilBuildFinish(string realBuildNumber)
        //{
        //    bool shouldContinue = false;
        //    string buildStatus = string.Empty;
        //    do
        //    {
        //        buildStatus = buildAdapter.GetSpecificBuildStatusXml(realBuildNumber);
        //        bool isProjectBuilding = this.buildAdapter.IsProjectBuilding(buildStatus);
        //        if (!isProjectBuilding)
        //        {
        //            shouldContinue = true;
        //        }
        //        Debug.WriteLine("Waits 5 seconds before the new check if the build is completed...");
        //        Thread.Sleep(5000);
        //    }
        //    while (!shouldContinue);
        //}
    }
}
