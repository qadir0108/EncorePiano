namespace WFP.ICT.Web.Helpers
{
    public class ExcelUtility
    {
        public static int GetRow(int itemNumber)
        {
            int rowNumber = 2;
            int firstStartingRow = 6;
            int secondStartingRow = 63;

            if (itemNumber <= 55)
                rowNumber = itemNumber + firstStartingRow;

            if (itemNumber > 55 && itemNumber < 200)
                rowNumber = itemNumber + firstStartingRow - 55;

            if (itemNumber > 200 && itemNumber <= 215)
                rowNumber = itemNumber - 200 + secondStartingRow;

            if (itemNumber > 215)
                rowNumber = itemNumber - 200 + secondStartingRow - 15;

            return rowNumber;
        }

        public static string GetColumn(int itemNumber, string columnName)
        {
            string columnNumber = string.Empty;

            if ((itemNumber <= 55) || (itemNumber > 200 && itemNumber <= 215))
            {
                columnNumber = columnName.Equals("unit") ? "E" : (columnName.Equals("quantity") ? "F" : "G");
            }

            if ((itemNumber > 55 && itemNumber < 200) || (itemNumber > 215))
            {
                columnNumber = columnName.Equals("unit") ? "M" : (columnName.Equals("quantity") ? "N" : "O");
            }
            return columnNumber;
        }
    }
}