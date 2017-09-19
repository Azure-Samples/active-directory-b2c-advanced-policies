using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WingTipMusicWebApplication.HtmlHelpers
{
    public static class DropDownListHelpers
    {
        private static readonly IDictionary<string, string> Cultures;
        private static readonly IDictionary<string, string> Locations;

        static DropDownListHelpers()
        {
            Cultures = new Dictionary<string, string>
            {
                { Constants.Cultures.Keys.Auto, Constants.Cultures.Values.Auto },
                { Constants.Cultures.Keys.English, Constants.Cultures.Values.English },
                { Constants.Cultures.Keys.French, Constants.Cultures.Values.French }
            };

            Locations = new Dictionary<string, string>
            {
                { Constants.Locations.Keys.Auto, Constants.Locations.Values.Auto },
                { Constants.Locations.Keys.Manual, Constants.Locations.Values.Manual },
                { Constants.Locations.Keys.Canada, Constants.Locations.Values.Canada },
                { Constants.Locations.Keys.UnitedStates, Constants.Locations.Values.UnitedStates }
            };
        }

        public static IEnumerable<SelectListItem> CreateCultureSelectListItems(string currentCulture)
        {
            return Cultures.Select(culture => new SelectListItem
            {
                Selected = culture.Key == currentCulture,
                Text = culture.Value,
                Value = culture.Key
            }).ToList();
        }

        public static IEnumerable<SelectListItem> CreateLocationSelectListItems(string currentLocation)
        {
            return Locations.Select(culture => new SelectListItem
            {
                Selected = culture.Key == currentLocation,
                Text = culture.Value,
                Value = culture.Key
            }).ToList();
        }
    }
}
