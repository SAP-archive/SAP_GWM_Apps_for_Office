<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DefaultForGateway.aspx.cs" Inherits="SAP.AppsForOffice.Workflow.Pages.GatewayPages.DefaultForGateway" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>    
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <title></title>
    <script src="../../Scripts/jquery-1.9.1.js" type="text/javascript"></script>

    <link href="../../Content/Office.css" rel="stylesheet" type="text/css" />
    <script src="https://appsforoffice.microsoft.com/lib/1.1/hosted/office.js" type="text/javascript"></script>

    <link href="../../AppRead/App.css" rel="stylesheet" type="text/css" />
    <script src="../../AppRead/App.js" type="text/javascript"></script>

    <link href="../Common/Default.css" rel="stylesheet" type="text/css" />
    <script src="DefaultForGateway.js" type="text/javascript"></script>
</head>
<body id="bodyStyle">
    <div>
        <!--SAP Logo-->        
        <img id="imageStyle" src="../../Images/SAP_Icon.png"/>
    </div>
    <div id="content-main">
        <div class="padding">
            <table id="details">
                <tr>
                    <td id="username"></td>
                </tr>
                <tr>
                    <td id="requestmsg"></td>
                </tr>
                <tr>
                    <td id="leavetype"></td>
                </tr>
                <tr>
                    <td id="leaveduration"></td>
                </tr>
                <tr>
                    <td>
                        <table id="info">
                            <tr>
                                <th id="fromdateheader"></th>
                                <td id="fromdate"></td>
                            </tr>
                            <tr>
                                <th id="todateheader"></th>
                                <td id="todate"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <p id="appinfo"></p>
            <p id="statusmsg"></p>
            <%--<div id ="actionbuttons">

            </div>--%>
            <table id="actionbuttons">
                <tr id ="buttonrow">

                </tr>
            </table>
        </div>
    </div>
</body>
</html>
