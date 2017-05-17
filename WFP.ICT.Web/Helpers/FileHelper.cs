using System.Web;

namespace WFP.ICT.Web.Helpers
{
    public class FileHelper
    {
        public static readonly string UploadsSupplierListingPath = HttpContext.Current.Server.MapPath("~/Uploads/SuppliersListings");

        public static string GetImagePath(string fileName)
        {
            return UploadsSupplierListingPath+"\\"+fileName+".jpg";
        }
    }
}