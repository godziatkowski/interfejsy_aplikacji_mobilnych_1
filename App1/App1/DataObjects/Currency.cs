using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.DataObjects
{
    public class Currency
    {
        public String currencyName { get; set; }
        public int conversionRate { get; set; }        
        public String currencyCode { get; set; }        
        public double currencyAsPLN { get; set; }

    }
}
