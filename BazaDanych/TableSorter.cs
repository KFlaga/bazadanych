using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanych
{
    class TableSorter
    {
        private Comparison<object[]> compareMethod;
        public Table TableSource { get; set; }
        public bool IsSortedAscending { get; private set; }
        public ColumnSchema LastSortColumn { get; private set; }

        public TableSorter(Table table)
        {
            TableSource = table;
            SortByColumn(TableSource.Columns[0],true);
        }

        public void SortByColumn(ColumnSchema column, bool sortAscending)
        {
            int columnPos = -1;
            for (int col = 0; col < TableSource.Columns.Count; col++ )
            {
                if (column == TableSource.Columns[col])
                {
                    columnPos = col;
                    break;
                }
            }
            if (columnPos == -1)
                return;

            LastSortColumn = column;
            IsSortedAscending = sortAscending;

            if (column.type == Type.GetType("System.String"))
            {
                if(IsSortedAscending)
                    compareMethod = (object[] A, object[] B) =>
                    {
                        if (B[columnPos] == null)
                            return 1;
                        if (A[columnPos] == null)
                            return -1;
                        return String.Compare((string)A[columnPos], (string)B[columnPos]);
                    };
                else
                    compareMethod = (object[] A, object[] B) =>
                    {
                        if (B[columnPos] == null)
                            return 1;
                        if (A[columnPos] == null)
                            return -1;
                        return String.Compare((string)B[columnPos], (string)A[columnPos]);
                    };
            }
            if (column.type == Type.GetType("System.Int32") || column.type == Type.GetType("System.Int16"))
            {
                if (IsSortedAscending)
                    compareMethod = (object[] A, object[] B) =>
                    {
                        if (B[columnPos] == null)
                            return 1;
                        if (A[columnPos] == null)
                            return -1;
                        if ((int)A[columnPos] > (int)B[columnPos])
                            return 1;
                        else if ((int)A[columnPos] < (int)B[columnPos])
                            return -1;
                        else
                            return 0;
                    };
                else
                    compareMethod = (object[] A, object[] B) =>
                    {
                        if (B[columnPos] == null)
                            return 1;
                        if (A[columnPos] == null)
                            return -1;
                        if ((int)A[columnPos] < (int)B[columnPos])
                            return 1;
                        else if ((int)A[columnPos] > (int)B[columnPos])
                            return -1;
                        else
                            return 0;
                    };
            }
            if (column.type == Type.GetType("System.DateTime") )
            {
                if (IsSortedAscending)
                    compareMethod = (object[] A, object[] B) =>
                    {
                        if (B[columnPos] == null)
                            return 1;
                        if (A[columnPos] == null)
                            return -1;
                        return DateTime.Compare((DateTime)((DateOnly)A[columnPos]).Date, (DateTime)((DateOnly)B[columnPos]).Date);
                    };
                else
                    compareMethod = (object[] A, object[] B) =>
                    {
                        if (B[columnPos] == null)
                            return 1;
                        if (A[columnPos] == null)
                            return -1;
                        return DateTime.Compare((DateTime)((DateOnly)B[columnPos]).Date, (DateTime)((DateOnly)A[columnPos]).Date);
                    };
            }

            TableSource.Rows.Sort(compareMethod);
        }
    }
}
