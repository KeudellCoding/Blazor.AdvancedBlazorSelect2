# Blazor.AdvancedBlazorSelect2

## Installation
1. Install the NuGet package `KeudellCoding.Blazor.AdvancedBlazorSelect2`.
2. Add the following lines to _Host.cshtml (in the head tag)
```html
<script src="_content/KeudellCoding.Blazor.AdvancedBlazorSelect2/Select2JsInterop.js" type="text/javascript" language="javascript"></script>
<script src="_content/KeudellCoding.Blazor.AdvancedBlazorSelect2/lib/select2/js/select2.full.min.js" type="text/javascript" language="javascript"></script>
<link href="_content/KeudellCoding.Blazor.AdvancedBlazorSelect2/lib/select2/css/select2.min.css" rel="stylesheet" />
```
3. Make sure [jQuery](https://jquery.com/download/) is installed.
4. Add the following lines to _Imports.razor
```csharp
@using KeudellCoding.Blazor.AdvancedBlazorSelect2
```

## Usage Example
```csharp
@page "/"
@inject IMemoryCache __MemoryCache
@inject ApplicationDbContext __DbContext

<Select2 TItem="ExampleItemForm"
         TSource="DbSet<ExampleItemForm>"
         IdSelector="c => c.Id.ToString()"
         TextSelector="c => c.Text"
         FilterFunction="filterFunction"
         GetElementById="async (items, id, token) => await items.FindAsync(id, token)"
         Datasource="__DbContext.ExampleItems"
         Value="@SelectedItems"
         Cache="__MemoryCache"
         Multiselect="false" />


@code {
    public class ExampleItemForm {
        public Guid Id { get; set; }
        public string Text { get; set; }

        public ExampleItemForm(Guid id, string text) {
            Id = id;
            Text = text;
        }
    }

    //======================================================

    public List<ExampleItemForm> SelectedItems { get; set; }

    //======================================================

    private async Task<List<ExampleItemForm>> filterFunction(DbSet<ExampleItemForm> allItems, string filter, CancellationToken token) {
        return await allItems.Where(i => i.Text.StartsWith(filter)).ToListAsync();
    }
}
```

## Notes
1. It is ensured that the `SelectedItems` list contains only one item if `Multiselect` is set to `false`.
2. As soon as `SelectedItems` is edited manually, `StateHasChanged();` should be executed.
3. `GetElementById` must be set manually because the process also depends on the data source used.