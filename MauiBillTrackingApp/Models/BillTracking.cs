using System;
using System.Collections.Generic;

namespace BillTrackingAppBackend.Models;

public partial class BillTracking
{
    public int BillId { get; set; }

    public string Lähettäjä { get; set; } = null!;

    public decimal Summa { get; set; }

    public DateTime Eräpäivä { get; set; }

    public string Tila { get; set; } = null!;

    public DateTime? Maksupäivä { get; set; }
}
