using System.ComponentModel.DataAnnotations;

namespace DocaLabs.Storage.Integration.Tests._Repositories._FederatedPartitionProvider
{
    public class Customer
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
