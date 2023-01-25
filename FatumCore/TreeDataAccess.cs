//   Jophiel - Computer Forensics Collection, Analysis and Reporting Tool
//   Copyright (C) 2006-2014 by Eric Knight

//   Copyright (C) 2003-2019 Eric Knight

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Data.SQLite;
using Newtonsoft.Json;
using FatumCore;
using System.Text.RegularExpressions;
using Fatum.FatumCore;

namespace FatumCore
{
    public class TreeDataAccess
    {  
        // =======================================================================
        // Read CSV File
        // =======================================================================

        public static Tree readCSV(string infile)
        {
            TextReader tr = new StreamReader(infile);
            string indata;
            Tree newData = new Tree(infile);
            string[] headerRow = null;

            try
            {
                int i = 0;

                do
                {
                    indata = tr.ReadLine();
                    string[] parsedData = indata.Split(',');

                    if (i != 0)
                    {
                        Tree newRow = new Tree(i.ToString());
                        for (int x = 0; x < parsedData.Length; x++)
                        {
                            newRow.setElement(headerRow[x], parsedData[x]);
                        }
                        newData.addNode(newRow,"row");
                    }
                    else
                    {
                        headerRow = parsedData;
                    }
                    i++;
                }
                while (indata != null);
            }
            catch (Exception)
            {
                throw new TreeDataAccessInvalidDataException();
            }
            return (newData);
        }

        // =======================================================================
        // Read tab delimited text file
        // =======================================================================

        public static Tree readTextTab(string infile)
        {
            TextReader tr = new StreamReader(infile);
            string indata = "";
            Tree newData = new Tree(infile);
            string[] headerRow = null;

            try
            {
                int i = 0;

                do
                {
                    indata = tr.ReadLine();
                    string[] parsedData = indata.Split('\t');

                    if (i != 0)
                    {
                        Tree newRow = new Tree(i.ToString());
                        for (int x = 0; x < parsedData.Length; x++)
                        {
                            newRow.setElement(headerRow[x], parsedData[x]);
                        }
                        newData.addNode(newRow, "row");
                    }
                    else
                    {
                        headerRow = parsedData;
                    }
                    i++;
                }
                while (indata != null);
            }
            catch (Exception)
            {
                throw new TreeDataAccessInvalidDataException();
            }
            return (newData);
        }

        // =======================================================================
        // Read SQLite Table
        // =======================================================================

        public static Tree readSQLite(string filename, string Table)
        {
            Tree result = new Tree();
            ArrayList columns = new ArrayList();

            string connString = "Data Source=" + filename;
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand("pragma table_info(" + Table + ");", conn))
                {
                    conn.Open();
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                             columns.Add(dr.GetString(1));
                        }
                    }
                    conn.Close();
                }

                using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM " + Table + ";", conn))
                {
                    conn.Open();
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Tree newRow = new Tree();
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                string avalue = dr.GetValue(i).ToString();
                                newRow.addElement((string)columns[i], avalue);
                            }
                            result.addNode(newRow, "Row");
                        }
                    }
                    conn.Close();
                }
            }
            return result;
        }

        public static int writeXML(string filename, Tree outdata, string recordName)
        {
            int result = 1;

            XmlTextWriter newWriter = new XmlTextWriter(filename,Encoding.Unicode);

            newWriter.WriteStartDocument();
            newWriter.WriteWhitespace("\r\n");
            newWriter.WriteStartElement(recordName);
            recurseWriteXML(outdata, newWriter,1);
            newWriter.WriteEndElement();
            newWriter.WriteWhitespace("\r\n");
            newWriter.WriteEndDocument();
            newWriter.WriteWhitespace("\r\n");
            newWriter.Close();

            return result;
        }

        public static string writeTreeToXMLString(Tree outdata, string recordName)
        {
            StringWriter stringoutput = new StringWriter(new StringBuilder(63334));
            XmlTextWriter newWriter = new XmlTextWriter(stringoutput);

            newWriter.WriteStartDocument();
            newWriter.WriteWhitespace("\r\n");
            newWriter.WriteStartElement(recordName);
            recurseWriteXML(outdata, newWriter, 1);
            newWriter.WriteEndElement();
            newWriter.WriteWhitespace("\r\n");
            newWriter.WriteEndDocument();
            newWriter.WriteWhitespace("\r\n");
            newWriter.Close();

            return stringoutput.ToString();
        }

        public static int writeXML(TextWriter textwriter, Tree outdata, string recordName)
        {
            int result = 1;

            XmlTextWriter newWriter = new XmlTextWriter(textwriter);

            newWriter.WriteStartDocument();
            newWriter.WriteWhitespace("\r\n");
            newWriter.WriteStartElement(recordName);
            recurseWriteXML(outdata, newWriter, 1);
            newWriter.WriteEndElement();
            newWriter.WriteWhitespace("\r\n");
            newWriter.WriteEndDocument();
            newWriter.WriteWhitespace("\r\n");
            newWriter.Close();

            return result;
        }

        private static Boolean recurseWriteXML(Tree current, XmlTextWriter writer, int depth)
        {
            Boolean writeIndentation = true;

            try
            {
                //Boolean attribs = false;

                for (int i = 0; i < current.attributes.Count; i++)
                {
                    //attribs = true;
                    // Write Attributes
                    writer.WriteStartAttribute((string)current.attributes[i]);
                    writer.WriteString(sanitize((string)current.attributevalues[i]));
                    writer.WriteEndAttribute();
                }


                if (!current.Value.Equals("") && (current.tree.Count == 0))
                {
                    writeIndentation = false;
                    //Boolean mustsimplify = mustConvertValue(current.Value);
                    if (XMLTree.mustConvertValue(current.Value))
                    {
                        writer.WriteStartAttribute("HexConv");
                        writer.WriteString("true");
                        writer.WriteEndAttribute();
                        //if (attribs) writer.WriteWhitespace("\r\n");
                        writer.WriteValue(XMLTree.stringToHex(current.Value));
                    }
                    else
                    {
                        //if (attribs) writer.WriteWhitespace("\r\n");
                        try
                        {
                            writer.WriteValue(sanitize(current.Value));
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
                        Tree nextElement = (Tree)current.tree[i];

                        writer.WriteWhitespace("\r\n");
                        for (int x = 0; x < depth; x++)
                        {
                            writer.WriteWhitespace(" ");
                        }

                        // Write Elements
                        writer.WriteStartElement((string)current.leafnames[i]);

                        // Recurse

                        writeIndentation = recurseWriteXML(nextElement, writer, depth + 1);

                        // Write End Element
                        if (writeIndentation == true)
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

        public static Tree readJSONFromString(string indata)
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
            catch (Newtonsoft.Json.JsonReaderException xyz)
            {
                //int i = 0;
            }
            catch (Exception)
            {
                //int i = 0;
            }
            return XMLTree.readXMLFromString(xml);
        }

        public static string sanitize(string instring)
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
            recurseWriteXML(outdata, newWriter, 1);
            newWriter.WriteEndElement();
            newWriter.WriteWhitespace("\r\n");
            newWriter.WriteEndDocument();
            newWriter.WriteWhitespace("\r\n");
            newWriter.Close();

            string xmlformat = textwriter.ToString();
            xmlformat = xmlformat.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n", "");
            return XmlToJSON(xmlformat);
        }

        public static string XmlToJSON(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            return XmlToJSON(doc);
        }

        public static string XmlToJSON(XmlDocument xmlDoc)
        {
            StringBuilder sbJSON = new StringBuilder();
            sbJSON.Append("{ ");
            XmlToJSONnode(sbJSON, xmlDoc.DocumentElement, true);
            sbJSON.Append("}");
            return sbJSON.ToString();
        }

        //  XmlToJSONnode:  Output an XmlElement, possibly as part of a higher array
        private static void XmlToJSONnode(StringBuilder sbJSON, XmlElement node, bool showNodeName)
        {
            if (showNodeName)
                sbJSON.Append("\"" + SafeJSON(node.Name) + "\": ");
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
                    sbJSON.Append(" \"" + SafeJSON(childname) + "\": [ ");
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
                    sbJSON.Append("\"" + SafeJSON(childname) + "\": ");
                sbJSON.Append("null");
            }
            else if (alChild is string)
            {
                if (showNodeName)
                    sbJSON.Append("\"" + SafeJSON(childname) + "\": ");
                string sChild = (string)alChild;
                sChild = sChild.Trim();
                sbJSON.Append("\"" + SafeJSON(sChild) + "\"");
            }
            else
                XmlToJSONnode(sbJSON, (XmlElement)alChild, showNodeName);
            sbJSON.Append(", ");
        }

        // Make a string safe for JSON
        private static string SafeJSON(string sIn)
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

        public static Tree readJson(string filename)
        {
            string infile = File.ReadAllText(filename);
            Tree converted = readJSONFromString(infile);
            return converted;
        }

        public static int writeJson(string filename, Tree outdata, string recordName)
        {
            int result = 1;
            string tmp = WriteJsonToString(outdata, recordName);
            File.WriteAllText(filename, tmp);
            return result;
        }
    }
}
