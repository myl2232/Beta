using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace AlphaWork
{
    public class UGridItem : IUListItemView
    {        
        public override void SetData(object data)
        {
            transform.Find("Text").transform.GetComponent<Text>().text = (string)data;
            
        }
        public string GetItemText()
        {
            return transform.Find("Text").transform.GetComponent<Text>().text;
        }
    }
}
