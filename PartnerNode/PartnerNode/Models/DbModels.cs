using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartnerNode.Models
{
    public class DbDataAccount {

        public int Id { get;set; }
        public string Account { get;set; }
        public string FileKey { get;set; }
        public string ContractAddress { get;set; }
    }
}
