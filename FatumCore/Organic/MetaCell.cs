using System;
using System.Collections.Generic;
using System.Text;

namespace FatumCore.Organic
{
    public interface MetaCell
    {
        void Process(MetaThought Thought);
        void ThoughtBridge(MetaOrgan MajorOrgan, MetaOrgan MinorOrgan);
    }
}
