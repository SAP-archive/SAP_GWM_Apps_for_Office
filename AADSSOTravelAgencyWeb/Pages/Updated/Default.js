var id;
var coll;
var isReady;
var mHostType;
var mAuthUrl;
var mUserInfo;
(function () {
    "use strict";

    // The Office initialize function must be run each time a new page is loaded
    Office.initialize = function (reason) {
        $(document).ready(function () {
            app.initialize();


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
                        window.location = "https://localhost:44330/Pages/Updated/ErrorMessageWindow.aspx?errormsg=" + error;
                    },
                    async: false
                });

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
                       window.location = "https://localhost:44330/Pages/Updated/ErrorMessageWindow.aspx?errormsg=" + error;
                   },
                   async: false
               });

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
                      window.location = "https://localhost:44330/Pages/Updated/ErrorMessageWindow.aspx?errormsg=" + error;
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

     
    function GetDetails()    
    {
        /// <summary>
        /// Gets the task details from TaskProcessing Service     
        /// </summary>
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
                           window.location = "https://localhost:44330/Pages/Updated/ErrorMessageWindow.aspx?errormsg=" + mUserInfo.response[0];
                       }
                   },
                   error: function (xhr, status, error) {
                       window.location = "https://localhost:44330/Pages/Updated/ErrorMessageWindow.aspx?errormsg=" + error;
                   },
                   async: false
               }

               );
    }    

    // Displays the task details
    function displayItemDetails(UserInfo) {        
        if (UserInfo != null) {
            var userData = UserInfo[0] + "," + UserInfo[1] + "," + UserInfo[2] + "," + UserInfo[3] + "," + UserInfo[4] + "," + UserInfo[5];
            var customAttr = UserInfo[7] + "," + UserInfo[8] + "," + UserInfo[9];
            window.location = "https://localhost:44330/Pages/Updated/UserInfoPage.aspx?userinfo="+ userData + "&id=" + id + "&description=" + UserInfo[6] + "&custom=" + customAttr;           
        }        
    }
})();
