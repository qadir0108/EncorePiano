using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFP.ICT.Web.Models
{
    public class SyncStartModel
    {
        public string Id { get; set; }
        public string tripStatus { get; set; }
        public string departureTime { get; set; }
        public string estimatedTime { get; set; }

        public string statusTime { get; set; }
    }

    public class SyncLoadModel
    {
        public string Id { get; set; }
        public string assignmentId { get; set; }
        public int isMainUnitLoaded { get; set; }
        public string loadingTimeStamp { get; set; }
        public int additionalBenchesStatus { get; set; }
        public int additionalCasterCupsStatus { get; set; } 
        public int additionalCoverStatus { get; set; } 
        public int additionalLampStatus { get; set; } 
        public int additionalOwnersManualStatus { get; set; }
    }

    public class SyncDeliverModel
    {
        public string Id { get; set; }
        public string assignmentId { get; set; }
        public string pianoStatus { get; set; }
        public string deliveredAt { get; set; }
        public int benchesUnloaded { get; set; }
        public int casterCupsUnloaded { get; set; }
        public int coverUnloaded { get; set; }
        public int lampUnloaded { get; set; }
        public int ownersManualUnloaded { get; set; }
        public string receiverName { get; set; }
        public string receiverSignature { get; set; }
        public string dateSigned { get; set; }
    }

    public class SyncImageModel
    {
        public string Id { get; set; }
        public string unitId { get; set; }
        public string image { get; set; }
        public string takenAt { get; set; }
    }
}