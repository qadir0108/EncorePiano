using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFP.ICT.Data.Entities;

namespace WFP.ICT.Data.Repository
{
    public class UnitOfWork : IDisposable
    {
        private WFPICTContext context = new WFPICTContext();
        private GenericRepository<WFPUser> campaignRepository;

        public GenericRepository<WFPUser> CampaignRepository
        {
            get
            {
                if (this.campaignRepository == null)
                {
                    this.campaignRepository = new GenericRepository<WFPUser>(context);
                }
                return campaignRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
