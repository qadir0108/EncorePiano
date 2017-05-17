using System;
using System.Collections.Generic;
using System.Data;

namespace WFP.ICT.Web.Helpers
{
    public static class ListExtensions
    {
        public static DataSet ToDataSet<T>(this IList<T> list)
        {
            var ds = new DataSet();
            ds.Tables.Add(list.ToDataTable());
            return ds;
        }

        public static DataTable ToDataTable<T>(this IList<T> list)
        {
            Type elementType = typeof(T);
            DataTable t = new DataTable();

            //add a column to table for each public property on T
            foreach (var propInfo in elementType.GetProperties())
            {
                Type ColType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

                t.Columns.Add(propInfo.Name, ColType);
            }

            //go through each property on T and add each value to the table
            foreach (T item in list)
            {
                DataRow row = t.NewRow();

                foreach (var propInfo in elementType.GetProperties())
                {
                    row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;
                }

                t.Rows.Add(row);
            }
            return t;
        }
    }

}