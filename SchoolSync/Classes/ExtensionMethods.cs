using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSync
{
    public static class ExtensionMethods
    {
        public static string Implode(this School school)
        {
            PropertyInfo[] _PropertyInfos = null;
            if(_PropertyInfos == null)
                _PropertyInfos = school.GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var info in _PropertyInfos)
            {
                var value = info.GetValue(school, null) ?? "";
                sb.AppendLine(value.ToString()+"~");
            }
            return sb.ToString();
        }
    }
}
