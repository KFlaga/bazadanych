using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BazaDanych
{
    public class ColumnSchema
    {
        public string Name { get; set; }//private set; }
        public Type type { get; set; }//private set; }
        public int MaxSize { get; private set; }
        public bool IsNullable { get; private set; }
        public bool IsPrivateKey { get; private set; }
        public bool IsForeignKey { get; private set; }
        public TableSchema ReferenceTable { get; set; }
        public ColumnSchema ReferenceColumn { get; set; }
        public object DefaultValue { get; set; }
        public List<object> ValueConstraints { get; private set; }

        string referenceTableName;
        string referenceColumnName;

        public ColumnSchema()
        {
            ReferenceTable = null;
            ReferenceColumn = null;
            IsNullable = false;
            IsPrivateKey = false;
            IsForeignKey = false;
        }

        internal void ParseColumn(string name, string nullable, string typeStr, int size)
        {
            Name = name;
            if (nullable == "N")
                IsNullable = false;
            else
                IsNullable = true;

            if (typeStr.StartsWith("VARCHAR"))
            {
                type = Type.GetType("System.String");
            }
            else if (typeStr.StartsWith("NUMBER"))
            {
                if (size >= 64)
                {
                    type = Type.GetType("System.Int64");
                }
                else if (size >= 32)
                {
                    type = Type.GetType("System.Int32");
                }
                else
                {
                    type = Type.GetType("System.Int16");
                }
            }
            else if (typeStr.StartsWith("DATE"))
            {
                
            }
            MaxSize = size;
        }

        internal void ParseConstraint(string sql)
        {

        }

        public override string ToString()
        {
            return Name;
        }

        internal void SetForeignKey(string refTab, string refCol)
        {
            IsForeignKey = true;
            referenceTableName = refTab;
            referenceColumnName = refCol;
        }

        internal void ResolveForeignKey(List<TableSchema> tableSchemas)
        {
            foreach (TableSchema tabSchema in tableSchemas)
            {
                if (tabSchema.Name == referenceTableName)
                {
                    foreach (ColumnSchema colSchema in tabSchema.Columns)
                    {
                        if (colSchema.Name == referenceColumnName)
                            ReferenceColumn = colSchema;
                    }
                    ReferenceTable = tabSchema;
                }
            }
        }

        internal void SetValConstraints(string valsString)
        {
            ValueConstraints = new List<object>();
            if (type == Type.GetType("System.String"))
            {
                Regex valPat = new Regex("'([^,]*)'");
                MatchCollection vals = valPat.Matches(valsString);
                foreach (Match val in vals)
                {
                    ValueConstraints.Add(val.Groups[1].Value);
                }
            }
        }
    }
}
