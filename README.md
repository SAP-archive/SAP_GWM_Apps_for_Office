


**

Overview Of SAP Apps for Office Workflow Application
====================================================

SAP Gateway for Microsoft (GWM) is a product that simplifies the integration of SAP Gateway based services into Microsoft products. GWM provides an interoperability layer that makes sure that applications can communicate in a secure, reliable and scalable manner with SAP backend systems.

The GWM add-in for Visual Studio provides an extensive OData browsing functionality, a set of templates that can be used to create Contact, Calendar or Workflow-based scenarios in Microsoft Outlook 2010 and 2013 and also provides a generic service generation for any Visual Studio C# project. For details refer: [SAP Gateway for Microsoft](http://scn.sap.com/docs/DOC-47563).

SAP Workflow Apps for Office Application helps to retrieve work item details (ex: leave requests, Travel Requests, Purchase Order Requests….etc.) from SAP Backend and update (Approve\Reject) request using Task Gateway Service V2, which is THE central workflow service that is used by plenty of (Fiori) applications, for example [SAP FIori - MyInbox](http://scn.sap.com/docs/DOC-62602).
This means that if you have already the SAP Fiori - My Inbox running, you can now easily also bring your workflows to Outlook. For details refer: Task Gateway Service Version 2.

This application supports 2 ways of accessing the TaskProcessingV2.
1.	Directly from Gateway (Default).
2.	Through Azure via Provider Systems configured in [GWM Azure Portal](http://help.sap.com/saphelp_nwgwpam_1/helpdata/en/f6/4aea7b57d441e9a5172193ac50b4dc/content.htm).

This application focuses on 2 potential scenarios.
1.	Any sample Workflow in the SAP Backend system - Showcases how you can take any workflow that you want to build or that you might already have in your system and expose it to Outlook.
2.	SAP Standard Travel request - Showcases how you can use existing out of the box SAP workflow services and get them to Outlook.
***Note:*** ***Sample Workflow in this application can be considered as the Leave request workflow.***
