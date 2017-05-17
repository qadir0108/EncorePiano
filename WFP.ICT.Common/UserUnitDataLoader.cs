using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;

namespace WFP.ICT.Common
{
    public class UserUnitDataLoader
    {
        public static List<UserUnitData> Load()
        {
            List<UserUnitData> dataList = new List<UserUnitData>();
            string dataFile = ServerUtility.MapPath(@"~/App_Data/UserUnit.csv");
            using (TextFieldParser parser = new TextFieldParser(dataFile))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    UserUnitData dataRow = new UserUnitData()
                    {
                        Unit = fields[0],
                        User = fields[1]
                    };
                    dataList.Add(dataRow);
                }
            }
            return dataList;
        }
    }
}
