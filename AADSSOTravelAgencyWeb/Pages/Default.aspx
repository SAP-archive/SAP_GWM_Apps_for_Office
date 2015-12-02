<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AADSSOTravelAgencyWeb.Pages.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <title></title>
    <script src="../../Scripts/jquery-1.9.1.js" type="text/javascript"></script>

    <link href="../../Content/Office.css" rel="stylesheet" type="text/css" />
    <script src="https://appsforoffice.microsoft.com/lib/1.1/hosted/office.js" type="text/javascript"></script>

    <!-- To enable offline debugging using a local reference to Office.js, use:                        -->
    <!-- <script src="../../Scripts/Office/MicrosoftAjax.js" type="text/javascript"></script>  -->
    <!-- <script src="../../Scripts/Office/1.1/office.js" type="text/javascript"></script>  -->

    <link href="../App.css" rel="stylesheet" type="text/css" />
    <script src="../App.js" type="text/javascript"></script>

    <link href="Home.css" rel="stylesheet" type="text/css" />
    <script src="Home.js" type="text/javascript"></script>
</head>
<body style="background-color:lightblue">
    <form id="form1" runat="server" >
       
    <asp:ScriptManager ID="ScriptMgr" runat="server" EnablePageMethods="true"></asp:ScriptManager>
        <br />
        <br />
         <div id="title" style="align-items:initial; font-size:20px;font-family:Arial">
                </div>
        <br />
        <br />
        <%--<button id="get-data">Get Data</button>--%>
        <table id="details" style="font-family:Arial; width:400px"> 
               
            <tr>
                </tr>
                <tr>                    
                    <td id="createdby" style="text-align:left; text-decoration:underline; color:blue"></td>                 
                    <td id="status" style="text-align:right"></td>
                </tr>
            <tr>
            </tr>
            <tr></tr>
                <tr>
                    <td id="createdon" style="text-align:left"></td>
                    <td id="priority" style="text-align:right"></td>
                </tr> 
            </table>

<%--        <button id="btn-info" ></button> <br />--%>
        
        <div id="dispInfo" style="font-family:Arial"></div>


        <div id="btnInfo" style="font-family:Arial"></div>
       <%-- <button id="opt1"  class ="decisionBtn"></button>  
        <button id="opt2" class="decisionBtn"></button>--%>
        
</form>


    <script type="text/javascript">
        var taskId, status;
        $(document).ready(function () {
            //app.initialize();
            Office.initialize = function (reason) {

                PageMethods.GetHostType(redirectToAuthenticate);
                var hostType = Office.context.mailbox.diagnostics.hostName;
                // get agency number from subject of email
                var item = Office.cast.item.toItemRead(Office.context.mailbox.item);
                var sub = item.subject;
                var temp = sub.split('#');
                taskId = temp[1].substring(0, 12);
                //$("#get-data").click(GetDetails);        

                
                
               
            };
        });

        
        function redirectToAuthenticate(hostType) {
            
            //alert("enter 1");
            PageMethods.GetAuthorizeUrl(function (value) {
               // alert("enter 2");
                if (value) {
                    if (hostType === "client")
                        window.open(value);
                    else
                        window.location = value;
                }

                GetDetails();
                
            });
        }

        //function DisplayDescription()
        //{
        //    PageMethods.GetDescription(taskId, function (value) {
        //        if (value) {
        //            $('#dispInfo').text(value);
        //        }
        //    });
        //}

        function GetDetails() {
            PageMethods.GetTaskDetails(taskId, function (value) {
                if (value) {
                    status = value[0];
                    $('#status').text(value[0]);
                    $('#title').text(value[1]);
                    $('#priority').text(value[2]);
                    $('#createdon').text('Created On ' + value[3]);
                    $('#createdby').text(value[4]);
                }

                if (status == 'READY' || status == 'RESERVED' || status == 'IN_PROGRESS' || status == 'EXECUTED') {
                PageMethods.GetDecisionOptions(taskId, function (value) {
                    if (value[0]) {

                        $('<button/>', {
                            'id': 'opt1',
                            'class': 'decisionBtn',
                        }).appendTo('#btnInfo');


                        $('#opt1').text(value[0][2]);
                        $('.decisionBtn').data("opt1", value[0][1] );
                    }
                    if (value[1]) {
                        
                        $('<button/>', {
                            'id': 'opt2',
                            'class': 'decisionBtn',
                        }).appendTo('#btnInfo');

                        $('#opt2').text(value[1][2]);
                        //$('#opt2').data("decisionKey", value[1][1]);
                        $('.decisionBtn').data("opt2", value[1][1]);
                    }

                    $('.decisionBtn').click(function () {
                        var id = $(this).attr('id');
                        var key = $('.decisionBtn').data(id);
                        //var key = $(this).attr('id').data("decisionKey");
                        var comments = "test";
                        PageMethods.PostDecision(taskId, key, comments, function () {                            
                                GetDetails(taskId);
                        });
                        
                    })
                });
               }


            });
            PageMethods.GetDescription(taskId, function (value) {
                if (value) {
                    $('#dispInfo').text(value);
                }
            });
            
            
        }

        $('.decisionBtn').click(function()
        {
            var id = $(this).attr('id');
            var key = $(id).data("decisionKey");
            var comments = "test";
            PageMethods.PostDecision(taskId, key, comments);
            GetDetails(taskId);
        })

</script>
</body>
</html>
