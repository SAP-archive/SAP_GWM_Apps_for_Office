![](https://img.shields.io/badge/STATUS-NOT%20CURRENTLY%20MAINTAINED-red.svg?longCache=true&style=flat)

# Important Notice
We have decided to stop the maintenance of this public GitHub repository.


SAP Apps for Office Workflow Application
========================================

**Overview:**
=========

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
Refer to the [SAP Workflow Apps for Office Application](http://scn.sap.com/community/interoperability-microsoft/blog/2015/12/04/apps-for-office-workflow) SCN blog and [SAP Gateway for microsoft Installation and Configuration Guide](http://help.sap.com/downloads/pdf/saphelp_nwgwpam_1_en_53_8be0db450541e493d7b4c2e5685ecf_frameset.pdf) for the pre-requisites.

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

**You can find details of this Application and step by step instructions for running this application in the SCN Blog [SAP Workflow Apps for Office Application](http://scn.sap.com/community/interoperability-microsoft/blog/2015/12/04/apps-for-office-workflow).**
