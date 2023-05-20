//   Fatum -- Metadata Processing Library
//
//   Copyright (C) 2003-2023 Eric Knight
//   This software is distributed under the GNU Public v3 License
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.

//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.

//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Text;
using System.Xml;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Proliferation.Fatum
{
    public sealed class TreeDataAccess
    {  
        // =======================================================================
        // Read CSV File
        // =======================================================================

        public static Tree ReadCsv(string infile)
        {
            TextReader tr = new StreamReader(infile);
            string? indata;
            Tree newData = new(infile);
            string[]? headerRow = null;

            try
            {
                int i = 0;

                do
                {
                    indata = tr.ReadLine();
                    if (indata != null)
                    {
                        string[] parsedData = indata.Split(',');

                        if (i != 0)
                        {
                            Tree newRow = new(i.ToString());
                            if (parsedData != null)
                            {
                                for (int x = 0; x < parsedData.Length; x++)
                                {
                                    if (headerRow != null && parsedData[x] != null)
                                    {
                                        newRow.SetElement(headerRow[x], parsedData[x]);
                                    }
                                }
                                newData.AddNode(newRow, "row");
                            }
                        }
                        else
                        {
                            headerRow = parsedData;
                        }
                    }
                    i++;
                }
                while (indata != null);
            }
            catch (Exception)
            {
                tr.Close();
                throw new TreeDataAccessInvalidDataException();
            }
            
            tr.Close();
            return (newData);
        }

        // =======================================================================
        // Read tab delimited text file
        // =======================================================================

        public static Tree ReadTextTab(string infile)
        {
            TextReader tr = new StreamReader(infile);
            string? indata;
            Tree newData = new(infile);
            string[]? headerRow = null;

            try
            {
                int i = 0;

                do
                {
                    indata = tr.ReadLine();
                    if (indata != null)
                    {
                        string[] parsedData = indata.Split('\t');

                        if (i != 0)
                        {
                            Tree newRow = new(i.ToString());
                            for (int x = 0; x < parsedData.Length; x++)
                            {
                                if (headerRow!=null && parsedData[x] != null)
                                {
                                    newRow.SetElement(headerRow[x], parsedData[x]);
                                }
                            }
                            newData.AddNode(newRow, "row");
                        }
                        else
                        {
                            headerRow = parsedData;
                        }
                        i++;
                    }
                }
                while (indata != null);
            }
            catch (Exception)
            {
                tr.Close();
                throw new TreeDataAccessInvalidDataException();
            }
            tr.Close();
            return (newData);
        }

        public static int WriteXml(string filename, Tree outdata, string recordName)
        {
            int result = 1;

            XmlTextWriter newWriter = new(filename,Encoding.Unicode);

            newWriter.WriteStartDocument();
            newWriter.WriteWhitespace("\r\n");
            newWriter.WriteStartElement(recordName);
            RecurseWriteXML(outdata, newWriter,1);
            newWriter.WriteEndElement();
            newWriter.WriteWhitespace("\r\n");
            newWriter.WriteEndDocument();
            newWriter.WriteWhitespace("\r\n");
            newWriter.Close();

            return result;
        }

        public static string WriteTreeToXmlString(Tree outdata, string recordName)
        {
            StringWriter stringoutput = new(new StringBuilder(63334));
            XmlTextWriter newWriter = new(stringoutput);

            newWriter.WriteStartDocument();
            newWriter.WriteWhitespace("\r\n");
            newWriter.WriteStartElement(recordName);
            RecurseWriteXML(outdata, newWriter, 1);
            newWriter.WriteEndElement();
            newWriter.WriteWhitespace("\r\n");
            newWriter.WriteEndDocument();
            newWriter.WriteWhitespace("\r\n");
            newWriter.Close();

            return stringoutput.ToString();
        }

        public static int WriteXML(TextWriter textwriter, Tree outdata, string recordName)
        {
            int result = 1;

            XmlTextWriter newWriter = new(textwriter);

            newWriter.WriteStartDocument();
            newWriter.WriteWhitespace("\r\n");
            newWriter.WriteStartElement(recordName);
            RecurseWriteXML(outdata, newWriter, 1);
            newWriter.WriteEndElement();
            newWriter.WriteWhitespace("\r\n");
            newWriter.WriteEndDocument();
            newWriter.WriteWhitespace("\r\n");
            newWriter.Close();

            return result;
        }

        private static Boolean RecurseWriteXML(Tree current, XmlTextWriter writer, int depth)
        {
            Boolean writeIndentation = true;

            try
            {
                for (int i = 0; i < current.attributes.Count; i++)
                {
                    // Write Attributes
                    writer.WriteStartAttribute(current.attributes[i]);
                    writer.WriteString(Sanitize(current.attributevalues[i]));
                    writer.WriteEndAttribute();
                }

                if (!current.Value.Equals("") && (current.tree.Count == 0))
                {
                    writeIndentation = false;

                    if (XMLTree.MustConvertValue(current.Value))
                    {
                        writer.WriteStartAttribute("HexConv");
                        writer.WriteString("true");
                        writer.WriteEndAttribute();

                        writer.WriteValue(XMLTree.StringToHex(current.Value));
                    }
                    else
                    {
                        try
                        {
                            writer.WriteValue(Sanitize(current.Value));
                        }
                        catch (Exception)
                        {
                            writer.WriteValue("Error");
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < current.tree.Count; i++)
                    {
                        Tree nextElement = current.tree[i];

                        writer.WriteWhitespace("\r\n");
                        for (int x = 0; x < depth; x++)
                        {
                            writer.WriteWhitespace(" ");
                        }

                        // Write Elements
                        writer.WriteStartElement(current.leafnames[i]);

                        // Recurse

                        writeIndentation = RecurseWriteXML(nextElement, writer, depth + 1);

                        // Write End Element
                        if (writeIndentation)
                        {
                            if (current.tree.Count > 0)
                            {
                                for (int z = 0; z < depth; z++)
                                {
                                    writer.WriteWhitespace(" ");
                                }
                            }
                        }

                        writer.WriteEndElement();
                        writeIndentation = true;
                    }
                    writer.WriteWhitespace("\r\n");
                }
            }
            catch (XmlException)
            {
                throw new TreeDataAccessInvalidDataException();
            }
            catch (Exception)
            {
                throw new TreeDataAccessInvalidDataException();
            }

            return writeIndentation;
        }

        public static Tree ReadJsonFromString(string indata)
        {
            string xml = "";

            try
            {
                string value = CleanInput(indata);
                XmlDocument doc = JsonConvert.DeserializeXmlNode(value, "Root");
                

                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    doc.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    xml = stringWriter.GetStringBuilder().ToString();
                }
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {

            }
            catch (Exception)
            {

            }
            return XMLTree.ReadXmlFromString(xml);
        }

        public static string Sanitize(string instring)
        {
            return instring.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;").Replace("\"", "&quot;").Replace("'", "&apos;").Replace("\0", string.Empty);
        }

        public static string CleanInput(string strIn)
        {
            return Regex.Replace(strIn, @"[^\u0009\u000A\u000D\u0020-\uFFFE]", "*");
        }

        public static string WriteJsonToString(Tree outdata, string recordName)
        {
            TextWriter textwriter = new StringWriter();
            XmlTextWriter newWriter = new XmlTextWriter(textwriter);

            newWriter.WriteStartDocument();
            newWriter.WriteWhitespace("\r\n");
            newWriter.WriteStartElement(recordName);
            RecurseWriteXML(outdata, newWriter, 1);
            newWriter.WriteEndElement();
            newWriter.WriteWhitespace("\r\n");
            newWriter.WriteEndDocument();
            newWriter.WriteWhitespace("\r\n");
            newWriter.Close();

            string xmlformat = textwriter.ToString();
            xmlformat = xmlformat.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n", "");
            return XmlToJson(xmlformat);
        }

        public static string XmlToJson(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            return XmlToJson(doc);
        }

        public static string XmlToJson(XmlDocument xmlDoc)
        {
            StringBuilder sbJSON = new StringBuilder();
            sbJSON.Append("{ ");
            XmlToJsonNode(sbJSON, xmlDoc.DocumentElement, true);
            sbJSON.Append("}");
            return sbJSON.ToString();
        }

        //  XmlToJSONnode:  Output an XmlElement, possibly as part of a higher array
        private static void XmlToJsonNode(StringBuilder sbJSON, XmlElement node, bool showNodeName)
        {
            if (showNodeName)
                sbJSON.Append("\"" + SafeJson(node.Name) + "\": ");
            sbJSON.Append("{");
            // Build a sorted list of key-value pairs
            //  where   key is case-sensitive nodeName
            //          value is an ArrayList of string or XmlElement
            //  so that we know whether the nodeName is an array or not.
            SortedList<string, object> childNodeNames = new SortedList<string, object>();

            //  Add in all node attributes
            if (node.Attributes != null)
                foreach (XmlAttribute attr in node.Attributes)
                    StoreChildNode(childNodeNames, attr.Name, attr.InnerText);

            //  Add in all nodes
            foreach (XmlNode cnode in node.ChildNodes)
            {
                if (cnode is XmlText)
                    StoreChildNode(childNodeNames, "value", cnode.InnerText);
                else if (cnode is XmlElement)
                    StoreChildNode(childNodeNames, cnode.Name, cnode);
            }

            // Now output all stored info
            foreach (string childname in childNodeNames.Keys)
            {
                List<object> alChild = (List<object>)childNodeNames[childname];
                if (alChild.Count == 1)
                    OutputNode(childname, alChild[0], sbJSON, true);
                else
                {
                    sbJSON.Append(" \"" + SafeJson(childname) + "\": [ ");
                    foreach (object Child in alChild)
                        OutputNode(childname, Child, sbJSON, false);
                    sbJSON.Remove(sbJSON.Length - 2, 2);
                    sbJSON.Append(" ], ");
                }
            }
            sbJSON.Remove(sbJSON.Length - 2, 2);
            sbJSON.Append(" }");
        }

        //  StoreChildNode: Store data associated with each nodeName
        //                  so that we know whether the nodeName is an array or not.
        private static void StoreChildNode(SortedList<string, object> childNodeNames, string nodeName, object nodeValue)
        {
            // Pre-process contraction of XmlElement-s
            if (nodeValue is XmlElement)
            {
                // Convert  <aa></aa> into "aa":null
                //          <aa>xx</aa> into "aa":"xx"
                XmlNode cnode = (XmlNode)nodeValue;
                if (cnode.Attributes.Count == 0)
                {
                    XmlNodeList children = cnode.ChildNodes;
                    if (children.Count == 0)
                        nodeValue = null;
                    else if (children.Count == 1 && (children[0] is XmlText))
                        nodeValue = ((XmlText)(children[0])).InnerText;
                }
            }
            // Add nodeValue to ArrayList associated with each nodeName
            // If nodeName doesn't exist then add it
            List<object> ValuesAL;

            if (childNodeNames.ContainsKey(nodeName))
            {
                ValuesAL = (List<object>)childNodeNames[nodeName];
            }
            else
            {
                ValuesAL = new List<object>();
                childNodeNames[nodeName] = ValuesAL;
            }
            ValuesAL.Add(nodeValue);
        }

        private static void OutputNode(string childname, object alChild, StringBuilder sbJSON, bool showNodeName)
        {
            if (alChild == null)
            {
                if (showNodeName)
                    sbJSON.Append("\"" + SafeJson(childname) + "\": ");
                sbJSON.Append("null");
            }
            else if (alChild is string)
            {
                if (showNodeName)
                    sbJSON.Append("\"" + SafeJson(childname) + "\": ");
                string sChild = (string)alChild;
                sChild = sChild.Trim();
                sbJSON.Append("\"" + SafeJson(sChild) + "\"");
            }
            else
                XmlToJsonNode(sbJSON, (XmlElement)alChild, showNodeName);
            sbJSON.Append(", ");
        }

        // Make a string safe for JSON
        private static string SafeJson(string sIn)
        {
            StringBuilder sbOut = new StringBuilder(sIn.Length);
            foreach (char ch in sIn)
            {
                if (Char.IsControl(ch) || ch == '\'')
                {
                    int ich = (int)ch;
                    sbOut.Append(@"\u" + ich.ToString("x4"));
                    continue;
                }
                else if (ch == '\"' || ch == '\\' || ch == '/')
                {
                    sbOut.Append('\\');
                }
                sbOut.Append(ch);
            }
            return sbOut.ToString();
        }

        public static Tree ReadJson(string filename)
        {
            string infile = File.ReadAllText(filename);
            Tree converted = ReadJsonFromString(infile);
            return converted;
        }

        public static int WriteJson(string filename, Tree outdata, string recordName)
        {
            int result = 1;
            string tmp = WriteJsonToString(outdata, recordName);
            File.WriteAllText(filename, tmp);
            return result;
        }
    }
}
