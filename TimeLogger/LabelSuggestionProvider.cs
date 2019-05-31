using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoCompleteTextBox.Editors;

namespace TimeLogger
{
    public class LabelsSuggestionProvider : ISuggestionProvider
    {
        public IEnumerable<Label> Labels { get; set; }

        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return null;
            return
                Labels
                    .Where(state => state.Name.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
        }

        public LabelsSuggestionProvider()
        {
            Labels = Label.Labels;
        }
    }

}