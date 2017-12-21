using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphaWork
{
    public delegate void OccupyCallback(object asset, string assetName,object userData, 
        bool initialized);
    public delegate void UnOccupyCallback(object asset, string assetName, object userData, 
        bool destroy);
}
