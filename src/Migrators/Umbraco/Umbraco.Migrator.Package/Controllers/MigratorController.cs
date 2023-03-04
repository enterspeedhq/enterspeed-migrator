using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.BackOffice.Controllers;

namespace Umbraco.Migrator.Package.Controllers
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
        public async Task<IActionResult> ImportData(ImportDataRequest importDataRequest)
        {
            await _migratorService.ImportDataAsync(importDataRequest.EnterspeedHandle, importDataRequest.SectionId);
            return Ok("");
        }

        public class ImportDataRequest
        {
            [JsonProperty("enterspeedHandle")]
            public string EnterspeedHandle { get; set; }

            [JsonProperty("sectionId")]
            public int? SectionId { get; set; }
        }
    }
}