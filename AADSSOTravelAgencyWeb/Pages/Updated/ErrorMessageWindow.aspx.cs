using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AADSSOTravelAgencyWeb.Pages.Updated
{
    public partial class ErrorMessageWindow : System.Web.UI.Page
    {
        /// <summary>
        /// on load of page, assigns error message to HTML id
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string message = Request.QueryString["errormsg"];
            errormsg.InnerText = message;
        }
    }
}