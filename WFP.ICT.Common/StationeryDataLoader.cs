using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;

namespace WFP.ICT.Common
{
    public class StationeryDataLoader
    {
        public static List<StationeryItemData> Load()
        {
            List<StationeryItemData> dataList = new List<StationeryItemData>();
            string dataFile = ServerUtility.MapPath(@"~/App_Data/StationeryItems.csv");
            using (TextFieldParser parser = new TextFieldParser(dataFile))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    StationeryItemData dataRow = new StationeryItemData()
                    {
                        Code = fields[0],
                        Name = fields[1]
                    };
                    dataList.Add(dataRow);
                }
            }
            return dataList;
        }
    }
}
