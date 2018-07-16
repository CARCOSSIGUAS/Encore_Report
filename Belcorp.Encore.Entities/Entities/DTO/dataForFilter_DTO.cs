using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.DTO
{
    public class dataForFilter_DTO
    {
        public List<stateFilter> state { get; set; }
        public List<generationFilter> generation { get; set; }
        public List<levelFilter> level { get; set; }
    }

    public class stateFilter
    {
        public string state { get; set; }
    }

    public class generationFilter
    {
        public int generation { get; set; }
    }

    public class levelFilter
    {
        public int level { get; set; }
    }
}
