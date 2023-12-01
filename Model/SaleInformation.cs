﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Microsis.CWM.Model
{
    [Table("SaleInformation")]
    public class SaleInformation
    {
        public long Id { get; set; }
        public long WholesaleId { get; set; }
        public string? SaleCondition { get; set; }
        public string? OpenningDescription { get; set; }
        public bool IsActive { get; set; }
        public string? ConfirmUsername { get; set; }
        public string? RecordUsername { get; set; }
        public string? RecordDate { get; set; }
        public string? ConfirmDate { get; set; }
        public string? RecordTime { get; set; }
        public string? ConfirmTime { get; set; }
        public int RecordDateint { get; set; }
        public int ConfirmDateint { get; set; }
        public int RecordTimeint { get; set; }
        public int ConfirmTimeint { get; set; }
    }
}
