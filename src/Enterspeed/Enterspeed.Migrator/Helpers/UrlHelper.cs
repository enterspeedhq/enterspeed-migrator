using System.Collections.Generic;
using System.Linq;
using Enterspeed.Migrator.Models;

namespace Enterspeed.Migrator.Helpers
{
    internal class UrlHelper
    {
        ///// <summary>
        ///// Iterates trough all navigation items for the handle that handles routes
        ///// </summary>
        ///// <param name="enterspeedResponse"></param>
        ///// <returns>Returns a list of urls for the routes</returns>
        //public static List<string> GetUrls(EnterspeedResponse enterspeedResponse)
        //{
        //    var urls = new List<string>
        //    {
        //        enterspeedResponse.Views.Navigation.Self?.View?.Url
        //    };

        //    foreach (var child in enterspeedResponse.Views.Navigation.Children)
        //    {
        //        urls.Add(child.View?.Self?.View?.Url);
        //        if (child.View?.Children != null)
        //        {
        //            foreach (var subChild in child.View.Children)
        //            {
        //                AddUrl(subChild, urls);
        //            }
        //        }
        //    }

        //    return urls;
        //}

        //private static void AddUrl(Child child, ICollection<string> urls)
        //{
        //    urls.Add(child.View.Self?.View?.Url);

        //    if (child.View.Children == null || !child.View.Children.Any()) return;
        //    foreach (var subChild in child.View.Children)
        //    {
        //        AddUrl(subChild, urls);
        //    }
        //}
    }
}
