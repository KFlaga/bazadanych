using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        internal void ParseForeignKey(string column, string refTab, string refCol)
        {
            for (int col = 0; col < Columns.Count; col++)
            {
                if (Columns[col].Name == column)
                {
                    Columns[col].SetForeignKey(refTab, refCol);
                }
            }
        }

        internal void ResolveForeignKeys(List<TableSchema> tableSchemas)
        {
            for (int col = 0; col < Columns.Count; col++)
            {
                if (Columns[col].IsForeignKey)
                    Columns[col].ResolveForeignKey(tableSchemas);
            }
        }

        internal void ParseConstraint(string column, string constraint)
        {
            if (Regex.IsMatch(constraint, ".* IN .*")) // odfiltrowanie NOT NULLi
            {
                Regex valsPat = new Regex("\\((.*)\\)");
                if( valsPat.IsMatch(constraint))
                {
                    string vals = valsPat.Match(constraint).Groups[1].Value; // zostaje 'val1','val2','val3'
                    for (int col = 0; col < Columns.Count; col++)
                    {
                        if (Columns[col].Name == column)
                            Columns[col].SetValConstraints(vals);
                    }       
                }
            }
        }

        public ColumnSchema FindColumn(string name)
        {
            foreach (ColumnSchema column in Columns)
            {
                if (column.Name.Equals(name,StringComparison.OrdinalIgnoreCase))
                    return column;
            }
            return null;
        }
    }
}
