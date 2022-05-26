using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Migrator.Models;
using Enterspeed.Migrator.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco9.Migrator.Builders.Contracts;
using Umbraco9.Migrator.Extensions;

namespace Umbraco9.Migrator.Builders
{
    public class DocumentTypeBuilder : IDocumentTypeBuilder
    {
        private readonly EnterspeedConfiguration _enterspeedConfiguration;
        private readonly ILogger<DocumentTypeBuilder> _logger;
        private readonly IContentTypeService _contentTypeService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly List<ContentTypeSort> _contentTypes;
        private ContentType _root;
        const string ContainerName = "Migrated Page Types";

        public DocumentTypeBuilder(ILogger<DocumentTypeBuilder> logger, IOptions<EnterspeedConfiguration> enterspeedConfiguration, IContentTypeService contentTypeService, IShortStringHelper shortStringHelper)
        {
            _enterspeedConfiguration = enterspeedConfiguration?.Value;
            _logger = logger;
            _contentTypeService = contentTypeService;
            _shortStringHelper = shortStringHelper;
            _contentTypes = new List<ContentTypeSort>();
        }

        public void BuildPageDocTypes(EntityTypes entityTypes)
        {
            try
            {
                var container = CreateContainer();
                var sortOrder = 0;

                foreach (var page in entityTypes.Pages)
                {
                    CreatePageDocType(page, container, sortOrder);
                    sortOrder++;
                }

                _root.AllowedContentTypes = _contentTypes;
                _contentTypeService.Save(_root);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong when building schemas");
                throw;
            }
        }

        private void CreatePageDocType(EntityType page, Attempt<OperationResult<OperationResultType, EntityContainer>> container, int sortOrder)
        {
            var contentTypes = _contentTypeService.GetAllContentTypeAliases();
            if (!contentTypes.Any(c => c.Equals(page.Meta.SourceEntityAlias)))
            {
                var newPageDocumentType = new ContentType(_shortStringHelper, container.Result.Entity.Id)
                {
                    Alias = page.Meta.SourceEntityAlias.FirstCharToLower(),
                    Name = page.Meta.SourceEntityName.FirstCharToUpper(),
                    AllowedAsRoot = string.Equals(page.Meta.SourceEntityAlias, _enterspeedConfiguration.RootPageType, StringComparison.InvariantCultureIgnoreCase)
                };

                _contentTypeService.Save(newPageDocumentType);

                if (newPageDocumentType.AllowedAsRoot)
                {
                    _root = newPageDocumentType;
                }
                else
                {
                    _contentTypes.Add(new ContentTypeSort(newPageDocumentType.Id, sortOrder));
                }
            }
        }

        private Attempt<OperationResult<OperationResultType, EntityContainer>> CreateContainer()
        {
            var container = _contentTypeService.CreateContainer(-1, Guid.NewGuid(), ContainerName);
            _contentTypeService.SaveContainer(container.Result.Entity);
            return container;
        }
    }
}
