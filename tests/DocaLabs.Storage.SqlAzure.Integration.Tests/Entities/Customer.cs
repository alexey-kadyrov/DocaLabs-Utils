using System.ComponentModel.DataAnnotations;
using DocaLabs.Storage.Core;

namespace DocaLabs.Storage.SqlAzure.Integration.Tests.Entities
{
    public class Customer : IEntity
    {
        [Key]
        public virtual long CustomerId { get; set; }

        public virtual string Title { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string MiddleName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Suffix { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string SalesPerson { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string Phone { get; set; }
    }
}
