using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyBook
{
    public class BuyModel
    {
        public string emailid { get; set; }
        public string bookid { get; set; }
        public DateTime purchasedate { get; internal set; }
        public string IsRefund { get; internal set; }
        public string paymentmode { get; internal set; }
    }
}
