//   Jophiel - Computer Forensics Collection, Analysis and Reporting Tool
//   Copyright (C) 2003-2019 Eric Knight

namespace FatumCore
{
    public class TreeDataAccessInvalidDataException : System.Exception
    {
        public int linenumber = 0;
        public int character = 0;
    }
}
