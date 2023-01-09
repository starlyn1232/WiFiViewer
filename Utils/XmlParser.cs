using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiFiViewer.Utils
{
    internal static class XmlParser
    {
        //Fast Xml Parser
        internal static string XmlRead(ref string xml, string _finder)
        {
            string _return = "";
            string init = $"<{_finder}>";
            string end = $"</{_finder}>";

            string line = "";
            StringReader reader = new(xml);

            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains(init) && line.Contains(end))
                {
                    _return = line.Substring(line.IndexOf(init) + init.Length);
                    _return = _return.Substring(0, _return.IndexOf(end));

                    break;
                }
            }

            return _return;
        }
    }
}
