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

namespace Proliferation.Fatum
{
    public sealed class XMLTree
    {
        // =======================================================================
        // XML File Read and Write
        // =======================================================================

        public static Tree readXML(string filename)
        {
            StreamReader infile = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            XmlTextReader newReader = new XmlTextReader(infile);
            Tree newRoot = new Tree();
            List<string> LabelStack = new List<string>();

            recurseXML(newReader, LabelStack, newRoot, true, false, 0, filename);

            if (LabelStack.Count > 0)
            {
                Console.Out.WriteLine("Stack underflow by " + LabelStack.Count.ToString());
            }
            LabelStack.Clear();

            infile.Close();
            return newRoot;
        }

        public static Tree readXML(Stream instream, string filename)
        {
            StreamReader infile = new StreamReader(instream);

            XmlTextReader newReader = new XmlTextReader(infile);
            Tree newRoot = new Tree();
            List<string> LabelStack = new List<string>();

            recurseXML(newReader, LabelStack, newRoot, true, false, 0, filename);

            infile.Close();
            return newRoot;
        }

        public static Tree readXMLFromString(string indata)
        {
            StringReader newStringReader = new StringReader(indata);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.MaxCharactersFromEntities = 1024;
            XmlReader newReader = XmlReader.Create(newStringReader, settings);

            Tree newRoot = new Tree();
            List<string> LabelStack = new List<string>();

            if (newReader.HasAttributes)
            {
                if (newReader.MoveToFirstAttribute())
                    do
                    {
                        newRoot.addAttribute(newReader.Name, newReader.Value);
                    } while (newReader.MoveToNextAttribute());
            }

            try
            {
                recurseXML(newReader, LabelStack, newRoot, true, false, 0, "(string)");
            }
            catch (XmlException)
            {
                throw new TreeDataAccessInvalidDataException();
            }

            newReader.Close();
            return newRoot;
        }

        public static Boolean mustConvertValue(string instring)
        {
            Boolean result = false;

            int instringLength = instring.Length;

            for (int i = 0; i < instringLength; i++)
            {
                char tmp = instring[i];
                if (tmp < 32)
                {
                    if (tmp != '\t')
                    {
                        if (tmp != '\n')
                        {
                            if (tmp != '\r')
                            {
                                if (tmp != '<')
                                {
                                    result = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (tmp == '<')
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        public static string stringToHex(string instring)
        {
            string outstring;
            ASCIIEncoding encoding = new ASCIIEncoding();
            outstring = FatumLib.convertBytesTostring(encoding.GetBytes(instring));
            return outstring;
        }

        public static void recurseXML(XmlReader newReader, List<string> LabelStack, Tree currentNode, Boolean root, Boolean BinHex, int level, string filename)
        {
            if (level >= 200)  // This is to prevent Stack Overflows because, for whatever reason, C# doesn't know how to handle their own.
            {
                TreeDataAccessInvalidDataException tothrow = new TreeDataAccessInvalidDataException()
                {
                    linenumber = 0,
                    character = 0
                };

                throw tothrow;
            }

            try
            {
                do
                {
                    if (newReader.NodeType != XmlNodeType.Whitespace)
                    {
                        if (newReader.NodeType == XmlNodeType.Element)
                        {
                            if (newReader.IsEmptyElement)
                            {
                                Tree newNode = new Tree();
                                currentNode.addNode(newNode, newReader.Name);
                            }
                            else
                            {
                                LabelStack.Add(newReader.Name);
                                if (!root)
                                {
                                    Tree newElement = new Tree();
                                    Boolean isBinHex = false;

                                    currentNode.addNode(newElement, newReader.Name);
                                    isBinHex = checkAttributes(newReader, newElement);
                                    newReader.Read();
                                    recurseXML(newReader, LabelStack, newElement, false, isBinHex, level + 1, filename);
                                }
                                else
                                {
                                    Boolean isBinHex = false;
                                    currentNode.Value = newReader.Name;
                                    isBinHex = checkAttributes(newReader, currentNode);
                                    newReader.Read();
                                    recurseXML(newReader, LabelStack, currentNode, false, isBinHex, level + 1, filename);
                                }
                            }
                        }
                        else
                        {
                            if (newReader.NodeType == XmlNodeType.Text)
                            {
                                if (BinHex)
                                {
                                    ASCIIEncoding enc = new ASCIIEncoding();
                                    byte[] currentConverted = FatumLib.hexToBytes(newReader.Value);

                                    currentNode.setElement(enc.GetString(currentConverted));
                                }
                                else
                                {
                                    currentNode.setElement(newReader.Value);
                                }
                            }
                            else
                            {
                                if (newReader.NodeType == XmlNodeType.EndElement)
                                {
                                    string tmp = (string)LabelStack[LabelStack.Count - 1];
                                    LabelStack.RemoveAt(LabelStack.Count - 1);

                                    if (tmp.CompareTo(newReader.Name) == 0)
                                    {
                                        return;
                                    }
                                }
                                else
                                {
                                    if (newReader.NodeType == XmlNodeType.CDATA)
                                    {
                                        currentNode.setElement("CDATA", newReader.Value);
                                    }
                                }
                            }
                        }
                    }
                }
                while (newReader.Read());
            }
            catch (System.StackOverflowException)
            {
                TreeDataAccessInvalidDataException tothrow = new TreeDataAccessInvalidDataException();
                tothrow.linenumber = 0;
                tothrow.character = 0;
                throw tothrow;
            }
            catch (XmlException)
            {
                TreeDataAccessInvalidDataException tothrow = new TreeDataAccessInvalidDataException();
                tothrow.linenumber = 0;
                tothrow.character = 0;
                throw tothrow;
            }
        }

        private static Boolean checkAttributes(XmlReader newReader, Tree newElement)
        {
            Boolean isBinHex = false;

            if (newReader.HasAttributes)
            {
                for (int i = 0; i < newReader.AttributeCount; i++)
                {
                    newReader.MoveToAttribute(i);

                    if (newReader.Name.Equals("HexConv"))
                    {
                        isBinHex = true;
                    }
                    else
                    {
                        newElement.addAttribute(newReader.Name, newReader.Value);
                    }
                }
            }
            return isBinHex;
        }
    }
}
