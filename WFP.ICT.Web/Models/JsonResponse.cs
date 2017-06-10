using System.Collections.Generic;

namespace WFP.ICT.Web.Models
{
    public class JsonResponse
    {
        public bool IsSucess { get; set; }
        public bool IsTokenValid { get; set; }
        public string AuthToken { get; set; }
        public string ErrorMessage { get; set; }
        public object Result { get; set; }

    }
}