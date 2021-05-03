using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Zadanie2_1MIW
{
    public class UserConfig
    {
        public string DataName;
        public string DataSaveName;
        public string DataConfigName;
        public string DataPath;
        public string DataTypesPath;
        public string DataSeparator;
        public int DataNormalizationFrom;
        public int DataNormalizationTo;
        public int ClassColumn;
        public List<MinimumValues> Mins;
        public List<MaximumValues> Maxs;
        public List<DataSybmolicToNumeric> DataSybmolicsToNumerics;
    }
}
