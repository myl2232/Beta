using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphaWork
{
    public sealed class StructureBehaviourCallback
    {
        private readonly OccupyCallback m_OccupyCallback;
        private readonly UnOccupyCallback m_UnOccupyCallback;

        public StructureBehaviourCallback():this(null,null)
        { }

        public StructureBehaviourCallback(OccupyCallback enterCallback, UnOccupyCallback leaveCallback)
        {
            m_OccupyCallback = enterCallback;
            m_UnOccupyCallback = leaveCallback;
        }

        public OccupyCallback occupyCallback
        {
            get
            {
                return m_OccupyCallback;
            }
        }

        public UnOccupyCallback unOccupyCallback
        {
            get
            {
                return m_UnOccupyCallback;
            }
        }
    }
}
