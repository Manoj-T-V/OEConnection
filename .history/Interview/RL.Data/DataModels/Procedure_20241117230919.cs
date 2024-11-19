using System.ComponentModel.DataAnnotations;
using RL.Data.DataModels.Common;
using System.ComponentModel.DataAnnotations.Schema;
namespace RL.Data.DataModels;

public class Procedure : IChangeTrackable
{
    public Procedure()
    {
        PlanProcedureUsers = new LinkedList<PlanProcedureUser>();
    }
    [Key]
    public int ProcedureId { get; set; }
    public string ProcedureTitle { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    [ForeignKey("ProcedureId")]
    public virtual ICollection<PlanProcedureUser> PlanProcedureUsers { get; set; }
}
