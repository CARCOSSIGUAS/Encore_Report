using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public interface IMonitorMongoService
    {
        void Migrate();
    }
}