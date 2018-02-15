﻿namespace OSPSuite.UI.Views.Journal
{
   partial class RelatedItemsView
   {
      /// <summary> 
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary> 
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         _gridViewBinder.Dispose();
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.buttonAddRelatedItemFromFile = new DevExpress.XtraEditors.SimpleButton();
         this.gridControl = new Controls.UxGridControl();
         this.gridView = new Controls.UxGridView();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemGrid = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemAddRelatedItem = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGrid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAddRelatedItem)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.buttonAddRelatedItemFromFile);
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(566, 323);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "uxLayoutControl1";
         // 
         // buttonAddRelatedItemFromFile
         // 
         this.buttonAddRelatedItemFromFile.Location = new System.Drawing.Point(285, 2);
         this.buttonAddRelatedItemFromFile.Name = "buttonAddRelatedItemFromFile";
         this.buttonAddRelatedItemFromFile.Size = new System.Drawing.Size(279, 22);
         this.buttonAddRelatedItemFromFile.StyleController = this.layoutControl;
         this.buttonAddRelatedItemFromFile.TabIndex = 5;
         this.buttonAddRelatedItemFromFile.Text = "buttonAddRelatedItemFromFile";
         // 
         // gridControl
         // 
         this.gridControl.Location = new System.Drawing.Point(2, 28);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(562, 293);
         this.gridControl.TabIndex = 4;
         this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
         // 
         // gridView
         // 
         this.gridView.GridControl = this.gridControl;
         this.gridView.Name = "gridView";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemGrid,
            this.layoutItemAddRelatedItem,
            this.emptySpaceItem});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(566, 323);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemGrid
         // 
         this.layoutItemGrid.Control = this.gridControl;
         this.layoutItemGrid.Location = new System.Drawing.Point(0, 26);
         this.layoutItemGrid.Name = "layoutItemGrid";
         this.layoutItemGrid.Size = new System.Drawing.Size(566, 297);
         this.layoutItemGrid.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemGrid.TextVisible = false;
         // 
         // layoutItemAddRelatedItem
         // 
         this.layoutItemAddRelatedItem.Control = this.buttonAddRelatedItemFromFile;
         this.layoutItemAddRelatedItem.Location = new System.Drawing.Point(283, 0);
         this.layoutItemAddRelatedItem.Name = "layoutItemAddRelatedItem";
         this.layoutItemAddRelatedItem.Size = new System.Drawing.Size(283, 26);
         this.layoutItemAddRelatedItem.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemAddRelatedItem.TextVisible = false;
         // 
         // emptySpaceItem
         // 
         this.emptySpaceItem.AllowHotTrack = false;
         this.emptySpaceItem.Location = new System.Drawing.Point(0, 0);
         this.emptySpaceItem.Name = "emptySpaceItem";
         this.emptySpaceItem.Size = new System.Drawing.Size(283, 26);
         this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
         // 
         // RelatedItemsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "RelatedItemsView";
         this.Size = new System.Drawing.Size(566, 323);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGrid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAddRelatedItem)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemGrid;
      private DevExpress.XtraEditors.SimpleButton buttonAddRelatedItemFromFile;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemAddRelatedItem;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem;
      private Controls.UxGridControl gridControl;
      private Controls.UxGridView gridView;
   }
}
