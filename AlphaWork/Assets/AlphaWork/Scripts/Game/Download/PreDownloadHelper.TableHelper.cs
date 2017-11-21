using GameFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlphaWork
{
    public partial class PreDownloadHelper
    {
        private static readonly string[] ColumnSplit = new string[] { "\t" };
        private const int ColumnCount = 2;

        private bool LoadDownloadTable(string tableName, object tableAsset, object userData)
        {
            TextAsset textAsset = tableAsset as TextAsset;
            if (textAsset == null)
            {
                Log.Warning("Dictionary asset '{0}' is invalid.", tableName);
                return false;
            }

            bool retVal = ParseTable(textAsset.text, userData);
            if (!retVal)
            {
                Log.Warning("Dictionary asset '{0}' parse failure.", tableName);
            }

            return retVal;
        }

        private bool ParseTable(string text, object userData)
        {
            try
            {
                string[] rowTexts = Utility.Text.SplitToLines(text);
                for (int i = 1; i < rowTexts.Length; i++)
                {
                    if (rowTexts[i].Length <= 0 || rowTexts[i][0] == '#')
                    {
                        continue;
                    }

                    string[] splitLine = rowTexts[i].Split(ColumnSplit, StringSplitOptions.None);
                    if (splitLine.Length != ColumnCount)
                    {
                        Log.Warning("Can not parse dictionary '{0}'.", text);
                        return false;
                    }

                    m_downloadFiles.Add(splitLine[0],splitLine[1]);

                }

                return true;
            }
            catch (Exception exception)
            {
                Log.Warning("Can not parse table '{0}' with exception '{1}'.", text, string.Format("{0}\n{1}", exception.Message, exception.StackTrace));
                return false;
            }
        }

        public void ReleaseTableAsset(object dictionaryAsset)
        {
            //m_ResourceComponent.UnloadAsset(dictionaryAsset);
        }


    }
}
