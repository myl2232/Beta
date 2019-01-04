using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine.Assertions;

namespace SkillSystem
{
  static partial class GUIWrapper
  {
    public class TreeView
    {
      class Impl : UnityEditor.IMGUI.Controls.TreeView
      {
        struct ItemBinder
        {
          public int Depth
          {
            set;
            get;
          }

          public IUserItem UserItem
          {
            set;
            get;
          }
        }

        class ContextMenuBuilder : IContextMenuBuilder
        {
          int m_itemId;
          Impl m_impl;
          GenericMenu m_menu = new GenericMenu();

          void IContextMenuBuilder.BuildRename()
          {
            m_menu.AddItem(new GUIContent("Rename"), false, () => m_impl.Rename(m_itemId));
          }

          void IContextMenuBuilder.BuildRemove()
          {
            m_menu.AddItem(new GUIContent("Delete"), false, () =>
            {
              if (GUIWrapper.MessageBoxYesNo("Delete \"" + m_impl.IdToItem(m_itemId).displayName + "\" ?"))
                m_impl.Remove(m_itemId);
            });
          }

          void IContextMenuBuilder.BuildAdd(string caption, Func<IUserItem> userItemFactory)
          {
            m_menu.AddItem(new GUIContent("Add/" + caption), false,
              () => m_impl.AddChild(m_itemId, userItemFactory()));
          }

          void IContextMenuBuilder.BuildCustomCommand(string caption, Action action)
          {
            m_menu.AddItem(new GUIContent(caption), false, () => action());
          }

          public ContextMenuBuilder(int itemId, Impl impl)
          {
            m_itemId = itemId;
            m_impl = impl;
          }

          public void Show()
          {
            m_menu.ShowAsContext();
          }
        }

        SearchField m_searchField = new SearchField();
        List<ItemBinder> m_binders = new List<ItemBinder>();

        TreeViewItem IdToItem(int itemId)
        {
          return FindItem(itemId, rootItem);
        }

        int EndOfDescendants(int itemId)
        {
          var item = IdToItem(itemId);

          while (item.hasChildren)
            item = item.children.Last();

          return item.id + 1;
        }

        void Rename(int itemId)
        {
          BeginRename(IdToItem(itemId));
        }

        void Remove(int itemId)
        {
          var parent = IdToItem(itemId).parent;
          if (null != parent)
            m_binders[itemId].UserItem.OnRemovedFromParent(m_binders[parent.id].UserItem);

          m_binders.RemoveRange(itemId, EndOfDescendants(itemId) - itemId);

          Reload();

          if (itemId < m_binders.Count)
            SelectionChanged(GetSelection());
          else if (m_binders.Count > 0)
            SetSelection(new[] { m_binders.Count - 1 }, TreeViewSelectionOptions.FireSelectionChanged);
        }

        void AddChild(int itemId, IUserItem userItem)
        {
          var newId = EndOfDescendants(itemId);
          m_binders.Insert(newId, new ItemBinder { Depth = m_binders[itemId].Depth + 1, UserItem = userItem });

          m_binders[newId].UserItem.OnAddedToParent(m_binders[itemId].UserItem);

          Reload();

          if (!IsExpanded(itemId))
            SetExpanded(itemId, true);

          SetSelection(new[] { newId }, TreeViewSelectionOptions.FireSelectionChanged);

          Rename(newId);
        }

        protected override TreeViewItem BuildRoot()
        {
          var treeItems = new List<TreeViewItem>();
          m_binders.ForEach(binder => treeItems.Add(
            new TreeViewItem(m_binders.IndexOf(binder), binder.Depth, binder.UserItem.Name)));

          var root = new TreeViewItem(-1, -1, "Root");
          SetupParentsAndChildrenFromDepths(root, treeItems);
          return root;
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
          return false;
        }

        protected override bool CanRename(TreeViewItem item)
        {
          return false;
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
          m_binders[args.itemID].UserItem.Name = args.newName;
          Reload();
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
          Assert.IsTrue(selectedIds.Count == 1);

          var id = selectedIds[0];

          var ancestor = IdToItem(id).parent;

          var ancestors = new List<IUserItem>();
          while (rootItem != ancestor)
          {
            ancestors.Add(m_binders[ancestor.id].UserItem);
            ancestor = ancestor.parent;
          }

          m_binders[id].UserItem.OnSelected(ancestors);
        }

        protected override void ContextClickedItem(int id)
        {
          if (id != GetSelection()[0])
            return;

          var menu = new ContextMenuBuilder(id, this);
          m_binders[id].UserItem.OnContextClicked(menu);
          menu.Show();
        }

        public Impl(Action<Action<int, IUserItem>> populator)
          : base(new TreeViewState())
        {
          populator((depth, userItem) => m_binders.Add(new ItemBinder { Depth = depth, UserItem = userItem }));
          Reload();
        }

        public void OnGUI()
        {
          GUILayout.BeginHorizontal(EditorStyles.toolbar);
          GUILayout.Space(100);
          GUILayout.FlexibleSpace();
          searchString = m_searchField.OnToolbarGUI(searchString);
          GUILayout.EndHorizontal();

          Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
          OnGUI(rect);
        }
      }

      Impl m_impl;

      public void OnGUI()
      {
        m_impl.OnGUI();
      }

      public interface IContextMenuBuilder
      {
        void BuildRename();
        void BuildRemove();
        void BuildAdd(string caption, Func<IUserItem> userItemFactory);
        void BuildCustomCommand(string caption, Action action);
      }

      public interface IUserItem
      {
        string Name
        {
          set;
          get;
        }

        void OnSelected(List<IUserItem> ancestors);
        void OnContextClicked(IContextMenuBuilder builder);

        void OnAddedToParent(IUserItem parent);
        void OnRemovedFromParent(IUserItem parent);
      }

      public TreeView(Action<Action<int, IUserItem>> populator)
      {
        m_impl = new Impl(populator);
      }
    }
  }
}
