using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KeudellCoding.Blazor.AdvancedBlazorSelect2 {
    public partial class Select2<TItem, TSource> : ComponentBase where TSource : IEnumerable<TItem> {

        private const string JS_COMMAND_PREFIX = "KeudellCoding_Blazor_AdvancedBlazorSelect2Component";
        private const string HTML_ID_PREFIX = "select2-input-";

        //===========================================================================================================================================

        [Inject] public IJSRuntime JSRuntime { get; set; }

        //===========================================================================================================================================

        [Parameter, Required] public Func<TItem, string> IdSelector { get; set; }
        [Parameter, Required] public Func<TItem, string> TextSelector { get; set; }
        [Parameter, Required] public Func<TSource, string, CancellationToken, Task<List<TItem>>> FilterFunction { get; set; }
        [Parameter, Required] public Func<TSource, string, CancellationToken, Task<TItem>> GetElementById { get; set; }
        [Parameter, Required] public TSource Datasource { get; set; }

        [Parameter] public List<TItem> Value { get; set; }

        [Parameter] public EventCallback OnValueChanged { get; set; } = EventCallback.Empty;

        [Parameter] public Func<TItem, MarkupString> OptionLayout { get; set; }
        [Parameter] public InputSelect2Options Select2Options { get; set; } = new InputSelect2Options();
        [Parameter] public ushort MaxItemsPerPage { get; set; } = 50;
        [Parameter] public bool Multiselect { get => Select2Options.Multiple; set => Select2Options.Multiple = value; }

        [Parameter] public IMemoryCache Cache { get; set; } = null;

        [Parameter(CaptureUnmatchedValues = true)] public IDictionary<string, object> AdditionalAttributes { get; set; }

        //===========================================================================================================================================

        private readonly string inputGuid = Guid.NewGuid().ToString();

        private CancellationTokenSource searchCancellationToken = null;
        private DotNetObjectReference<Select2<TItem, TSource>> dotNetRef;

        //===========================================================================================================================================

        protected override void OnInitialized() {
            base.OnInitialized();

            if (Datasource.Count() > MaxItemsPerPage && Select2Options.MinimumInputLength < 1)
                Select2Options.MinimumInputLength = 1;

            dotNetRef = DotNetObjectReference.Create(this);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            base.OnAfterRender(firstRender);

            if (firstRender) {
                await JSRuntime.InvokeVoidAsync($"{JS_COMMAND_PREFIX}.init", HTML_ID_PREFIX + inputGuid, JsonConvert.SerializeObject(Select2Options), dotNetRef);
                await JSRuntime.InvokeVoidAsync($"{JS_COMMAND_PREFIX}.onChange", HTML_ID_PREFIX + inputGuid, dotNetRef);
                await updateSelect2Async(false);
            }

            await checkIfUpdateNeededAsync();
        }

        //===========================================================================================================================================

        private async Task updateSelect2Async(bool triggerChange) {
            checkValueForLimit();

            await JSRuntime.InvokeVoidAsync($"{JS_COMMAND_PREFIX}.updateSelected", HTML_ID_PREFIX + inputGuid, getFromTItem(Value.AsQueryable()), triggerChange);
        }

        private async Task checkIfUpdateNeededAsync() {
            Value.RemoveAll(i => i == null);
            var selectedIds = await JSRuntime.InvokeAsync<string[]>($"{JS_COMMAND_PREFIX}.getSelectedIds", HTML_ID_PREFIX + inputGuid);
            if (!new HashSet<string>(selectedIds).SetEquals(Value.Select(IdSelector))) {
                await updateSelect2Async(true);
            }
        }

        private Select2Item getFromTItem(TItem item) {
            return new Select2Item {
                Id = IdSelector(item),
                Text = TextSelector(item),
                FormatedResult = OptionLayout == null ? TextSelector(item) : OptionLayout(item).ToString()
            };
        }
        private IEnumerable<Select2Item> getFromTItem(IEnumerable<TItem> items) => items.Select(getFromTItem);

        private object getCacheKey(string id) => new { Namespace = "KeudellCoding.Blazor.AdvancedBlazorSelect2", Type = "SearchCache", Select2Id = inputGuid, ObjId = id };
        private object getCacheKey(TItem item) => getCacheKey(IdSelector(item));

        private void checkValueForLimit() {
            if (Value == null)
                Value = new List<TItem> { };
            if (!Multiselect && Value.Count > 1)
                Value.RemoveRange(1, Value.Count - 1);
        }

        private async Task<TItem> getByIdAsync(string id) {
            if (string.IsNullOrEmpty(id))
                return default;
            if (Cache != null && Cache.TryGetValue(getCacheKey(id), out TItem value))
                return value;
            else {
                var result = await GetElementById(Datasource, id, default);
                if (Cache != null)
                    Cache.Set(getCacheKey(id), result);
                return result;
            }
        }
        private async Task<IEnumerable<TItem>> getByIdAsync(IEnumerable<string> ids) => (await Task.WhenAll(ids.Select(getByIdAsync))).AsQueryable();

        private List<TItem> getPageOfResult(List<TItem> select2Items, int page, out bool hasNextPage) {
            page = page < 1 ? 1 : page;

            var elementsFromPage = select2Items.Where(e => e != null).Skip((page - 1) * MaxItemsPerPage).Take(MaxItemsPerPage + 1).ToList();
            hasNextPage = elementsFromPage.Count() > MaxItemsPerPage;
            if (hasNextPage)
                elementsFromPage.RemoveAt(elementsFromPage.Count() - 1);
            return elementsFromPage;
        }

        //===========================================================================================================================================

        [JSInvokable("Select2OnChange")]
        public async Task OnSelectionChangedAsync(IEnumerable<string> newSelectedIds) {
            Value.Clear();

            var selectedElements = await getByIdAsync(newSelectedIds.Where(i => i != null));
            if (!Multiselect)
                selectedElements = selectedElements.Take(1);

            Value.AddRange(selectedElements);

            _ = OnValueChanged.InvokeAsync(this);
        }

        [JSInvokable("Select2OnSearch")]
        public async Task<SearchServerResponse> OnSearchAsync(string searchTerm, int page) {
            if (searchCancellationToken != null)
                searchCancellationToken.Cancel();
            searchCancellationToken = new CancellationTokenSource();

            var returnItem = new SearchServerResponse { };

            var hasMoreResults = false;

            var filteredDatasource = await FilterFunction(Datasource, searchTerm, searchCancellationToken.Token);
            var searchResults = await Task.Run(() => getPageOfResult(filteredDatasource, page, out hasMoreResults), searchCancellationToken.Token);

            if (searchCancellationToken.IsCancellationRequested)
                return returnItem;

            if (Cache != null)
                foreach (var searchResult in searchResults)
                    Cache.Set(getCacheKey(searchResult), searchResult);

            returnItem.Results = getFromTItem(searchResults.AsQueryable());
            returnItem.Pagination.More = hasMoreResults;

            return returnItem;
        }
    }
}
