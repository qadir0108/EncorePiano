using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;

namespace WFP.ICT.Common
{
    public class InventoryItemTypesLoader
    {
        public static List<string> Load()
        {
            List<string> dataList = new List<string>();
            string dataFile = ServerUtility.MapPath(@"~/App_Data/InventoryItemTypes.csv");
            using (TextFieldParser parser = new TextFieldParser(dataFile))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    dataList.Add(fields[0]);
                }
            }
            return dataList;
        }
    }
}
