using Microsoft.AspNetCore.Components;
using System.Net.NetworkInformation;
using WestWindSystem.BLL;
using WestWindSystem.Entities;

namespace WestWindWebApp.Pages
{
    public partial class RegionQueries
    {
        
        private string feedbackMessage { get; set; } = "";
        private Dictionary<string, string> errorDictionary { get; set; } = new Dictionary<string, string>();
        private int regionarg { get; set; }
        private int regionselectarg { get; set; }
        private List<Region> regionData { get; set; } = new List<Region>();
        private Region regionInfo { get; set; }

        //setup the injected dependencies required for this component
        //using property injection
        [Inject]
        public RegionServices _regionServices { get; set; }

        protected override void OnInitialized()
        {
            regionData =_regionServices.Region_GetList();
            base.OnInitialized();
        }

        private void InputProcess()
        {
            feedbackMessage = "";
            errorDictionary.Clear();

            if (regionarg <= 0)
            {
                errorDictionary.Add("Region ID","Region id must be a positive non zero number.");
            }
            if (errorDictionary.Count == 0)
            {
                regionInfo = _regionServices.Region_GetByID(regionarg);
            }
        }
        private void SelectProcess()
        {
            feedbackMessage = "";
            errorDictionary.Clear();

            if (regionselectarg == 0)
            {
                errorDictionary.Add("Region Select", "Select a region to view.");
            }
            if (errorDictionary.Count == 0)
            {
                regionInfo = _regionServices.Region_GetByID(regionselectarg);
            }
        }
    }
}
