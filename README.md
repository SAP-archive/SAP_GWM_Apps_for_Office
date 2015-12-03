

SAP Apps for Office Workflow Application
========================================

**Overview **
=============

SAP Workflow Apps for Office Application helps to retrieve work item details (ex: leave requests, Travel Requests, Purchase Order Requests….etc.) from SAP Backend and update (Approve\Reject) request using Task Gateway Service V2, which is THE central workflow service that is used by plenty of (Fiori) applications, for example [SAP FIori - MyInbox](http://scn.sap.com/docs/DOC-62602).
This means that if you have already the SAP Fiori - My Inbox running, you can now easily also bring your workflows to Outlook. For details refer: Task Gateway Service Version 2.

This application supports 2 ways of accessing the TaskProcessingV2.
1.	Directly from Gateway (Default).
2.	Through Azure via Provider Systems configured in [GWM Azure Portal](http://help.sap.com/saphelp_nwgwpam_1/helpdata/en/f6/4aea7b57d441e9a5172193ac50b4dc/content.htm).

This application focuses on 2 potential scenarios.
1.	Any sample Workflow in the SAP Backend system - Showcases how you can take any workflow that you want to build or that you might already have in your system and expose it to Outlook.
2.	SAP Standard Travel request - Showcases how you can use existing out of the box SAP workflow services and get them to Outlook.

***Note:*** ***Sample Workflow in this application can be considered as the Leave request workflow.***

**Getting Started**
===============

Pre-requisites:
---------------
1. Operating System - Windows 7 and Windows 8.
2. Microsoft Visual Studio 2013 Editions - Professional, Premium, Ultimate and Community.
3.  .Net Framework 4.5.
4. Microsoft Office Professional 2013 and Microsoft Office 365 (Office Online).
5. **Required Library Files**-
The following Dynamic Link Library files (with the extension .dll) should be present in the Global Assembly Cache (GAC) folder for installation:
	1. Newtonsoft.Json.dll (version 5.0.6)
	2. Microsoft.IdentityModel.Clients.ActiveDirectory.dll (version 1.0.2)
	3. Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll (version 1.0.2)
	4. Newtonsoft.Json.dll is available in the Json.Net 5.0.6 package,
	5. Microsoft.IdentityModel.Clients.ActiveDirectory.dll and
	6. Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll are available in the Active Directory Authentication Library 1.0.2 package.
These packages can be downloaded at https://www.nuget.org/ with a registered account. The downloaded files can be extracted to view the DLL files in the lib folder based on the .NET Framework version. 
Follow these steps to add the required DLL files to the GAC folder:
		1. Open Visual Studio Tools on the Start menu, right-click on Developer Command Prompt for VS2013 or Developer Command Prompt for VS2010 (depending on the version of Visual Studio being used) and choose Run as Administrator , choose Yes if Windows displays a security prompt.
		2. In Developer Command Prompt , run the command gacutil -if <file path of DLL> and press Enter. Repeat this step till all the required DLL files are added successfully.
6. **SAP Gateway for Microsoft (GWM)** Solution.

Steps to run Apps for Office Workflow Solution
----------------------------------------------

***Note: You need to have a workflow and a related Taskprocessing service configured and accessible.***

 - Download the SAP.Workflow.AppsforOfficeApplication code from Github.
 - Extract the file.
 - Open SAP.Workflow.AppsforOfficeApplication.sln file.
 - Build the project (You need to add the above mentioned library files to GAC to build the project successfully).
 - Deploy the solution.
 - Run the project.


Documentation:
==============

**You can find details of this Application and step by step instructions in the SCN Blog.**