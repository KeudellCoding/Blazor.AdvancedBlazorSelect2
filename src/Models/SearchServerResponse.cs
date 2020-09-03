using Newtonsoft.Json;
using System.Collections.Generic;

namespace KeudellCoding.Blazor.AdvancedBlazorSelect2 {
    public class SearchServerResponse {
        [JsonProperty("pagination")]
        public PaginationOptions Pagination { get; set; } = new PaginationOptions();
        [JsonProperty("results")]
        public IEnumerable<Select2Item> Results { get; set; }
    }
}
