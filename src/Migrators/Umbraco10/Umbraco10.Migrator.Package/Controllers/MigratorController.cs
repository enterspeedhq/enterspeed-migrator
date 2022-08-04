using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;

namespace Umbraco10.Migrator.Package.Controllers
{
    public class MigratorController : UmbracoAuthorizedApiController
    {
        private readonly IUmbracoMigratorService _migratorService;

        public MigratorController(IUmbracoMigratorService migratorService)
        {
            _migratorService = migratorService;
        }

        [HttpPost]
        public async Task<IActionResult> ImportDocumentTypes()
        {
            await _migratorService.ImportDocumentTypesAsync();  
            return Ok("");
        }

        [HttpPost]
        public async Task<IActionResult> ImportData()
        {
             await _migratorService.ImportDataAsync();
            return Ok("");
        }
    }
}