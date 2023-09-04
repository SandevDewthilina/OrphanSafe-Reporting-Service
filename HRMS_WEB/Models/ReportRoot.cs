using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HRMS_WEB.Models
{
    [XmlRoot("ReportRoot")]
    public class ReportRoot
    {
        [XmlAttribute]
        public String ReportName { get; set; }
        [XmlAttribute]
        public String FileName { get; set; }
        [XmlAttribute]
        public bool IsDynamicEnable { get; set; } = true;
        public Query Query { get; set; }
        [XmlElement("EParameter")]
        public List<EParameter> EParameters { get; set; }
    }

    public class Query
    {
        [XmlText]
        public String value { get; set; }
        [XmlAttribute]
        public String hasParams { get; set; }
    }

    public class EParameter
    {
        [XmlAttribute]
        public String type { get; set; }
        [XmlText]
        public String name { get; set; }
        [XmlAttribute]
        public String bindingName { get; set; }
        [XmlAttribute]
        public String query { get; set; }
    }
}
