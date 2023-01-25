using System;
using System.Collections.Generic;
using System.Text;

namespace Metatron
{
    public interface MetaCell
    {
        void Process(MetaThought Thought);
        void ThoughtBridge(MetaOrgan MajorOrgan, MetaOrgan MinorOrgan);
    }
}
