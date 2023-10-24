using Newtonsoft.Json;
using System.Collections.Generic;

namespace KeudellCoding.Blazor.AdvancedBlazorSelect2 {
    public class InputSelect2Options {
        [JsonProperty("adaptContainerCssClass")]
        public object AdaptContainerCssClass { get; set; } = null;
        [JsonProperty("adaptDropdownCssClass")]
        public object AdaptDropdownCssClass { get; set; } = null;

        [JsonProperty("ajax")]
        public object Ajax { get; set; } = null;
        [JsonProperty("allowClear")]
        public bool AllowClear { get; set; } = true;

        [JsonProperty("amdBase")]
        public string AmdBase { get; set; } = "./";
        [JsonProperty("amdLanguageBase")]
        public string AmdLanguageBase { get; set; } = "./i18n/";

        [JsonProperty("closeOnSelect")]
        public bool CloseOnSelect { get; set; } = true;
        [JsonProperty("containerCss")]
        public object ContainerCss { get; set; } = null;

        [JsonProperty("containerCssClass")]
        public string ContainerCssClass { get; set; } = string.Empty;
        //[JsonProperty("data")]
        //public IList<Select2Item> DefaultData { get; set; }

        [JsonProperty("debug")]
        public bool Debug { get; set; } = false;
        [JsonProperty("disabled")]
        public bool Disabled { get; set; } = false;

        [JsonProperty("dropdownAutoWidth")]
        public bool DropdownAutoWidth { get; set; } = false;
        [JsonProperty("dropdownCss")]
        public object DropdownCss { get; set; } = null;

        [JsonProperty("dropdownCssClass")]
        public string DropdownCssClass { get; set; } = string.Empty;
        [JsonProperty("maximumInputLength")]
        public int MaximumInputLength { get; set; } = 0;

        [JsonProperty("maximumSelectionLength")]
        public int MaximumSelectionLength { get; set; } = 0;
        [JsonProperty("minimumInputLength")]
        public int MinimumInputLength { get; set; } = 0;

        [JsonProperty("minimumResultsForSearch")]
        public int MinimumResultsForSearch { get; set; } = 0;
        [JsonProperty("multiple")]
        public bool Multiple { get; set; } = false;

        [JsonProperty("placeholder")]
        public string Placeholder { get; set; } = string.Empty;
        [JsonProperty("selectOnClose")]
        public bool SelectOnClose { get; set; } = false;

        //[JsonProperty("tags")]
        //public bool Tags { get; set; } = false;
        [JsonProperty("theme")]
        public string Theme { get; set; } = "default";

        [JsonProperty("tokenSeparators")]
        public HashSet<char> TokenSeparators { get; set; } = new HashSet<char> { };
        [JsonProperty("width")]
        public string Width { get; set; } = "100%";

        [JsonProperty("scrollAfterSelect")]
        public bool ScrollAfterSelect { get; set; } = false;
        [JsonProperty("noMarkupEscape")]
        public bool NoMarkupEscape { get; set; } = true;
        [JsonProperty("tags")] 
        public bool Tags { get; set; } = false;
    }
}
