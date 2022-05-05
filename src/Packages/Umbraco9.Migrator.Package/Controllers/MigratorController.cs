using System.Threading.Tasks;
using Enterspeed.Migrator.Contracts;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;

namespace Umbraco9.Migrator.Package.Controllers
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
            await _migratorService.BuildUmbracoDataAsync();
            return Ok("");
        }
    }
}