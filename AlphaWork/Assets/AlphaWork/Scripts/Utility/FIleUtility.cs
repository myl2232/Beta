using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public static class FileUtility
    {
        //Application.streamingAssetsPath
        //Application.dataPath
        //Application.persistentDataPath
        public static string GetDeviceFolderRoot()
        {
            return "";
        }
        //读资源配表
        public static string GetStreamingDest(string file)
        {
            return "";
        }

        public static void GetFilesInFolder(string folderFullName, ref List<string> files)
        {
            List<DirectoryInfo> folders = new List<DirectoryInfo>();
            GetFolders(folderFullName, ref folders);
            foreach(DirectoryInfo folder in folders)
            {
                foreach (FileInfo NextFile in folder.GetFiles())
                    files.Add(NextFile.Name);
            }
        }

        public static void GetFolders(string folder, ref List<DirectoryInfo> subFolders)
        {
            List<DirectoryInfo> tempfolders = new List<DirectoryInfo>();
            DirectoryInfo TheFolder = new DirectoryInfo(folder);
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                tempfolders.Add(NextFolder);
                subFolders.Add(NextFolder);
            }
            foreach(DirectoryInfo sub in tempfolders)
            {
                GetFolders(sub.FullName, ref subFolders);
            }
        }
    }
}
