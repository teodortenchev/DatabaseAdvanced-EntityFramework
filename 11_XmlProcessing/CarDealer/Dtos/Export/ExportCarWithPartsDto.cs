using System.Collections.Generic;
using System.Xml.Serialization;

namespace CarDealer.Dtos.Export
{
    [XmlType("car")]
    public class ExportCarWithPartsDto : ExportCarWithMakeDto
    {
        [XmlArray("parts")]
        public List<PartDto> Parts { get; set; }
    }
}
