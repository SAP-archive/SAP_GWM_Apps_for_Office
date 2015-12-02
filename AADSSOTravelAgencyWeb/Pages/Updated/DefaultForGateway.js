var id;
var status;

var stringMessages = {
    NoDecisionOptionsAvailable: "Decision options are not available for the request."
};

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
            var coll = "TaskCollection";
            //Gets the requested details
            $.ajax(
                {
                    type: "GET",
                    url: "DefaultForGateway.aspx/RequestTaskDetails",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: { "id": JSON.stringify(id), "coll": JSON.stringify(coll) },
                    success: function (result) {                        
                        if (result.d.statusMsg == 'OK') {
                            displayItemDetails(result.d.response);
                        }
                        else {
                            window.location = "https://localhost:44330/Pages/Updated/ErrorMessageWindow.aspx?errormsg=" + result.d.response[0];
                        }
                    },
                    error: function (xhr, status, error) {
                        window.location = "https://localhost:44330/Pages/Updated/ErrorMessageWindow.aspx?errormsg=" + xhr.statusText;
                    }
                }
                );
        });
    };

    //Gets decision options which can be applied to this requested type
    function getDecisionOptions() {
        $.ajax(
        {
            type: "GET",
            url: "DefaultForGateway.aspx/RequestDecisionOptions",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: { "id": JSON.stringify(id) },
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

    //If it has any decision options, create buttons for those
    function createButtons(buttons) {
        var btnCnt = buttons.length;
        var btn = [];
        var btnId = [];
        var btnNo = -1;

        if (btnCnt > 0) {
            var btnName = ['approvebutton','rejectbutton']; //This will be added as button ID. Since we have only 2 i have kept it as approve and reject button
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
                        'class':'decisionBtn',
                    }).appendTo('#actionbuttons');

                //adding button to <td> 
                    var td = tr.appendChild(document.createElement("td"));
                    var button = document.getElementById(btnId[btnNo]);
                    td.appendChild(button);

                    document.getElementById(btnId[btnNo]).textContent = btn[btnNo];
                    $('.decisionBtn').data(btnId[btnNo], buttons[i][1]); // adds the decision key. eg: 0001 , 0002 etc
                    document.getElementById(btnId[btnNo]).style.fontWeight = "bold";

                    if (status != 'COMPLETED') {
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
                postDecision(id, deckey); // make post call by passing decision key and instanceID

            })
        }
        else {
                //If no decision options are available, update the status message accordingly.
                document.getElementById('statusmsg').innerText = stringMessages.NoDecisionOptionsAvailable;
        }
    }

    //fetches csrf token and makes post call. on success returns json response with status
    function postDecision(incID, dec_Key) {
        $.ajax(
        {
            type: "GET",
            url: "DefaultForGateway.aspx/postTaskDecision",
            contentType: "application/json; charset=utf-8",
            data: { "dec_key": JSON.stringify(dec_Key), "incidentID": JSON.stringify(incID) },
            success: function (result) {
                if (result) {

                    if (result.d.statusMsg == 'OK') {
                        var json_obj = JSON.parse(result.d.postresponse);
                        var status = json_obj.d.Status;
                        displayStatusMessage(status, dec_Key);
                    }
                    else {
                        OnFailure(result.d.postresponse);
                    }
                }
            },
            error: function (xhr, status, error) {
                OnFailure(xhr.statusText);
            }
        });
    }

    // Displays the task details
    function displayItemDetails(TaskInfo) {
        status = TaskInfo[0];
        if (TaskInfo != null) {
            $('#username').text(TaskInfo[4]);
            $('#leavetype').text(TaskInfo[2]);
            $('#appinfo').text(TaskInfo[6]);
            $('#requestmsg').text('is requesting approval for');

            if (TaskInfo[5] == "TS92400454_WS92400745_0000000004") {
                $('#leaveduration').text(TaskInfo[9]);
                $('#fromdate').text(TaskInfo[7]);
                $('#todate').text(TaskInfo[8]);

                //Constant string text. Require Localization
                $('#fromdateheader').text('From:');
                $('#todateheader').text('To:');
            }

            //Styling user name, date headers and leave type &duration elements of HTML by ID
            var userName = document.getElementById('username');
            userName.style.fontWeight = 'bold';
            userName.style.fontSize = '200%';

            var fromHeader = document.getElementById('fromdateheader');
            fromHeader.style.fontSize = '150%';
            fromHeader.style.textAlign = "right";

            var toHeader = document.getElementById('todateheader');
            toHeader.style.fontSize = '150%';
            toHeader.style.textAlign = "right";

            var fromdate = document.getElementById('fromdate');
            fromdate.style.fontSize = '150%';

            var todate = document.getElementById('todate');
            todate.style.fontSize = '150%';

            var leaveType = document.getElementById('leavetype');
            leaveType.style.fontSize = '160%';

            var leaveDuration = document.getElementById('leaveduration');
            leaveDuration.style.fontWeight = 'bold';
            leaveDuration.style.fontSize = '160%';
            leaveDuration.style.color = '#990099';

            getDecisionOptions();
        }
    }
})();

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