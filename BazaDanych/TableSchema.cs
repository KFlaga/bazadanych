using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanych
{
    public class TableSchema
    {
        public string Name { get; set; }
        public List<ColumnSchema> Columns { get; set; }

        // Prawa zalogowanego właśnie użytkownika do tej tabeli
        public bool CanSelect { get; private set; }
        public bool CanInsert { get; private set; }
        public bool CanUpdate { get; private set; }
        public bool CanDelete { get; private set; }

        public string Owner { get; set; }

        public TableSchema()
        {
            Columns = new List<ColumnSchema>();
            CanDelete = false;
            CanSelect = false;
            CanUpdate = false;
            CanInsert = false;
        }

        public override string ToString()
        {
            return Name;
        }


        internal void GrantPrivilege(string priv)
        {
            if (priv == "SELECT")
                CanSelect = true;
            else if (priv == "INSERT")
                CanInsert = true;
            else if (priv == "UPDATE")
                CanUpdate = true;
            else if (priv == "DELETE")
                CanDelete = true;
        }
    }
}
