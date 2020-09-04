using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace KeudellCoding.Blazor.AdvancedBlazorSelect2 {
    public partial class Select2Enum<TEnum> : ComponentBase where TEnum : Enum {

        [Parameter, Required] public List<TEnum> Value { get; set; }
        [Parameter] public EventCallback OnValueChanged { get; set; } = EventCallback.Empty;
        [Parameter] public Func<IEnumerable<TEnum>, string, CancellationToken, Task<List<TEnum>>> FilterFunction { get; set; } = filterEnums;
        [Parameter] public bool Multiselect { get => Select2Options.Multiple; set => Select2Options.Multiple = value; }

        [Parameter] public InputSelect2Options Select2Options { get; set; } = new InputSelect2Options();
        [Parameter] public ushort MaxItemsPerPage { get; set; } = 50;

        [Parameter(CaptureUnmatchedValues = true)] public IDictionary<string, object> AdditionalAttributes { get; set; }

        //===========================================================================================================================================

        private static Task<List<TEnum>> filterEnums(IEnumerable<TEnum> allItems, string filter, CancellationToken token) {
            var result = allItems?.ToDictionary(e => e, e => {
                var counter = 0;

                if (getName(e).StartsWith(filter, StringComparison.CurrentCultureIgnoreCase))
                    counter += 2;
                if (getName(e).ToUpper().Contains(filter.ToUpper()))
                    counter++;

                if (getDisplayValue(e).StartsWith(filter, StringComparison.CurrentCultureIgnoreCase))
                    counter += 2;
                if (getDisplayValue(e).ToUpper().Contains(filter.ToUpper()))
                    counter++;

                return counter;
            }) ?? new Dictionary<TEnum, int> { };

            return Task.FromResult(result.Where(kvp => string.IsNullOrEmpty(filter) || kvp.Value > 0)
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToList());
        }

        //===========================================================================================================================================

        private static string getDisplayValue(TEnum value) {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attr = fieldInfo.GetCustomAttribute<DisplayAttribute>(false);
            return attr?.Name ?? value.ToString();
        }

        private static string getName(TEnum value) {
            var fieldInfo = value.GetType().GetField(value.ToString());
            return fieldInfo?.Name ?? value.ToString();
        }
    }
}
