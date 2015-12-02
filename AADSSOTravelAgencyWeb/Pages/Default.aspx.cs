using AADSSOTravelAgencyWeb.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AADSSOTravelAgencyWeb.Pages
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AADAuthHelper.CurrentHostType = HttpContext.Current.Request.QueryString["_host_Info"];
                AADAuthHelper.StoreAuthorizationCodeFromRequest(this.Request);
            }

        }


        [WebMethod]
        public static string GetHostType()
        {
            return Utilities.AADAuthHelper.CurrentHostType;
        }

        [WebMethod]
        public static string GetAuthorizeUrl()
        {
            if (!AADAuthHelper.IsAuthorized)
            {
                return AADAuthHelper.AuthorizeUrl;
            }
            return string.Empty;
        }

        [WebMethod]
        public static string[] GetTaskDetails(string taskId)
        {
            var accessToken = AADAuthHelper.EnsureValidAccessToken(HttpContext.Current);
            var dataList = DataGetter.GetDataMatrix("http://gwmblr.cloudapp.net:8080/TestAppForOffice/sap/opu/odata/iwpgw/TASKPROCESSING;v=0002/TaskCollection('" + taskId + "')", accessToken);            
            return dataList;
        }

        [WebMethod]
        public static string GetDescription(string taskId)
        {
            var accessToken = AADAuthHelper.EnsureValidAccessToken(HttpContext.Current);
            var description = DataGetter.GetDescription("http://gwmblr.cloudapp.net:8080/TestAppForOffice/sap/opu/odata/iwpgw/TASKPROCESSING;v=0002/TaskCollection('" + taskId + "')/Description", accessToken);
            return description;
        }

        [WebMethod]
        public static string[][] GetDecisionOptions(string taskId)
        {
            var accessToken = AADAuthHelper.EnsureValidAccessToken(HttpContext.Current);
            var decisionList = DataGetter.GetDecisionOptions("http://gwmblr.cloudapp.net:8080/TestAppForOffice/sap/opu/odata/iwpgw/TASKPROCESSING;v=0002/DecisionOptions?InstanceID='" + taskId + "'", accessToken);
            return decisionList;
        }


        [WebMethod]
        public static void PostDecision(string taskId, string decisionKey, string comments)
        {
            var accessToken = AADAuthHelper.EnsureValidAccessToken(HttpContext.Current);
            DataGetter.PostJsonString("http://gwmblr.cloudapp.net:8080/TestAppForOffice/sap/opu/odata/iwpgw/TASKPROCESSING;v=0002/Decision?InstanceID='" + taskId + "'&DecisionKey='"+ decisionKey + "'&Comments='" + comments + "'", accessToken);
            return;
        }
    }
}