using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanNShop_POC.Views
{
    public class MessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BotTemplate { get; set; }
        public DataTemplate UserTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return (item as MessageObject).IsBot ? BotTemplate : UserTemplate;
        }
    }
}
