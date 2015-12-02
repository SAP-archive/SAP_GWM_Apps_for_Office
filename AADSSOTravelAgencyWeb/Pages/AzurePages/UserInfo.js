var id;
var userinfo;
var errWindowUrl;

var stringMessages = {
    NoDecisionOptionsAvailable: "Decision options are not available for the request."
};

/// <summary>
/// Funtion gets called on the UserInfo page load
/// </summary>
$(document).ready(function () {
    id = getParameterByName('id');
    errWindowUrl = getParameterByName('errWindowUrl');
    userinfo = getParameterByName('userinfo').split(',');
    //Constant string text. Require Localization
    $('#requestmsg').text('is requesting approval for');

    ////Styling user name, date headers and leave type &duration elements of HTML by ID
    var userName = document.getElementById('username');
    userName.style.fontWeight = 'bold';
    userName.style.fontSize = '200%';

    document.getElementById('statusmsg').style.textAlign = "center";

    var leaveType = document.getElementById('leavetype');
    leaveType.style.fontSize = '160%';

    getDecisionOptions();
});

/// <summary>
/// Gets QueryString value by parameter name
/// </summary>
/// <param name="name">Parameter name</param>
/// <returns>Returns the value for the Parameter passed</returns>
function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

/// <summary>
/// Gets decision options for the requested type
/// On failure Show error message
/// </summary>
function getDecisionOptions() {
    $.ajax(
    {
        type: "GET",
        url: "UserInfoPage.aspx/GetDecisionOptions",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: { "taskId": JSON.stringify(id) },
        success: function (result) {
            if (result.d.statusMsg == 'OK') {
                createButtons(result.d.decisionOptions);
            }
            else {
                window.location = errWindowUrl + "?errormsg=" + result.d.response[0];
            }
        },
        error: function (xhr, status, error) {
            window.location = errWindowUrl + "?errormsg=" + error;
        }
    }

    );
}

/// <summary>
/// If decision options are available , creates buttons for the same
/// </summary>
/// <param name="decisionOptions">Decision Options</param>
function createButtons(decisionOptions) {
    var btnCnt = decisionOptions.length;
    var btn = [];
    var btnId = [];
    var btnNo = -1;

    if (btnCnt > 0) {

        //This will be added as button ID. Since we have only 2 i have kept it as approve and reject button
        //Retained the button ID (HTMl ID for each button) as approvebutton and rejectbutton. otherwise styling has to be changed(on how to decide on button colors)
        var btnName = ['approvebutton', 'rejectbutton']; 

        var tr = document.getElementById('buttonrow');

        // creats new buttons and adds style to it. Makes it visible only if status != completed.
        for (var i = 0; i < btnCnt; i++) {
            
            if ((decisionOptions[i][2].indexOf("Approve") >= 0) || (decisionOptions[i][2].indexOf("Reject") >= 0))
            {
                btnNo++;
                btnId.push(btnName[btnNo]); // populating array with button names
                btn.push(decisionOptions[i][2]); // populating array with decision options

            $('<button/>', {
                'id': btnId[btnNo],
                'class': 'decisionBtn',
            }).appendTo('#actionbuttons');

            //adding button to <td> 
            var td = tr.appendChild(document.createElement("td"));
            var button = document.getElementById(btnId[btnNo]);
            td.appendChild(button);

            document.getElementById(btnId[btnNo]).textContent = btn[btnNo];
            $('.decisionBtn').data(btnId[btnNo], decisionOptions[i][1]); // adds the decision key. eg: 0001 , 0002 etc
            document.getElementById(btnId[btnNo]).style.fontWeight = "bold";
            if (userinfo[0] != 'COMPLETED') {
                document.getElementById(btnId[btnNo]).style.visibility = "visible";
            }
            else {
                document.getElementById('statusmsg').innerHTML = '<img src="../../Images/Done.png" > Completed';
                document.getElementById('statusmsg').style.color = "#088A08";
                document.getElementById('statusmsg').style.textAlign = "center";
            }
        }
        }

        $('.decisionBtn').click(function () {
            var btnID = $(this).attr('id');
            var deckey = $('.decisionBtn').data(btnID);
            onButtonClick(id, deckey); // make post call by passing decision key and instanceID
        })
    }
    else {

        //If no decision options available update the status message accordingly.
        document.getElementById('statusmsg').innerText = stringMessages.NoDecisionOptionsAvailable;
    }
}

/// <summary>
/// Event handler for Approve & Reject buttons
/// </summary>
/// <param name="incID">Incident ID</param>
/// <param name="dec_Key">Decision Key</param>
function onButtonClick(incID, dec_Key) {
    var test = 1;
    var token = "";

    $.ajax(
               {
                   type: "POST",
                   url: "../AzurePages/Default.aspx/PostDecision",
                   contentType: "application/json; charset=utf-8",
                   dataType: "json",
                   data: '{decisionKey: "' + dec_Key + '", taskId: "' + incID + '"}',
                   success: function (result) {
                       if (result) {
                           if (result.d.statusMsg == 'OK') {
                               var json_obj = JSON.parse(result.d.postresponse);
                           var status = json_obj.d.Status;
                               displayStatusMessage(status,dec_Key);
                           }
                           else {
                               OnFailure(result.d.postresponse);
                            }
                       }
                   },
                   error: function (xhr, status, error) {
                       OnFailure(error);
                   }
               }
               );
}

/// <summary>
/// Displays status message on UI for Approval & Reject
/// on failure Displays error message
/// </summary>
/// <param name="status">Status of the request after updation</param>
/// <param name="decision_key">Decision Key</param>
function displayStatusMessage(status, decision_key) {
    if (status == "COMPLETED") {
        if (decision_key == "0001") {
            document.getElementById('statusmsg').innerHTML = '<img src="../../Images/Done.png" > Approved';
        }
        else if (decision_key == "0002" || decision_key == "0003") {
            document.getElementById('statusmsg').innerHTML = '<img src="../../Images/Done.png" > Rejected';
                       }
        document.getElementById('statusmsg').style.color = "#088A08";
                           document.getElementById('statusmsg').style.textAlign = "center";
                           document.getElementById('approvebutton').style.visibility = "hidden";
                           document.getElementById('rejectbutton').style.visibility = "hidden";
                       }
                       else {
        var errorMsg = "Could not process you response. Please try again";
        OnFailure(errorMsg)
    }
                       }

/// <summary>
/// Shows failure message on UI
/// </summary>
/// <param name="errorMsg">Error Message</param>
function OnFailure(errorMsg) {
    document.getElementById('statusmsg').innerHTML = '<img src="../../Images/decline.png"/>' + errorMsg;
                       document.getElementById('statusmsg').style.color = "#B40404";
                       document.getElementById('statusmsg').style.textAlign = "center";
                   }

