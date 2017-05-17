using WFP.ICT.Data.Entities;

namespace WFP.ICT.Data.EntityManager
{
    public class AspNetClaimsManager : BaseEntityManager<AspNetClaims>
    {
        public AspNetClaimsManager()
        {

        }

        public AspNetClaimsManager(WFPICTContext context) : base(context)
        {

        }
    }
}
