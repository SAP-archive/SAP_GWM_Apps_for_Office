//using AADSSOTravelAgencyWeb.Pages.Updated;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using SAP.IW.GWPAM.Common;
using SAP.IW.SSO;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace SAP.AppsForOffice.Workflow.Utilities
{
    public static class DataGetter
    {
        private static readonly string authType = ConfigurationManager.AppSettings["AuthenticatoinType"];
       
        /// <summary>
        /// Provides the collection data parsed in TaskDetailsDataModel, requested from TaskProcessingService V2
        /// requires service url & bearer token
        /// returns data in string array
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string[] GetDataMatrix(string requestUrl, string accessToken)
        {
            var jsonResult = GetJsonString(requestUrl, accessToken);
            var jsonValue = JObject.Parse(jsonResult)["d"];
            var dataCol = jsonValue.ToObject<TaskDetailsDataModel>();

            return new string[] { 
                    dataCol.Status.ToString(), 
                    dataCol.ProcessorName, 
                    dataCol.TaskDefinitionName, 
                    dataCol.CreatedOn,
                    dataCol.CreatedByName,
                    dataCol.TaskDefinitionID
            };
        }
        
        /// <summary>
        /// Provides the collection data parsed in TaskDetailsDataModel, requested from TaskProcessingService V2
        /// requires service url
        /// returns data in string array
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string[] GetDataMatrix(string requestUrl)
        {
            var jsonResult = GetJsonString(requestUrl);
            var jsonValue = JObject.Parse(jsonResult)["d"];
            var dataCol = jsonValue.ToObject<TaskDetailsDataModel>();

            return new string[] { 
                    dataCol.Status.ToString(), 
                    dataCol.ProcessorName, 
                    dataCol.TaskDefinitionName, 
                    dataCol.CreatedOn,
                    dataCol.CreatedByName,
                    dataCol.TaskDefinitionID
            };
        }

        /// <summary>
        /// Gets custom attributes i.e leave start, end day
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public static RootObject GetCustomAttributes(string requestUrl)
        {
            RootObject dataCol = null;
            var jsonResult = GetJsonString(requestUrl);
            string source = WebUtility.HtmlDecode(jsonResult);            
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(source);

            foreach (var item in doc.DocumentNode.Descendants())
            {
                if (item.XPath == "/html[1]/body[1]/div[2]/div[1]/code[1]/#text[1]")
                {
                    var jsonValue = JObject.Parse(item.InnerText)["d"];
                    dataCol = jsonValue.ToObject<RootObject>();            
                    break;
                }
            }            
            return dataCol;
        }
       
        /// <summary>
        /// Gets custom attributes i.e leave start, end day
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public static RootObject GetCustomAttributes(string requestUrl, string token)
        {
            RootObject dataCol = null;
            var jsonResult = GetJsonString(requestUrl, token);
            /*
            Use var jsonValue = JObject.Parse(jsonResult)["d"]["results"]
            if the response is in JSON
            */
            string source = WebUtility.HtmlDecode(jsonResult);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(source);

            foreach (var item in doc.DocumentNode.Descendants())
            {
                if (item.XPath == "/html[1]/body[1]/div[2]/div[1]/code[1]/#text[1]")
                {
                    var jsonValue = JObject.Parse(item.InnerText)["d"];
                    dataCol = jsonValue.ToObject<RootObject>();
                    break;
                }
            }
            return dataCol;
        }
        
        /// <summary>
        /// Provides the decision options of TaskCOllection retrieved from TaskProcessingService V2
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string[][] GetDecisionOptions(string requestUrl)
        {
            var jsonResult = GetJsonString(requestUrl);
            var jsonValue = JObject.Parse(jsonResult)["d"]["results"];
            var decisions = jsonValue.ToObject<List<DecisionOptionsModel>>();
            return decisions.Select((item) =>
            {
                return new string[] { 
                item.InstanceID,
                item.DecisionKey,
                item.DecisionText,
                item.CommentMandatory,
                item.Nature
            };
            }).ToArray();
        }

        /// <summary>
        /// Provides the description details of TaskCollection retrieved from TaskProcessingService V2
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string GetDescription(string requestUrl, string accessToken)
        {
            var jsonResult = GetJsonString(requestUrl, accessToken);
            var jsonValue = JObject.Parse(jsonResult)["d"]["Description"];
            var description = jsonValue.ToObject<string>();
            return description;
        }

        /// <summary>
        /// Provides the description details of TaskCollection retrieved from TaskProcessingService V2
        /// </summary>
        /// <param name="requestUrl"></param>        
        /// <returns></returns>
        public static string GetDescription(string requestUrl)
        {
            var jsonResult = GetJsonString(requestUrl);
            var jsonValue = JObject.Parse(jsonResult)["d"]["Description"];
            var description = jsonValue.ToObject<string>();
            return description;
        }

        /// <summary>
        /// Provides the decision options of TaskCOllection retrieved from TaskProcessingService V2
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string[][] GetDecisionOptions(string requestUrl, string accessToken)
        {
            var jsonResult = GetJsonString(requestUrl, accessToken);
            var jsonValue = JObject.Parse(jsonResult)["d"]["results"];
            var decisions = jsonValue.ToObject<List<DecisionOptionsModel>>();
            return decisions.Select((item) =>
            {
                return new string[] { 
                item.InstanceID,
                item.DecisionKey,
                item.DecisionText,
                item.CommentMandatory,
                item.Nature
            };
            }).ToArray();
        }
       
        /// <summary>
        /// Returns jasonstring result by making web call task processing service url 
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private static string GetJsonString(string requestUrl, string accessToken)
        {
            if (string.IsNullOrEmpty(requestUrl))
                return string.Empty;
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;                
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                return client.DownloadString(requestUrl);
            }
        }
        
        /// <summary>
        /// Returns jasonstring result by making web call task processing service url 
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private static string GetJsonString(string requestUrl)
        {
            if (string.IsNullOrEmpty(requestUrl))
                return string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "GET";
            request.Accept = "application/json";
            ISSOProvider ssoProvider;
            SystemTypes systemType = SystemTypes.Gateway;
            AuthenticationType authenticationType = (AuthenticationType)Enum.Parse(typeof(AuthenticationType), authType);
            ssoProvider = SSOProviderFactory.Instance.GetSSOProvider(authenticationType, request.Method, "", "", "", false, requestUrl, systemType);   //requestUrl is string                        
            switch (authenticationType)
            {
                case AuthenticationType.BASIC:
                    request.Credentials = new System.Net.NetworkCredential(Constant.SERVICE_USER, Constant.SERVICE_PSWD);
                    break;
            }
            ssoProvider.SAPCredientials(ref request);
            string text;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }
            return text;
        }

        /// <summary>
        /// Makes the POST call on the task processing service
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string PostJsonString(string requestUrl, string accessToken)
        {
            string response = "";            
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + accessToken;
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                response = client.UploadString(requestUrl, "POST");
            }
            return response;
        }
    }
}