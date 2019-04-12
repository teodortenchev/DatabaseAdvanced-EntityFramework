using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Customer")]
    public class ImportCustomerTicketDto
    {
        [XmlElement("FirstName")]
        [Required]
        [MinLength(3), MaxLength(20)]
        public string FirstName { get; set; }

        [XmlElement("LastName")]
        [Required]
        [MinLength(3), MaxLength(20)]
        public string LastName { get; set; }

        [XmlElement("Age")]
        [Range(typeof(int), "12", "110")]
        public int Age { get; set; }

        [XmlElement("Balance")]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Balance { get; set; }

        [XmlArray("Tickets")]
        public ImportTicketDto[] Tickets { get; set; }
    }

    [XmlType("Ticket")]
    public class ImportTicketDto
    {
        [XmlElement("ProjectionId")]
        public int ProjectionId { get; set; }

        [XmlElement("Price")]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }
    }
}
