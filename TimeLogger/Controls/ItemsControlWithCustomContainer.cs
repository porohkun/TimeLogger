using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TimeLogger
{
    public class ItemsControlWithCustomContainer : ItemsControl
    {
        protected override bool IsItemItsOwnContainerOverride(object item) { return false; }
    }
}
