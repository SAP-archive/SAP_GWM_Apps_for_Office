var id;
var userinfo;

var stringMessages = {
    NoDecisionOptionsAvailable: "Decision options are not available for the request."
};

$(document).ready(function () {
    id = getParameterByName('id');
    userinfo = getParameterByName('userinfo').split(',');
    //Constant string text. Require Localization
    $('#requestmsg').text('is requesting approval for');

    if (userinfo[5] == "TS21500003") {
        $('#fromdateheader').text('From:');
        $('#todateheader').text('To:');
    }
    ////Styling user name, date headers and leave type &duration elements of HTML by ID
    var userName = document.getElementById('username');
    userName.style.fontWeight = 'bold';
    userName.style.fontSize = '200%';

    document.getElementById('statusmsg').style.textAlign = "center";

    var fromHeader = document.getElementById('fromdateheader');
    fromHeader.style.textAlign = "right";

    var toHeader = document.getElementById('todateheader');
    toHeader.style.textAlign = "right";

    var leaveType = document.getElementById('leavetype');
    leaveType.style.fontSize = '160%';

    var leaveDuration = document.getElementById('leaveduration');
    leaveDuration.style.fontWeight = 'bold';
    leaveDuration.style.fontSize = '160%';
    leaveDuration.style.color = '#990099';                

    getDecisionOptions();
});

//Gets QueryString value by parameter name
function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

//Gets decision options for the requested type
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
                window.location = "https://localhost:44330/Pages/Updated/ErrorMessageWindow.aspx?errormsg=" + result.d.response[0];
            }
        },
        error: function (xhr, status, error) {
            window.location = "https://localhost:44330/Pages/Updated/ErrorMessageWindow.aspx?errormsg=" + error;
        }
    }

    );
}

//If decisions are available , creates buttons for the same
function createButtons(buttons) {
    var btnCnt = buttons.length;
    var btn = [];
    var btnId = [];
    var btnNo = -1;

    if (btnCnt > 0) {
        var btnName = ['approvebutton', 'rejectbutton']; //This will be added as button ID. Since we have only 2 i have kept it as approve and reject button
        //Retained the button ID (HTMl ID for each button) as approvebutton and rejectbutton. otherwise styling has to be changed(on how to decide on button colors) 

        var tr = document.getElementById('buttonrow');

        // creats new buttons and adds style to it. Makes it visible only if status != completed.
        for (var i = 0; i < btnCnt; i++) {
            
            if ((buttons[i][2].indexOf("Approve") >= 0)||(buttons[i][2].indexOf("Reject")>=0))
            {
                btnNo++;
                btnId.push(btnName[btnNo]); // populating array with button names
            btn.push(buttons[i][2]); // populating array with decision options

            $('<button/>', {
                'id': btnId[btnNo],
                'class': 'decisionBtn',
            }).appendTo('#actionbuttons');

            //adding button to <td> 
            var td = tr.appendChild(document.createElement("td"));
            var button = document.getElementById(btnId[btnNo]);
            td.appendChild(button);

            document.getElementById(btnId[btnNo]).textContent = btn[btnNo];
            $('.decisionBtn').data(btnId[btnNo], buttons[i][1]); // adds the decision key. eg: 0001 , 0002 etc
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

//Approve event handler
function onButtonClick(incID, dec_Key) {
    var test = 1;
    var token = "";

    $.ajax(
               {
                   type: "POST",
                   url: "../Updated/Default.aspx/PostDecision",
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

//Displays status message on UI
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

//Shows failure message on UI
function OnFailure(errorMsg) {
    document.getElementById('statusmsg').innerHTML = '<img src="../../Images/decline.png"/>' + errorMsg;
                       document.getElementById('statusmsg').style.color = "#B40404";
                       document.getElementById('statusmsg').style.textAlign = "center";
                   }

