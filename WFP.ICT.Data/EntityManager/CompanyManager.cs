using WFP.ICT.Data.Entities;

namespace WFP.ICT.Data.EntityManager
{
    public class CompanyManager : BaseEntityManager<Company>
    {
        public CompanyManager()
        {

        }

        public CompanyManager(WFPICTContext context) : base(context)
        {

        }
    }
}
