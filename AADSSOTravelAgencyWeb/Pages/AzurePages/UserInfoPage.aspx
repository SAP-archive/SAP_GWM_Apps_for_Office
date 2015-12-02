<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserInfoPage.aspx.cs" Inherits="SAP.AppsForOffice.Workflow.Pages.AzurePages.UserInfoPage" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>    
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <title></title>
    <script src="../../Scripts/jquery-1.9.1.js" type="text/javascript"></script>

    <link href="../../Content/Office.css" rel="stylesheet" type="text/css" />
    

    <link href="../../AppRead/App.css" rel="stylesheet" type="text/css" />
    <script src="../../AppRead/App.js" type="text/javascript"></script>

    <link href="../Common/Default.css" rel="stylesheet" type="text/css" />
    <script src="UserInfo.js" type="text/javascript"></script>
</head>
<body id="bodyStyle">
    <div>
        <!--SAP Logo-->
        <img id="imageStyle" src="../../Images/SAP_Icon.png"/>
    </div>
    <div id="content-main">
        <div class="padding">
            <table id="details" runat="server">                
                <tr>
                    <td id="username" runat="server"></td>
                </tr>
                <tr>
                    <td id="requestmsg" runat="server"></td>
                </tr>
                <tr>
                    <td id="leavetype" runat="server"></td>
                </tr>
                <tr>
                    <td id="leaveduration" runat="server"></td>
                </tr>
                <tr>
                    <td>
                        <table id="info" runat="server">
                            <tr>
                                <th id="fromdateheader" runat="server"></th>
                                <td id="fromdate" runat="server"></td>
                            </tr>
                            <tr>
                                <th id="todateheader" runat="server"></th>
                                <td id="todate" runat="server"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <p id="appinfo" runat="server"></p>
            <p id="statusmsg" runat="server"></p>
            <table id="actionbuttons">
                <tr id ="buttonrow">

                </tr>
            </table>
        </div>
    </div>
</body>
</html>
