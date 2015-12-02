using AADSSOTravelAgencyWeb.Utilities;
using SAP.IW.GWPAM.Common;
using SAP.IW.SSO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AADSSOTravelAgencyWeb.Pages.Updated
{
    public partial class DefaultForGateway : System.Web.UI.Page
    {

        private static readonly string authType = ConfigurationManager.AppSettings["AuthenticatoinType"];

        private static readonly string _serviceUrl = ConfigurationManager.AppSettings["GatewayServiceUrl"];
        private static readonly string _sapClient = ConfigurationManager.AppSettings["SapClient"];
        private static readonly string _sapOrigin = ConfigurationManager.AppSettings["SapOrigin"];

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public static string ReverseDate(string s)
        {
            string year = s.Substring(0,4);
            string month = s.Substring(5, 2);
            string date = s.Substring(8, 2);
            string returnString = date+"-"+month+"-"+year;
            return returnString;
            
        }

        private static string GetResourceUrl(string proprety, string id, bool hasOrigin, string collection = "", string deckey = "")
        {
            string result = "";
            switch (proprety)
            {
                case "base":
                    result = hasOrigin ? _serviceUrl + collection + "(SAP__Origin='" + _sapOrigin + "',InstanceID='" + id + "')" : _serviceUrl + collection + "('" + id + "')";
                    break;
                case "description":
                    result =hasOrigin? _serviceUrl + collection + "(SAP__Origin='" + _sapOrigin + "',InstanceID='" + id + "')/Description" : _serviceUrl + collection + "('" + id + "')/Description";
                    break;
                case "custom":
                    result = hasOrigin ? _serviceUrl + "TaskCollection" + "(SAP__Origin='" + _sapOrigin + "',InstanceID='" + id + "')/CustomAttributeData?sap-ds-debug=true" : _serviceUrl + "TaskCollection" + "('" + id + "')/CustomAttributeData?sap-ds-debug=true";
                    break;
                
                case "descionOptions":
                    result =hasOrigin ?  _serviceUrl + "DecisionOptions?sap-client=" + _sapClient + "&SAP__Origin='" + _sapOrigin + "'&InstanceID='" + id + "'" : _serviceUrl + "DecisionOptions?InstanceID='" + id + "'" ;//TODO
                    break;

                case "postDescion":
                    result = hasOrigin ? _serviceUrl + "Decision?sap-client=001&SAP__Origin='Q5K_004_TGW'&InstanceID='" + id + "'&DecisionKey='" + deckey + "'" : _serviceUrl + "Decision?InstanceID='" + id + "'&DecisionKey='" + deckey + "'";
                    break;
                
                default:
                    break;
            }
            return result;
        }

        /// <summary>
        /// Gets details for the selected InstanceId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="coll"></param>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(UseHttpGet=true)]
        public static ResponseToClient RequestTaskDetails(string id, string coll)
        {
            ResponseToClient responseToClient = new ResponseToClient();
            string[] dataList;
            bool hasOrigin = bool.Parse(ConfigurationManager.AppSettings["HasMultipleSapOrigin"].ToString());
            RootObject customAttributes;
            // The resource URL for NetWeaver Gateway
            string resourceUrl = GetResourceUrl("base",id, hasOrigin, coll);
            string descriptionUrl = GetResourceUrl("description", id, hasOrigin, coll);
            
            // The specific resource and options we need.
            string urlOptions = "?$format=json";
            resourceUrl = resourceUrl + urlOptions;

            try
            {
            dataList = DataGetter.GetDataMatrix(resourceUrl);
            string ldUrl = GetResourceUrl("custom", id, hasOrigin, coll);//CustomAttributeCollection
                customAttributes = DataGetter.GetCustomAttributes(ldUrl);

                responseToClient.response = new string[dataList.Length + 4];
                Array.Copy(dataList, 0, responseToClient.response, 0, dataList.Length);
                //gets description of the application
                var description = DataGetter.GetDescription(descriptionUrl);
                responseToClient.response[6] = description;

                if (dataList[5] == ConfigurationManager.AppSettings["GatewayRequestType"])
                {
                    var startObj = customAttributes.results.FirstOrDefault(x => x.Name == "/SWL/ALL/DYNCOL2");            
            string startDate = string.Format("{2}-{1}-{0}", startObj.Value.Substring(6, 2), startObj.Value.Substring(4, 2), startObj.Value.Substring(0, 4));
            DateTime sd = new DateTime();
            DateTime ed = new DateTime();
            sd = DateTime.ParseExact(startDate, "yyyy-MM-dd", null);
                    var endObj = customAttributes.results.FirstOrDefault(x => x.Name == "/SWL/ALL/DYNCOL3");            
                    string endDate = string.Format("{2}-{1}-{0}", endObj.Value.Substring(6, 2), endObj.Value.Substring(4, 2), endObj.Value.Substring(0, 4));
            ed = DateTime.ParseExact(endDate, "yyyy-MM-dd", null);
            var lduration = ed.Subtract(sd);
            string dayText = lduration.Days > 1 ? "Days" : "Day";
            string leaveDuration = lduration.Days.ToString() + " " + dayText;
                    responseToClient.response[7] = ReverseDate(startDate);
                    responseToClient.response[8] = ReverseDate(endDate);
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
        /// get csrf token
        /// </summary>
        /// <param name="dec_key"></param>
        /// <param name="incidentID"></param>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static ResponseToClient postTaskDecision(string dec_key, string incidentID)
        {
            ResponseToClient resp = new ResponseToClient();
            string XMLResponse = null;
            bool hasOrigin = bool.Parse(ConfigurationManager.AppSettings["HasMultipleSapOrigin"].ToString());
            try
            {
                string resourceUrl = GetResourceUrl("postDescion", incidentID, hasOrigin, " " , dec_key);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resourceUrl);
                request.AllowAutoRedirect = false;
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Set("x-requested-with", "JSONHttpRequest");
                request.Accept = "application/json";

                ISSOProvider ssoProvider;
                SystemTypes systemType = SystemTypes.Gateway;
                AuthenticationType authenticationType = (AuthenticationType)Enum.Parse(typeof(AuthenticationType), authType);
                ssoProvider = SSOProviderFactory.Instance.GetSSOProvider(authenticationType, request.Method, "", "", "", true, resourceUrl, systemType);   //requestUrl is string            
                switch (authenticationType)
                {
                    case AuthenticationType.BASIC:
                        request.Credentials = new System.Net.NetworkCredential(Constant.SERVICE_USER, Constant.SERVICE_PSWD);
                        break;
                }
                ssoProvider.SAPCredientials(ref request);

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    XMLResponse = sr.ReadToEnd();
                    resp.postresponse = XMLResponse;
                    resp.statusMsg = "OK";
                }
            }
            catch(WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                int statusCode = (int)res.StatusCode;
                resp.statusCode = statusCode;
                resp.postresponse = ex.Message;
                resp.statusMsg = "Exception";
            }
            return resp;
        }

        /// <summary>
        /// Gets decision options
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static ResponseToClient RequestDecisionOptions(string id)
        {
            bool hasOrigin = bool.Parse(ConfigurationManager.AppSettings["HasMultipleSapOrigin"].ToString());
            ResponseToClient responseToClient = new ResponseToClient();
            try
            {
                responseToClient.decisionOptions = DataGetter.GetDecisionOptions(GetResourceUrl("descionOptions", id, hasOrigin));
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
    }    
}