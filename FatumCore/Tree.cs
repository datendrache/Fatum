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

namespace Proliferation.Fatum
{
    public class Tree
    {
        public string Value = "";
        public string Name = "";

        public List<string> attributes = new List<string>(0);
        public List<string> attributevalues = new List<string>(0);

        public List<string> leafnames = new List<string>(0);
        public List<Tree> tree = new List<Tree>(0);

        public Boolean DEALLOCATED = false;

        // =================================================================
        // Constructors:  "Simple data."
        // =================================================================

        ~Tree()
        {
            if (tree != null)
            {
                if (tree.Count > 0)
                {
                    foreach (Tree current in tree)
                    {
                        if (current != null)
                        {
                            current.dispose();
                        }
                    }
                    tree.Clear();
                }
                tree = null;
            }
            if (attributes != null)
            {
                attributes.Clear();
                attributes = null;
            }
            if (attributevalues != null)
            {
                attributevalues.Clear();
                attributevalues = null;
            }
            if (leafnames != null)
            {
                leafnames.Clear();
                leafnames = null;
            }
        }

        public Tree()
        {

        }

        public Tree(int Capacity)
        {
            leafnames.Capacity = Capacity;
            tree.Capacity = Capacity;
        }

        public Tree(string newValue)
        {
            Value = newValue;
        }

        // =================================================================
        // Function for setting the Tree value
        // =================================================================

        public void setElement(string newValue)
        {
            Value = newValue;
        }

        // =================================================================
        // Functions for manipulating attributes and their values
        // =================================================================

        public void addAttribute(string attrib, string newValue)
        {
            if (attributes != null)
            {
                lock (attributes)
                {
                    if (attributevalues != null)
                    {
                        if (attributevalues != null)
                        {
                            lock (attributevalues)
                            {
                                attributes.Add(attrib);
                                attributevalues.Add(newValue);
                            }
                        }
                    }
                }
            }

        }

        public void setAttribute(string attrib, string newValue)
        {
            if (attributes != null)
            {
                int i = attributes.IndexOf(attrib);
                if (i != -1)
                {
                    if (attributevalues != null)
                    {
                        lock (attributevalues)
                        {
                            attributevalues.Insert(i, newValue);
                        }
                    }
                }
                else
                {
                    addAttribute(attrib, newValue);
                }
            }
        }

        public string getAttribute(string attrib)
        {
            if (attributes != null)
            {
                if (attributevalues != null)
                {
                    int i = attributes.IndexOf(attrib);
                    if (i != -1)
                    {
                        return (attributevalues[i]);
                    }
                }
            }
            return null;
        }

        public Boolean removeAttribute(string attrib)
        {
            if (attributes != null)
            {
                if (attributevalues != null)
                {
                    int i = attributes.IndexOf(attrib);
                    if (i != -1)
                    {
                        lock (attributes)
                        {
                            lock (attributevalues)
                            {
                                attributevalues.RemoveAt(i);
                                attributes.RemoveAt(i);
                            }
                        }
                        return (true);
                    }
                }
            }
            return false;
        }

        // =================================================================
        // Functions for managing tree nodes
        // =================================================================

        public void addNode(Tree newrecord, string recordname)
        {
            if (newrecord == null)
            {
                throw new TreeNullNodeException();
            }
            else
            {
                if (tree != null)
                {
                    if (leafnames != null)
                    {
                        lock (tree)
                        {
                            lock (leafnames)
                            {
                                lock (newrecord)
                                {
                                    tree.Add(newrecord);
                                    leafnames.Add(recordname);
                                }
                            }
                        }
                    }
                }
            }
        }

        public Boolean deleteNode(string target)
        {
            try
            {
                lock (target)
                {
                    Boolean result = true;

                    if (target != null)
                    {
                        if (leafnames != null)
                        {
                            if (tree != null)
                            {
                                lock (leafnames)
                                {
                                    lock (tree)
                                    {
                                        int index = leafnames.IndexOf(target);
                                        if (index != -1)
                                        {
                                            Tree disposeLeaf = (Tree)tree[index];
                                            leafnames.RemoveAt(index);
                                            tree.RemoveAt(index);
                                            disposeLeaf.dispose();
                                        }
                                        else
                                        {
                                            result = false;
                                        }
                                    }
                                }
                            }
                        }

                    }
                    return (result);
                }
            }
            catch (Exception)
            {
                throw new TreeNullNodeException();
            }
        }

        public void deleteNode(int target)
        {
            try
            {
                if (leafnames != null)
                {
                    if (tree != null)
                    {
                        Tree disposeLeaf = (Tree)tree[target];

                        if (disposeLeaf != null)
                        {
                            lock (leafnames)
                            {
                                lock (tree)
                                {
                                    leafnames.RemoveAt(target);
                                    tree.RemoveAt(target);
                                    disposeLeaf.dispose();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new TreeNullNodeException();
            }
        }

        public Tree findNode(string name)
        {
            if (leafnames != null)
            {
                if (tree != null)
                {
                    int index = leafnames.IndexOf(name);

                    if (index != -1)
                    {
                        return ((Tree)tree[index]);
                    }
                }
            }
            return (null);
        }

        // =================================================================
        // Functions for simplification of a process
        // =================================================================

        public void addElement(string elementname, string elementdata)
        {
            Tree tmpElement = new Tree(elementdata);
            if (tmpElement.leafnames != null)
            {
                if (tmpElement.tree != null)
                {
                    tmpElement.leafnames.Capacity = 0;
                    tmpElement.tree.Capacity = 0;

                    if (this.tree != null)
                    {
                        if (this.leafnames != null)
                        {
                            lock (this.tree)
                            {
                                lock (this.leafnames)
                                {
                                    this.addNode(tmpElement, elementname);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void setElement(string elementname, string elementdata)
        {
            if (leafnames != null)
            {
                if (tree != null)
                {
                    Tree check = null;

                    int leafnamescount = leafnames.Count;

                    for (int i = 0; i < leafnames.Count; i++)
                    {
                        if (leafnames[i].CompareTo(elementname) == 0)
                        {
                            check = (Tree)tree[i];
                            i = leafnamescount;
                        }
                    }

                    if (check == null)
                    {
                        this.addElement(elementname, elementdata);
                    }
                    else
                    {
                        if (check.Value != null)
                        {
                            lock (check.Value)
                            {
                                check.Value = elementdata;
                            }
                        }
                        else
                        {
                            check.Value = elementdata;
                        }
                    }
                }
            }
        }

        public string getElement(string elementname)
        {
            Tree tmpElement = this.findNode(elementname);
            if (tmpElement != null)
            {
                if (tmpElement.Value != null)
                {
                    return tmpElement.Value;
                }
            }
            return "";
        }

        // ====================================================================
        // Functions for memory management and maintenance of Tree objects
        // ====================================================================

        public void dispose()
        {
            if (!DEALLOCATED)
            {
                lock (this)
                {
                    if (tree != null)
                    {
                        int treesize = tree.Count;

                        for (int i = 0; i < treesize; i++)
                        {
                            Tree disposeTree = (Tree)tree[i];
                            if (disposeTree.DEALLOCATED == true)
                            {
                                i = treesize;
                            }
                            disposeTree.dispose();
                        }
                    }

                    if (attributes != null)
                    {
                        lock (attributes)
                        {
                            attributes.Clear();
                            attributes = null;
                        }
                    }

                    if (attributevalues != null)
                    {
                        lock (attributevalues)
                        {
                            attributevalues.Clear();
                            attributevalues = null;
                        }
                    }

                    if (leafnames != null)
                    {
                        lock (leafnames)
                        {
                            leafnames.Clear();
                            leafnames = null;
                        }
                    }

                    if (tree != null)
                    {
                        lock (tree)
                        {
                            tree.Clear();
                            tree = null;
                        }
                    }

                    Name = null;
                    Value = null;
                    DEALLOCATED = true;
                }
            }
        }

        public Tree Duplicate()
        {
            try
            {
                return recurseCopy(this);
            }
            catch (Exception)
            {
                throw new TreeNullNodeException();
            }
        }

        public static void mergeNode(Tree source, Tree destination)
        {
            if (source != null)
            {
                if (destination != null)
                {
                    Tree copy = source.Duplicate();
                    if (copy.leafnames != null)
                    {
                        if (copy.tree != null)
                        {
                            if (destination.leafnames != null)
                            {
                                if (destination.tree != null)
                                {
                                    for (int i = 0; i < copy.leafnames.Count; i++)
                                    {
                                        destination.leafnames.Add(copy.leafnames[i].ToString());
                                        destination.tree.Add(copy.tree[i].Duplicate());
                                    }
                                }
                            }
                        }
                    }

                    if (copy.attributes != null)
                    {
                        if (copy.attributevalues != null)
                        {
                            if (destination.attributes != null)
                            {
                                if (destination.attributevalues != null)
                                {
                                    for (int i = 0; i < copy.attributes.Count; i++)
                                    {
                                        destination.attributes.Add(copy.attributes[i].ToString());
                                        destination.attributevalues.Add(copy.attributevalues[i].ToString());
                                    }
                                }
                            }
                        }
                    }
                    copy.dispose();
                }
            }
        }

        public static Tree recurseCopy(Tree Old)
        {
            Tree result = new Tree(1);  // Create a low capacity Tree and expand to match original
            if (result.attributes != null)
            {
                if (Old.attributes != null)
                {
                    result.attributes.Capacity = Old.attributes.Count;
                }
            }

            if (result.attributevalues != null)
            {
                if (Old.attributevalues != null)
                {
                    result.attributevalues.Capacity = Old.attributevalues.Count;
                }
            }
            if (result.tree != null)
            {
                if (Old.tree != null)
                {
                    result.tree.Capacity = Old.tree.Count;
                }
            }
            if (result.leafnames != null)
            {
                if (Old.leafnames != null)
                {
                    result.leafnames.Capacity = Old.leafnames.Count;
                }
            }

            result.Value = Old.Value;

            if (Old.attributes != null)
            {
                if (Old.attributevalues != null)
                {
                    int Oldpropertysize = Old.attributes.Count;
                    for (int i = 0; i < Oldpropertysize; i++)
                    {
                        result.addAttribute(Old.attributes[i].ToString(), Old.attributevalues[i].ToString());
                    }
                }
            }

            if (Old.leafnames != null)
            {
                if (Old.tree != null)
                {
                    int Oldleafnamessize = Old.leafnames.Count;
                    for (int i = 0; i < Oldleafnamessize; i++)
                    {
                        Tree oldNode = recurseCopy((Tree)Old.tree[i]);
                        result.addNode(oldNode, Old.leafnames[i].ToString());
                    }
                }
            }

            return (result);
        }
    }
}
