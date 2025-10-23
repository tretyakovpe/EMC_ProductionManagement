using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionManagement.Data;
using ProductionManagement.Services;

namespace ProductionManagement.Controllers
{
    public class ProgressController : Controller
    {
        private readonly ProgressManager _progressManager;
        public ProgressController(ProgressManager progressManager) => _progressManager = progressManager;

        public async Task<IActionResult> Display()
        {
            var progress = await _progressManager.GetCurrentShiftProgressAsync();
            return View("Display",progress);
        }

        public IActionResult Test()
        {
            var volumes = _progressManager.GetPlannedAndBacklog("LD1100-101");
            return Ok($"{volumes.Planned} в ячейке {volumes.Backlog}");
        }
    }
}
