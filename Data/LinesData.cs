using ProductionManagement.Models;
using System.Collections.Concurrent;

namespace ProductionManagement.Data
{
    public static class LinesData
    {
        public static ConcurrentDictionary<string, Line> LinesCache { get; set; } = new ConcurrentDictionary<string, Line>();
    }
}
