namespace WFP.ICT.Web.Models
{
    public class JsonResponse
    {
        public bool IsSucess { get; set; }
        public string ErrorMessage { get; set; }
        public object Result { get; set; }
    }
}