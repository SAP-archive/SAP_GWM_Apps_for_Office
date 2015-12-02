using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP.AppsForOffice.Workflow.Utilities
{
    /// <summary>
    /// Task data model
    /// </summary>
    public class TaskDetailsDataModel
    {
        public string ProcessorName;
        public string TaskDefinitionName;
        public string CreatedOn;
        public string Status;        
        public string CreatedByName;
        public string TaskDefinitionID;
    }
    /// <summary>
    /// Decision data model
    /// </summary>
    public class DecisionOptionsModel
    {
        public string InstanceID;
        public string DecisionKey;
        public string DecisionText;
        public string CommentMandatory;
        public string Nature;

    }
    /// <summary>
    /// Data model for custom attribute
    /// </summary>
    public class CustomAttributeDataModel
    {
        public string __metadata;
        public string SAP__Origin;
        public string InstanceID;
        public string Name;
        public string Value;

    }
    public class Metadata
    {
        public string id { get; set; }
        public string uri { get; set; }
        public string type { get; set; }
    }

    public class Result
    {
        public Metadata __metadata { get; set; }
        public string SAP__Origin { get; set; }
        public string InstanceID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class RootObject
    {
        public List<Result> results { get; set; }
    }

}