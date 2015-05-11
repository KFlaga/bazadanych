using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanych
{
    public class ColumnSchema
    {
        public string Name { get; private set; }
        public Type type { get; private set; }
        public int MaxSize { get; private set; }
        public bool IsNullable { get; private set; }

        public void ParseColumn(string name, string nullable, string typeStr, int size)
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

        public override string ToString()
        {
            return Name;
        }
    }
}
