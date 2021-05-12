using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT.Import.EGI
{
    public class ImportObject
    {
        public static List<String> articleNotPlacedList = new List<string>();
        public static Dictionary<int, string> articlePlacedDict = new Dictionary<int, string>();
        public static Dictionary<int, int> articlePlacedRenumberDict = new Dictionary<int, int>();
    }
}
