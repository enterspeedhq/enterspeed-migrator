using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco9.Migrator.Builders.Contracts;

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
            await _migratorService.ImportDocumentTypesAsync();
            throw new System.Exception("painfull exception");
            return Ok("");
        }

        [HttpPost]
        public async Task<IActionResult> ImportData()
        {
             await _migratorService.ImportDataAsync();
            throw new System.Exception("painfull exception");
            return Ok("");
        }
    }
}