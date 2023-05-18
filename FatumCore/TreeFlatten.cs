﻿//   Fatum -- Metadata Processing Library
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

using System;
using System.Collections;

namespace Proliferation.Fatum
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
