var id;
var coll;
var isReady;
var mHostType;
var mAuthUrl;
var mUserInfo;

var windowUrl = {
    errWindow: "",
    userInfoWindow: ""
};
(function () {
    "use strict";

    /// <summary>
    /// The Office initialize function must be run each time a new page is loaded
    /// </summary>
    /// <param name="reason"></param>
    Office.initialize = function (reason) {
        $(document).ready(function () {
            app.initialize();
            getWindowURL();

            // get id from subject of email
            var item = Office.cast.item.toItemRead(Office.context.mailbox.item);
            var sub = item.subject;
            var temp = sub.split('#');
            var idIndex = temp[1].indexOf(' ');
            if (idIndex > 0) {
                id = temp[1].substring(0, idIndex);
            }
            else {
                id = temp[1];
            }

            coll = "TaskCollection";

            //Azure call to get the Host type value
            //On failure show the error message
            $.ajax(
                {
                    type: "GET",
                    url: "Default.aspx/GetHostType",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",                                        
                    success: function (result) {                                                
                        mHostType = result.d;

                    },
                    error: function (xhr, status, error) {
                        window.location = windowUrl.errWindow + "?errormsg=" + error;
                    },
                    async: false
                });

            //Azure call to get the Authorized URL value
            //On failure show the error message
            $.ajax(
               {
                   type: "GET",
                   url: "Default.aspx/GetAuthorizeUrl",
                   contentType: "application/json; charset=utf-8",
                   dataType: "json",
                   data: "",
                   success: function (result) {
                       if (result.d) {
                           if (mHostType === "client") {
                               window.open(result.d);
                           }

                           else {
                               window.location = result.d;                               
                           }
                       };                                              
                   },
                   error: function (xhr, status, error) {
                       window.location = windowUrl.errWindow + "?errormsg=" + error;
                   },
                   async: false
               });

            //Azure call to get the Check code 
            var myInterval = setInterval(function () {
              $.ajax(
              {
                  type: "GET",
                  url: "Default.aspx/CheckCode",
                  contentType: "application/json; charset=utf-8",
                  dataType: "json",
                  success: function (result) {
                      status = result.d;
                  },
                  error: function (xhr, status, error) {
                      window.location = windowUrl.errWindow + "?errormsg=" + error;
                  },
                  async: false
              }

              );            
              if (status == "Finished") {                
                    clearInterval(myInterval);
                    //got auth code now
                    GetDetails();
                }
            }, 1000);
        });
    };

     
    /// <summary>
    /// Gets the task details from TaskProcessing Service
    /// On failure Display error message 
    /// </summary>
    function GetDetails()    
    {
        $.ajax(
               {                   
                   type: "GET",
                   url: "Default.aspx/GetTaskDetails",
                   contentType: "application/json; charset=utf-8",
                   dataType: "json",                   
                   data: { "id": JSON.stringify(id), "coll": JSON.stringify(coll) },                   
                   success: function (result) {                       
                       mUserInfo = result.d;
                       if (result.d.statusMsg == 'OK') {
                           displayItemDetails(mUserInfo.response);
                       }
                       else {
                           window.location = windowUrl.errWindow + "?errormsg=" + mUserInfo.response[0];
                       }
                   },
                   error: function (xhr, status, error) {
                       window.location = windowUrl.errWindow + "?errormsg=" + error;
                   },
                   async: false
               }

               );
    }    

    /// <summary>
    /// Displays the task details
    /// </summary>
    /// <param name="UserInfo">Contains User information which needs to be displayed like: Requester Name, Leave type etc</param>
    function displayItemDetails(UserInfo) {        
        if (UserInfo != null) {
            var userData = UserInfo[0] + "," + UserInfo[1] + "," + UserInfo[2] + "," + UserInfo[3] + "," + UserInfo[4] + "," + UserInfo[5];
            var customAttr = UserInfo[7] + "," + UserInfo[8] + "," + UserInfo[9];
            window.location = windowUrl.userInfoWindow + "?userinfo=" + userData + "&id=" + id + "&description=" + UserInfo[6] + "&custom=" + customAttr + "&errWindowUrl=" + windowUrl.errWindow;
        }        
    }

    /// <summary>
    /// Gets the window url from web.config, to display error msg on.
    /// On failure Display error message 
    /// </summary>
    function getWindowURL() {
        $.ajax(
        {
            type: "GET",
            url: "Default.aspx/GetWindowUrl",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result) {
                    windowUrl.errWindow = result.d[0];
                    windowUrl.userInfoWindow = result.d[1];
                }
            },
            error: function (xhr, status, error) {
                OnFailure(xhr.statusText);
            }
        });
    }
})();
