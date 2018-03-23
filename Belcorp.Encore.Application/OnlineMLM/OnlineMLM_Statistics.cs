using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.OnlineMLM
{
    public class OnlineMLM_Statistics
    {
        public int orderId { get; set; }
        public Orders order { get; set; }
    }
}
