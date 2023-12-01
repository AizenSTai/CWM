using System.ComponentModel.DataAnnotations.Schema;

namespace Microsis.CWM.Model
{
    [Table("Guild")]

    public class Guild
    {
        public long Id { get; set; }
        public string? GuildNameFa { get; set; }
        public string? GuildNameEn { get; set; }
    }
}
