using System;
using Npgsql;
using Dapper;

namespace kERP.SM
{
    class Branch
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public String Status { get; set; }
        public string Insert_By { get; set; }
        public DateTime? Insert_At { get; set; }
        public string Change_By { get; set; }
        public DateTime? Change_At { get; set; }
    }
}
