using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.LocalStorageUtils
{
    class PageSettingsUtil
    {
        private Windows.Storage.ApplicationDataContainer container = Windows.Storage.ApplicationData.Current.RoamingSettings;
        public void setCurrentPage(string pageName) {
            container.Values["currentPage"] = pageName;            
        }

        public String getCurrentPage() {
            return (String) container.Values["currentPage"];
        }
    }
}
