//   Jophiel - Computer Forensics Collection, Analysis and Reporting Tool
//   Copyright (C) 2006-2014 by Eric Knight

//   Copyright (C) 2003-2019 Eric Knight

using System;
using System.Collections;

namespace FatumCore
{
    public class TreeFlatten
    {
        public Tree Flattened = null;

        public TreeFlatten(Tree DB)
        {
            ArrayList fields = new ArrayList();
            Flattened = new Tree();
            recurseSchema(DB, fields);
            fields.Clear();
        }

        ~TreeFlatten()
        {
            if (Flattened!=null)
            {
                Flattened.dispose();
                Flattened = null;
            }
        }

        private void recurseSchema(Tree currentNode, ArrayList fields)
        {
            if (currentNode != null)
            {
                int currentNodetreeCount = currentNode.tree.Count;

                for (int i = 0; i < currentNodetreeCount; i++)
                {
                    string currentLeafname = (string)currentNode.leafnames[i];
                    Tree currentTree = (Tree)currentNode.tree[i];

                    // Tricky logic- if the "value" of the Tree is not being used in the tuple,
                    //               we skip creating a column (these will appear as blanks)

                    if (currentLeafname!=null)
                    {
                        if (currentLeafname!="")
                        {
                            if (currentTree != null)
                            {
                                if (currentTree.leafnames.Count == 0)
                                {
                                    if (!isField(currentLeafname, fields))
                                    {
                                        if (!fields.Contains(currentLeafname))
                                        {
                                            if (!currentTree.Value.Equals(""))
                                            {
                                                fields.Add(currentLeafname);
                                                Flattened.addElement(currentLeafname, currentTree.Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    recurseSchema((Tree)currentNode.tree[i], fields);
                }
            }
        }

        private Boolean isField(String field, ArrayList fields)
        {
            Boolean found = false;

            int fieldsCount = 0;

            for (int i = 0; i < fieldsCount; i++)
            {
                String currentField = (string)fields[i];
                if (currentField.Equals(field))
                {
                    i = fieldsCount;
                    found = true;
                }
            }
            return found;
        }
    }
}
