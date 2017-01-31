using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenkins_CSharp_Api.Interfaces
{
    interface IHttpAdapter
    {
        string Get(string url);
        string Post(string url, string postData);
    }
}
