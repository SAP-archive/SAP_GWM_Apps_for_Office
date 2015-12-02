using SAP.AppsForOffice.Workflow.Pages.Common;
//using AADSSOTravelAgencyWeb.Pages.Updated;
using SAP.AppsForOffice.Workflow.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAP.AppsForOffice.Workflow.Pages.AzurePages
{
    public partial class UserInfoPage : System.Web.UI.Page
    {
        /// <summary>
        /// on page load, assigns the requested information on to HTLM IDs to display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string userinfo = Request.QueryString["userinfo"];
            string description = Request.QueryString["description"];
            string customAttr = Request.QueryString["custom"];
            string[] infos = userinfo.Split(',');
            string[] cusAttrs = customAttr.Split(',');

            appinfo.InnerText = description;
            username.InnerText = infos[4];
            leavetype.InnerText = infos[2];
            if (infos[5] == ConfigurationManager.AppSettings["AzureRequestType"])
            {
                leaveduration.InnerText = cusAttrs[2];
                fromdate.InnerText = cusAttrs[0];
                todate.InnerText = cusAttrs[1];
                fromdateheader.InnerText = "From:";
                todateheader.InnerText = "To:";
                fromdateheader.Style.Add("TextAlign", "right");
                todateheader.Style.Add("TextAlign", "right");
                leaveduration.Style.Add("fontWeight", "bold");
                leaveduration.Style.Add("fontSize", "160%");
                leaveduration.Style.Add("color", "#990099");
            }
            if(cusAttrs[0]==string.Empty)
            {
                fromdateheader.Visible = false;
                todateheader.Visible = false;
                fromdate.Visible = false;
                todate.Visible = false;
                leaveduration.Visible = false;
            }
        }


        


        /// <summary>
        /// Gets decision options
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static ResponseToClient GetDecisionOptions(string taskId)
        {
            ResponseToClient responseToClient = new ResponseToClient();
            try
            {
                var accessToken = AADAuthHelper.EnsureValidAccessToken(HttpContext.Current);
                responseToClient.decisionOptions = DataGetter.GetDecisionOptions(ConfigurationManager.AppSettings["AzureServiceUrl"] + "DecisionOptions?InstanceID='" + taskId + "'", accessToken);
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