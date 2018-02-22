using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class Proof : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public int ProofType { get; set; } // PictureTypeEnum

        // Pickup
        public bool IsMainUnitLoaded { get; set; }
        public DateTime? LoadTime { get; set; }
        public string PickerName { get; set; }
        public string PickerSignature { get; set; }
        public int AdditionalBench1Status { get; set; } // AdditionalItemStatus
        public int AdditionalBench2Status { get; set; } // AdditionalItemStatus
        public int AdditionalCasterCupsStatus { get; set; } // AdditionalItemStatus
        public int AdditionalCoverStatus { get; set; } // AdditionalItemStatus
        public int AdditionalLampStatus { get; set; } // AdditionalItemStatus
        public int AdditionalOwnersManualStatus { get; set; } // AdditionalItemStatus
        public int AdditionalMisc1Status { get; set; } // AdditionalItemStatus
        public int AdditionalMisc2Status { get; set; } // AdditionalItemStatus
        public int AdditionalMisc3Status { get; set; } // AdditionalItemStatus

        // Delivery
        public DateTime? ReceivingTime { get; set; } // SignedOn
        public string ReceivedBy { get; set; }
        public string Signature { get; set; }
        public string Notes { get; set; }
        public bool Bench1UnloadStatus { get; set; }
        public bool Bench2UnloadStatus { get; set; }
        public bool CasterCupsUnloadStatus { get; set; }
        public bool CoverUnloadStatus { get; set; } 
        public bool LampUnloadStatus { get; set; }
        public bool OwnersManualUnloadStatus { get; set; } 
        public bool Misc1UnloadStatus { get; set; } 
        public bool Misc2UnloadStatus { get; set; } 
        public bool Misc3UnloadStatus { get; set; } 
        public string PodForm { get; set; } //signed form in here
        public string AcknowledgeTo { get; set; } // Email addresses

        public Guid? PianoId { get; set; }
        public virtual Piano Piano { get; set; }

        public Guid? AssignmentId { get; set; }
        public virtual Assignment Assignment { get; set; }

        public virtual ICollection<PianoPicture> Pictures { get; set; }

        public Proof()
        {
            Pictures = new HashSet<PianoPicture>();
        }
    }
}
