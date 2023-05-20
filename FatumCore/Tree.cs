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
    public sealed class Tree
    {
        public string? Value = "";
        public string? Name = "";

        public List<string>? attributes = new(0);
        public List<string>? attributevalues = new(0);

        public List<string>? leafnames = new(0);
        public List<Tree>? tree = new(0);

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
                        current?.Dispose();
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

        public void SetElement(string newValue)
        {
            Value = newValue;
        }

        // =================================================================
        // Functions for manipulating attributes and their values
        // =================================================================

        public void AddAttribute(string attrib, string newValue)
        {
            if (attributes != null)
            {
                lock (attributes)
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

        public void SetAttribute(string attrib, string newValue)
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
                    AddAttribute(attrib, newValue);
                }
            }
        }

        public string? GetAttribute(string attrib)
        {
            if (attributes != null && attributevalues != null)
            {
                int i = attributes.IndexOf(attrib);
                if (i != -1)
                {
                    return (attributevalues[i]);
                }
            }
            return null;
        }

        public Boolean RemoveAttribute(string attrib)
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

        public void AddNode(Tree newrecord, string recordname)
        {
            if (newrecord == null)
            {
                throw new TreeNullNodeException();
            }
            else
            {
                if (tree != null && leafnames != null)
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

        public bool? DeleteNode(string target)
        {
            try
            {
                lock (target)
                {
                    bool? result = true;

                    if (target != null && leafnames != null && tree != null)
                    {
                        lock (leafnames)
                        {
                            lock (tree)
                            {
                                int index = leafnames.IndexOf(target);
                                if (index != -1)
                                {
                                    Tree disposeLeaf = tree[index];
                                    leafnames.RemoveAt(index);
                                    tree.RemoveAt(index);
                                    disposeLeaf.Dispose();
                                }
                                else
                                {
                                    result = false;
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

        public void DeleteNode(int target)
        {
            try
            {
                if (leafnames != null && tree != null)
                {
                    Tree disposeLeaf = tree[target];

                    if (disposeLeaf != null)
                    {
                        lock (leafnames)
                        {
                            lock (tree)
                            {
                                leafnames.RemoveAt(target);
                                tree.RemoveAt(target);
                                disposeLeaf.Dispose();
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

        public Tree? FindNode(string name)
        {
            if (leafnames != null && tree != null)
            {
                int index = leafnames.IndexOf(name);

                if (index != -1)
                {
                    return (tree[index]);
                }
            }
            return null;
        }

        // =================================================================
        // Functions for simplification of a process
        // =================================================================

        public void AddElement(string elementname, string elementdata)
        {
            Tree tmpElement = new(elementdata);
            if (tmpElement.leafnames != null && tmpElement.tree != null)
            {
                tmpElement.leafnames.Capacity = 0;
                tmpElement.tree.Capacity = 0;

                if (tree != null && this.leafnames != null)
                {
                    lock (tree)
                    {
                        lock (leafnames)
                        {
                            AddNode(tmpElement, elementname);
                        }
                    }
                }
            }
        }

        public void SetElement(string elementname, string elementdata)
        {
            if (leafnames != null && tree != null)
            {
                Tree? check = null;

                int leafnamescount = leafnames.Count;

                for (int i = 0; i < leafnames.Count; i++)
                {
                    if (leafnames[i].CompareTo(elementname) == 0)
                    {
                        check = tree[i];
                        i = leafnamescount;
                    }
                }

                if (check == null)
                {
                    AddElement(elementname, elementdata);
                }
                else
                {
                    check.Value = elementdata;
                }
            }
        }

        public string GetElement(string elementname)
        {
            Tree? tmpElement = FindNode(elementname);
            if (tmpElement != null && tmpElement.Value != null)
            {
                return tmpElement.Value;
            }
            return "";
        }

        // ====================================================================
        // Functions for memory management and maintenance of Tree objects
        // ====================================================================

        public void Dispose()
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
                            Tree disposeTree = tree[i];
                            if (disposeTree.DEALLOCATED)
                            {
                                i = treesize;
                            }
                            disposeTree.Dispose();
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
                return RecurseCopy(this);
            }
            catch (Exception)
            {
                throw new TreeNullNodeException();
            }
        }

        public static void MergeNode(Tree source, Tree destination)
        {
            if (source != null && destination != null)
            {
                Tree copy = source.Duplicate();
                if (copy.leafnames != null && copy.tree != null && destination.leafnames != null && destination.tree != null)
                {
                    for (int i = 0; i < copy.leafnames.Count; i++)
                    {
                        destination.leafnames.Add(copy.leafnames[i].ToString());
                        destination.tree.Add(copy.tree[i].Duplicate());
                    }
                }

                if (copy.attributes != null && copy.attributevalues != null && destination.attributes != null && destination.attributevalues != null)
                {
                    for (int i = 0; i < copy.attributes.Count; i++)
                    {
                        destination.attributes.Add(copy.attributes[i].ToString());
                        destination.attributevalues.Add(copy.attributevalues[i].ToString());
                    }
                }
                copy.Dispose();
            }
        }

        public static Tree RecurseCopy(Tree Old)
        {
            Tree result = new(1);  // Create a low capacity Tree and expand to match original
            if (result.attributes != null && Old.attributes != null)
            {
                result.attributes.Capacity = Old.attributes.Count;
            }

            if (result.attributevalues != null && Old.attributevalues != null)
            {
                result.attributevalues.Capacity = Old.attributevalues.Count;
            }
            if (result.tree != null && Old.tree != null)
            {
                result.tree.Capacity = Old.tree.Count;
            }
            if (result.leafnames != null && Old.leafnames != null)
            {
                result.leafnames.Capacity = Old.leafnames.Count;
            }

            result.Value = Old.Value;

            if (Old.attributes != null && Old.attributevalues != null)
            {
                int Oldpropertysize = Old.attributes.Count;
                for (int i = 0; i < Oldpropertysize; i++)
                {
                    result.AddAttribute(Old.attributes[i].ToString(), Old.attributevalues[i].ToString());
                }
            }

            if (Old.leafnames != null && Old.tree != null)
            {
                int Oldleafnamessize = Old.leafnames.Count;
                for (int i = 0; i < Oldleafnamessize; i++)
                {
                    Tree oldNode = RecurseCopy(Old.tree[i]);
                    result.AddNode(oldNode, Old.leafnames[i].ToString());
                }
            }
            return (result);
        }
    }
}
