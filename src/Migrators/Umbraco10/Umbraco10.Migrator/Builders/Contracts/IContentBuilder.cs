﻿using System.Collections.Generic;
using Enterspeed.Migrator.Models;
using Umbraco.Cms.Core.Models;

namespace Umbraco10.Migrator.Builders.Contracts
{
    public interface IContentBuilder
    {
        void BuildContentPages(List<PageData> pageEntityTypes, IContent parent = null);
    }
}