using Newtonsoft.Json;
using System;

namespace KeudellCoding.Blazor.AdvancedBlazorSelect2 {
    public class Select2Item {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonProperty("text")]
        public string Text { get; set; }
        /// <summary>
        /// Override this property to use a custom format.
        /// </summary>
        [JsonProperty("formatedResult")]
        public string FormatedResult { get; set; }
    }
}
