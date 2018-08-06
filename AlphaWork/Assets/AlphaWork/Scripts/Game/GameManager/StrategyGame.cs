using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphaWork
{
    public class StrategyGame : GameBase
    {
        public override GameMode GameMode
        {
            get { return GameMode.Strategy; }
        }
    }
}
