using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite4Unity3d;

namespace AlphaWork
{
    public abstract class ITable
    {
        public abstract string KeyName
        {
            get;
            set;
        }
    }
}
