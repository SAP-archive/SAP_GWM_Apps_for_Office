using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAP.AppsForOffice.Workflow.Utilities;
using SAP.AppsForOffice.Workflow.Pages.Common;
//using AADSSOTravelAgencyWeb.Pages.Updated;

namespace SAP.AppsForOffice.Workflow.Pages.AzurePages
{
    public partial class Default : System.Web.UI.Page
    {           
        private static readonly string systemType = ConfigurationManager.AppSettings["SystemType"];
        private static readonly string serviceUrl = ConfigurationManager.AppSettings["AzureServiceUrl"];
        private static readonly string Collection = "TaskCollection";
        
        /// <summary>
        /// Gets host type & stores auth code on the page load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            
                if (!IsPostBack)
                {
                    AADAuthHelper.CurrentHostType = HttpContext.Current.Request.QueryString["_host_Info"];
                    AADAuthHelper.StoreAuthorizationCodeFromRequest(this.Request);
                }
            
        }
        
        /// <summary>
        /// Checks if authorization code is available
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string CheckCode()
        {
            if (AADAuthHelper.IsAuthorized)
            {
                if (!string.IsNullOrEmpty(AADAuthHelper.AuthorizationCode))
                    return "Finished";
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the host type
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string GetHostType()
        {
            return AADAuthHelper.CurrentHostType;
        }

        /// <summary>
        /// Gets the authorize url for authentication with azure ad
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string GetAuthorizeUrl()
        {
            if (!AADAuthHelper.IsAuthorized)
            {
                return AADAuthHelper.AuthorizeUrl;
            }
            return string.Empty;
        }

        /// <summary>
        /// Approves/rejects the task request
        /// </summary>
        /// <param name="decisionKey"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(UseHttpGet = false)]
        public static ResponseToClient PostDecision(string decisionKey, string taskId)
        {
            ResponseToClient responseToClient = new ResponseToClient();
            try
            {
                var accessToken = AADAuthHelper.EnsureValidAccessToken(HttpContext.Current);
                responseToClient.postresponse = DataGetter.PostJsonString(serviceUrl + "Decision?InstanceID='" + taskId + "'&DecisionKey='" + decisionKey + "'", accessToken);
                responseToClient.statusMsg = "OK";
            }
            catch (WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                int statusCode = (int)res.StatusCode;

                responseToClient.statusCode = statusCode;
                responseToClient.statusMsg = "Exception";
                responseToClient.postresponse = ex.Message;
            }
            return responseToClient;
 
        }
       
        /// <summary>
        /// Gets task details from the service
        /// </summary>
        /// <param name="id"></param>
        /// <param name="coll"></param>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(UseHttpGet=true)]
        public static ResponseToClient GetTaskDetails(string id, string coll)
        {
            ResponseToClient responseToClient = new ResponseToClient();
            RootObject customAttrs;
            try
            {
                var accessToken = AADAuthHelper.EnsureValidAccessToken(HttpContext.Current);
                var dataList = DataGetter.GetDataMatrix(serviceUrl + Collection + "('" + id + "')", accessToken);
                var description = DataGetter.GetDescription(serviceUrl + Collection + "('" + id + "')/Description", accessToken);                
                customAttrs = DataGetter.GetCustomAttributes(serviceUrl + Collection + "('" + id + "')/CustomAttributeData", accessToken);
                responseToClient.response = new string[dataList.Length + 4];
                Array.Copy(dataList, 0, responseToClient.response, 0, dataList.Length);
                responseToClient.response[6] = description;
                if (dataList[5] == ConfigurationManager.AppSettings["AzureRequestType"])
                {
                    string startDate = "";
                    string endDate = "";
                    string leaveDuration = "";
                    if (customAttrs!= null && customAttrs.results != null && customAttrs.results.Count > 0)
                    {
                        var startObj = customAttrs.results.FirstOrDefault(x => x.Name == "/SWL/ALL/DYNCOL2");
                        startDate = string.Format("{2}-{1}-{0}", startObj.Value.Substring(6, 2), startObj.Value.Substring(4, 2), startObj.Value.Substring(0, 4));
                        DateTime sd = new DateTime();
                        DateTime ed = new DateTime();
                        sd = DateTime.ParseExact(startDate, "yyyy-MM-dd", null);
                        var endObj = customAttrs.results.FirstOrDefault(x => x.Name == "/SWL/ALL/DYNCOL3");
                        endDate = string.Format("{2}-{1}-{0}", endObj.Value.Substring(6, 2), endObj.Value.Substring(4, 2), endObj.Value.Substring(0, 4));
                        ed = DateTime.ParseExact(endDate, "yyyy-MM-dd", null);
                        var lduration = ed.Subtract(sd);
                        string dayText = lduration.Days > 1 ? "Days" : "Day";
                        leaveDuration = lduration.Days.ToString() + " " + dayText;
                    }
                    
                    responseToClient.response[7] = startDate;
                    responseToClient.response[8] = endDate;
                    responseToClient.response[9] = leaveDuration;
                }
                responseToClient.statusMsg = "OK";
            }
            catch (WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                int statusCode = (int)res.StatusCode;

                responseToClient.statusCode = statusCode;
                responseToClient.statusMsg = "Exception";
                responseToClient.response = new string[1];
                responseToClient.response[0] = ex.Message;
            }
            return responseToClient;
        }

        /// <summary>
        /// Gets window url from config file
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string[] GetWindowUrl()
        {
            string[] url = new string[2];
            url[0] = ConfigurationManager.AppSettings["ErrorMsgWindowUrl"];
            url[1] = ConfigurationManager.AppSettings["useInfoWindowUrl"];
            return url;
        }
    }    
}