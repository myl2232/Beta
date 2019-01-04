using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;
using UnityEditor.IMGUI.Controls;

using CellView = System.Action<SkillSystem.GUIWrapper.ListView.IUserItem, UnityEngine.Rect>;
using ColumnAppender = System.Action<string /*caption*/,
  float /*relativeWidth*/,
  System.Action<SkillSystem.GUIWrapper.ListView.IUserItem, UnityEngine.Rect>>;

namespace SkillSystem
{
  static partial class GUIWrapper
  {
    public class ListView : IEnumerable<ListView.IUserItem>
    {
      Impl m_impl;

      class Impl : UnityEditor.IMGUI.Controls.TreeView
      {
        interface IRowView
        {
          void OnGUI(Impl treeView, RowGUIArgs args);
        }

        class SimpleRowView : IRowView
        {
          void IRowView.OnGUI(Impl treeView, RowGUIArgs args)
          {
            treeView.DefaultRowGUI(args);
          }
        }

        class MultiColumnRowView : IRowView
        {
          List<CellView> m_cellViews;

          void IRowView.OnGUI(Impl treeView, RowGUIArgs args)
          {
            var oldRect = args.rowRect;
            args.rowRect = args.GetCellRect(0);
            treeView.DefaultRowGUI(args);
            args.rowRect = oldRect;

            var userItem = treeView.m_binders[args.item.id];

            for (var i = 1; i < args.GetNumVisibleColumns(); ++i)
            {
              var rect = args.GetCellRect(i);
              if (rect.width > 0)
                m_cellViews[i](userItem, rect);
            }
          }

          public MultiColumnRowView(List<CellView> cellViews)
          {
            m_cellViews = cellViews;
          }
        }

        class AddRemoveButtons
        {
          TwoStateButton m_addButton;
          TwoStateButton m_removeButton;
          Action m_additionalGUIAfterRemoveButton;
          Action m_onDeselect;

          public AddRemoveButtons(string addCaption, string removeCaption,
            Impl listViewImpl,
            Action<Action<IUserItem>> onAddItem,
            Action onDeselect,
            Action additionalGUIAfterRemoveButton)
          {
            m_addButton = new TwoStateButton(addCaption,
              () => onAddItem(listViewImpl.Add))
            { Enabled = true };

            m_removeButton = new TwoStateButton(removeCaption, listViewImpl.RemoveSelected);
            m_additionalGUIAfterRemoveButton = additionalGUIAfterRemoveButton;
            m_onDeselect = onDeselect;
          }

          public void OnDeselect()
          {
            m_removeButton.Enabled = false;
            m_onDeselect();
          }

          public void OnSelect()
          {
            m_addButton.Enabled = true;
            m_removeButton.Enabled = true;
          }

          public void OnBeginRename()
          {
            m_addButton.Enabled = false;
            m_removeButton.Enabled = false;
          }

          public void OnEndRename()
          {
            OnSelect();
          }

          public void OnGUI()
          {
            using (new HorizontalGroup())
            {
              m_addButton.OnGUI();
              m_removeButton.OnGUI();
              m_additionalGUIAfterRemoveButton();
            }
          }
        }

        AddRemoveButtons m_addRemoveButtons;
        TreeViewItemIdToUserObject<IUserItem> m_binders = new TreeViewItemIdToUserObject<IUserItem>();
        IRowView m_rowView;

        protected override TreeViewItem BuildRoot()
        {
          var treeItems = new List<TreeViewItem>();
          m_binders.ForEach((id, obj) => treeItems.Add(new TreeViewItem(id, 0, obj.Name)));

          var root = new TreeViewItem(-1, -1, "Root");
          SetupParentsAndChildrenFromDepths(root, treeItems);
          return root;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
          Assert.IsTrue(selectedIds.Count == 1);
          m_binders[selectedIds[0]].OnSelected();
          m_addRemoveButtons.OnSelect();
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
          return false;
        }

        protected override bool CanRename(TreeViewItem item)
        {
          if(m_binders[item.id].CanRename)
          {
            m_addRemoveButtons.OnBeginRename();
            return true;
          }

          return false;
        }

        protected override void DoubleClickedItem(int id)
        {
          m_addRemoveButtons.OnSelect();
          base.DoubleClickedItem(id);
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
          m_binders[args.itemID].Name = args.newName;
          Reload();
          m_addRemoveButtons.OnEndRename();
        }

        void DefaultRowGUI(RowGUIArgs args)
        {
          base.RowGUI(args);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
          m_rowView.OnGUI(this, args);
        }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
          return m_binders[args.draggedItemIDs[0]].CanStartDrag;
        }

        class DragAndDropWorkingData
        {
          public int DraggedItemId
          {
            set;
            get;
          }

          public TreeViewItem ParentItem
          {
            set;
            get;
          }
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
          DragAndDrop.PrepareStartDrag();
          DragAndDrop.activeControlID = args.draggedItemIDs[0];
          DragAndDrop.visualMode = DragAndDropVisualMode.Move;
          DragAndDrop.SetGenericData(typeof(DragAndDropWorkingData).FullName,
            new DragAndDropWorkingData
            {
              DraggedItemId = args.draggedItemIDs[0],
              ParentItem = FindItem(args.draggedItemIDs[0], rootItem).parent
            });
          DragAndDrop.StartDrag("");
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
          if (DragAndDropPosition.UponItem == args.dragAndDropPosition ||
              DragAndDropPosition.OutsideItems == args.dragAndDropPosition)
            return DragAndDropVisualMode.Rejected;

          var workingData = DragAndDrop.GetGenericData(typeof(DragAndDropWorkingData).FullName) as DragAndDropWorkingData;

          if (null == workingData ||args.parentItem != workingData.ParentItem)
            return DragAndDropVisualMode.Rejected;

          if (args.performDrop)
          {
            DragAndDrop.AcceptDrag();
            Move(workingData.DraggedItemId, args.insertAtIndex);
          }

          return DragAndDropVisualMode.Move;
        }

        void Move(int itemId, int pos)
        {
          m_binders[itemId].Move(pos);
          m_binders.Move(itemId, pos);
          Reload();
        }

        public static Impl MakeMultiColumnView(Action<ColumnAppender> columnSetup,
        Action<Action<IUserItem>> populator,
        string addButtonCaption, string removeButtonCaption,
        Action<Action<IUserItem>> onAddItem, Action onDeselect,
        Action additionalGUIAfterRemoveButton)
        {
          var columnDefs = new List<MultiColumnHeaderState.Column>();
          var cellViews = new List<CellView>();

          Action<string, float, CellView> appender = (caption, width, cellView) =>
          {
            columnDefs.Add(new MultiColumnHeaderState.Column
            {
              width = width,
              headerContent = new GUIContent(caption),
              headerTextAlignment = TextAlignment.Center,
              autoResize = true,
              canSort = false
            });

            cellViews.Add(cellView);
          };

          appender("Name", 1.0f, (userItem, rect) => { });

          columnSetup(appender);

          return new Impl(populator, addButtonCaption, removeButtonCaption,
            onAddItem, onDeselect, new TreeViewState(),
            new MultiColumnHeader(new MultiColumnHeaderState(columnDefs.ToArray())),
            cellViews, additionalGUIAfterRemoveButton);
        }

        void Populate(Action<Action<IUserItem>> populator)
        {
          populator(userItem => m_binders.Add(userItem));
          Reload();
        }

        Impl(Action<Action<IUserItem>> populator,
          string addButtonCaption, string removeButtonCaption,
        Action<Action<IUserItem>> onAddItem, Action onDeselect,
          TreeViewState state, MultiColumnHeader header,
          List<CellView> cellViews, Action additionalGUIAfterRemoveButton)
          : base(state, header)
        {
          m_addRemoveButtons = new AddRemoveButtons(addButtonCaption, removeButtonCaption,
            this, onAddItem, onDeselect, additionalGUIAfterRemoveButton);

          Populate(populator);

          m_rowView = new MultiColumnRowView(cellViews);

          header.ResizeToFit();
        }

        public Impl(Action<Action<IUserItem>> populator,
          string addButtonCaption, string removeButtonCaption,
        Action<Action<IUserItem>> onAddItem, Action onDeselect,
        Action additionalGUIAfterRemoveButton)
        : base(new TreeViewState())
        {
          m_addRemoveButtons = new AddRemoveButtons(addButtonCaption, removeButtonCaption,
            this, onAddItem, onDeselect, additionalGUIAfterRemoveButton);

          Populate(populator);

          m_rowView = new SimpleRowView();
        }

        public void OnGUI()
        {
          m_addRemoveButtons.OnGUI();

          Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
          OnGUI(rect);
        }

        public IEnumerator<IUserItem> GetEnumerator()
        {
          return m_binders.GetEnumerator();
        }

        void UpdateSelection(int newId)
        {
          SetSelection(new[] { newId }, TreeViewSelectionOptions.FireSelectionChanged);
        }

        void Add(IUserItem item)
        {
          var newId = m_binders.Add(item);
          Reload();

          UpdateSelection(newId);

          if (item.CanRename)
          {
            m_addRemoveButtons.OnBeginRename();
            BeginRename(FindItem(newId, rootItem));
          }
        }

        void RemoveSelected()
        {
          var itemId = GetSelection()[0];

          m_binders[itemId].OnRemove();
          var i = m_binders.Remove(itemId);
          Reload();

          if (i < m_binders.Count)
            UpdateSelection(m_binders.Id(i));
          else if (m_binders.Count > 0)
            UpdateSelection(m_binders.Id(m_binders.Count - 1));
          else
            m_addRemoveButtons.OnDeselect();
        }
      }

      public interface IUserItem
      {
        string Name
        {
          set;
          get;
        }

        void OnSelected();
        void OnRemove();

        bool CanRename
        {
          get;
        }

        bool CanStartDrag
        {
          get;
        }

        void Move(int newPos);
      }

      public ListView(Action<Action<IUserItem>> populator,
        string addButtonCaption, string removeButtonCaption,
        Action<Action<IUserItem>> onAddItem, Action onDeselect,
        Action additionalGUIAfterRemoveButton)
      {
        m_impl = new Impl(populator, addButtonCaption, removeButtonCaption, onAddItem, onDeselect,
          additionalGUIAfterRemoveButton);
      }

      public ListView(Action<ColumnAppender> columnSetup,
        Action<Action<IUserItem>> populator,
        string addButtonCaption, string removeButtonCaption,
        Action<Action<IUserItem>> onAddItem, Action onDeselect,
        Action additionalGUIAfterRemoveButton)
      {
        m_impl = Impl.MakeMultiColumnView(columnSetup,
          populator,
          addButtonCaption, removeButtonCaption,
          onAddItem, onDeselect,
          additionalGUIAfterRemoveButton);
      }

      public void OnGUI()
      {
        m_impl.OnGUI();
      }

      public IEnumerator<IUserItem> GetEnumerator()
      {
        return m_impl.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return GetEnumerator();
      }
    }
  }
}
