using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoCompleteTextBox.Editors;
using TimeLogger.Domain.Data;

namespace TimeLogger.Misc
{
    public class TagSuggestionProvider : ISuggestionProvider
    {
        public IEnumerable<Tag> Tags { get; set; }

        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) Enumerable.Empty<Tag>();
            return Tags
                .Where(state => state.Name.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
        }

        //public LabelsSuggestionProvider()
        //{
        //    Tags = Tag.Tags;
        //}
    }
}
