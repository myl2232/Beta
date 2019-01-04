using System;
using System.Linq;
using System.IO;
using System.Security.Cryptography;

using UnityEngine;
using UnityEditor;

namespace SkillSystem
{
  class SkillFileList : IDisposable
  {
    GUIWrapper.ListView m_listView;
    Action m_onSelect;
    FileItem m_activeItem;

    class FileItem : GUIWrapper.ListView.IUserItem
    {
      string m_filePath;
      SkillFileList m_fileList;

      public SkillList TheSkillList
      {
        private set;
        get;
      }

      public string Name
      {
        set
        {

        }

        get
        {
          return m_filePath;
        }
      }

      void GUIWrapper.ListView.IUserItem.OnSelected()
      {
        m_fileList.m_activeItem = this;
        m_fileList.m_onSelect();
      }

      static bool IsFileContentSame(string pathA, string pathB)
      {
        using (var hash = HashAlgorithm.Create())
        {
          using (Stream fileA = File.Open(pathA, FileMode.Open),
            fileB = File.Open(pathB, FileMode.Open))
          {
            return BitConverter.ToString(hash.ComputeHash(fileA)) ==
              BitConverter.ToString(hash.ComputeHash(fileB));
          }
        }
      }

      public void DoSave(Func<bool> confirmSave)
      {
        var tmpFile = Path.GetTempFileName();

        SkillFileWriter.Save(tmpFile, TheSkillList);

        if (!IsFileContentSame(m_filePath, tmpFile))
        {
          if (confirmSave())
          {
            File.Delete(m_filePath);
            File.Move(tmpFile, m_filePath);

            GlobalObj<SkillDataCache>.Instance.Clear();
          }
        }

        File.Delete(tmpFile);
      }

      public void OnRemove()
      {
        DoSave(() => GUIWrapper.MessageBoxYesNo(m_filePath + " has changed. Do you want to save ?"));
      }

      bool GUIWrapper.ListView.IUserItem.CanRename
      {
        get
        {
          return false;
        }
      }

      bool GUIWrapper.ListView.IUserItem.CanStartDrag
      {
        get
        {
          return false;
        }
      }

      void GUIWrapper.ListView.IUserItem.Move(int newPos)
      {

      }

      public FileItem(string filePath, SkillList skillList, SkillFileList fileList)
      {
        m_fileList = fileList;
        m_filePath = filePath;
        TheSkillList = skillList;
      }

      public void Save()
      {
        DoSave(() => true);
      }
    }

    void TryAddNewFile(Func<string> filePathSelector,
      Func<string, SkillList> loader,
      Action<GUIWrapper.ListView.IUserItem> appender)
    {
      var path = filePathSelector();

      if (path.Length == 0 || m_listView.Any(item => (item as FileItem).Name == path))
        return;

      appender(new FileItem(path, loader(path), this));
    }

    static string OpenFileDialog()
    {
      return EditorUtility.OpenFilePanel("Open", Application.dataPath, SkillFileWriter.SkillFileExt);
    }

    static string NewFileDialog()
    {
      return EditorUtility.SaveFilePanel("New", Application.dataPath, "SkillList", SkillFileWriter.SkillFileExt);
    }

    void OnAdd(Action<GUIWrapper.ListView.IUserItem> appender)
    {
      GUIWrapper.PopupContextMenu(addMenuItem =>
      {
        addMenuItem("New...", () => TryAddNewFile(NewFileDialog,
          path =>
          {
            var l = new SkillList();
            SkillFileWriter.Save(path, l);
            return l;
          },
          appender));

        addMenuItem("Open...", () => TryAddNewFile(OpenFileDialog, SkillFileReader.Open, appender));
      });
    }

    public SkillFileList(Action onSelect, Action onDeselect)
    {
      m_onSelect = onSelect;

      m_listView = new GUIWrapper.ListView(appender => { },

        "Add File...",

        "Remove File",

        OnAdd,

        () =>
        {
          m_activeItem = null;
          onDeselect();
        },

        () =>
        {
          if (null != m_activeItem)
            GUIWrapper.Button("Save", m_activeItem.Save);
        });
    }

    public void OnGUI()
    {
      m_listView.OnGUI();
    }

    public void ForEachSkill(Action<Skill> fn)
    {
      foreach (FileItem item in m_listView)
        item.TheSkillList.ForEachChild(fn);
    }

    public SkillList ActiveSkillList
    {
      get
      {
        return m_activeItem.TheSkillList;
      }
    }

    public void Dispose()
    {
      foreach (FileItem item in m_listView)
        item.OnRemove();
    }
  }
}
