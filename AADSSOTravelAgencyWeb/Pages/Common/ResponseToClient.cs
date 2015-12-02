/*
 * 
 * This class is used to Send server (aspx) response to client (js) 
 * 
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP.AppsForOffice.Workflow.Pages.Common
{
    public class ResponseToClient
    {
        public int statusCode;
        public string statusMsg;
        public string[] response;
        public string[][] decisionOptions;
        public string postresponse;
    }
}