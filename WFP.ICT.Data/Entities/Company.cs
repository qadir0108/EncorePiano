using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class Company : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string  Name { get; set; }
        public string Details { get; set; }
        public string WebSite { get; set; }

        // AD Settings
        public string ActiveDiretoryDomain { get; set; }
        public string ActiveDiretoryUserName { get; set; }
        public string ActiveDiretoryPassword { get; set; }

        // UI Settings
        public string Logo { get; set; }
        public int Theme { get; set; } // Default = 1

        // Email Settings
        public string EmailServer { get; set; }
        public string EmailUserName { get; set; }
        public string EmailPassword { get; set; }

        public Company()
        {
        }
    }
}
