using Newtonsoft.Json;

namespace KeudellCoding.Blazor.AdvancedBlazorSelect2 {
    public class PaginationOptions {
        [JsonProperty("more")]
        public bool More { get; set; } = false;
    }
}
