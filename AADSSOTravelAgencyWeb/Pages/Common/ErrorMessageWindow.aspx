<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorMessageWindow.aspx.cs" Inherits="SAP.AppsForOffice.Workflow.Pages.Common.ErrorMessageWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <title></title>
    <script src="../../Scripts/jquery-1.9.1.js" type="text/javascript"></script>

    <link href="../../Content/Office.css" rel="stylesheet" type="text/css" />
    <%--<script src="https://appsforoffice.microsoft.com/lib/1.1/hosted/office.js" type="text/javascript"></script>--%>

    <link href="../../AppRead/App.css" rel="stylesheet" type="text/css" />
    <script src="../../AppRead/App.js" type="text/javascript"></script>

    <link href="../Common/Default.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div>
        <!--SAP Logo-->
        <img id="imageStyle" src="../../Images/SAP_Icon.png"/>
    </div>
    <p id="errormsg" runat="server"></p>
</body>
</html>
