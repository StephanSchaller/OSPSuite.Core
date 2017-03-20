﻿using System;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.UI.Views
{
   public partial class ExceptionView : XtraForm, IExceptionView
   {
      private string _assemblyInfo;
      private string _issueTrackerUrl;
      private const string _couldNotCopyToClipboard = "Unable to copy the information to the clipboard.";
      public object MainView { private get; set; }

      public ExceptionView()
      {
         InitializeComponent();
         initializeResources();
         btnCopyToClipboard.Click += (o, e) => copyToClipboard();
      }

      private void initializeResources()
      {
         layoutItemException.TextVisible = false;
         layoutItemFullException.TextVisible = false;
         tbException.Properties.ReadOnly = true;
         tbFullException.Properties.ReadOnly = true;
         lblDescription.AutoSizeMode = LabelAutoSizeMode.Vertical;
         lblDescription.AllowHtmlString = true;
         MinimizeBox = false;
         MaximizeBox = false;
         btnCopyToClipboard.Text = Captions.CopyToClipboard;
         btnClose.Text = Captions.CloseButton;
         layoutItemOk.AdjustButtonSize();
         layoutItemCopyToClipbord.AdjustButtonSize();
         layoutGroupException.Text = Captions.Exception;
         layoutGroupStackTraceException.Text = Captions.StackTrace;
         issueTrackerLink.OpenLink += (o, e) => goToIssueTracker(e);
         ActiveControl = btnClose;
      }

      private void goToIssueTracker(OpenLinkEventArgs e)
      {
         e.EditValue = _issueTrackerUrl;
      }

      public string Description
      {
         set
         {
            layoutItemDescription.Visibility = LayoutVisibilityConvertor.FromBoolean(!string.IsNullOrEmpty(value));
            lblDescription.Text = value;
         }
      }

      public void Initialize(string caption, ApplicationIcon icon, string productInfo, string issueTrackerUrl, string productName)
      {
         Text = caption;
         Icon = icon;
         _assemblyInfo = productInfo;
         _issueTrackerUrl = issueTrackerUrl;
         Description = Captions.ExceptionViewDescription(issueTrackerUrl);
         issueTrackerLink.Text = Captions.IssueTrackerLinkFor(productName);
      }

      private void copyToClipboard()
      {
         try
         {
            invokeOnSTAThread(copyToClipboardOnUIThread);
         }
         catch (Exception)
         {
            showException(_couldNotCopyToClipboard);
         }
      }

      private void invokeOnSTAThread(ThreadStart method)
      {
         if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
         {
            Thread thread = new Thread(method);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
         }
         else
         {
            method();
         }
      }

      private void copyToClipboardOnUIThread()
      {
         Clipboard.SetText(fullContent());
      }

      private string fullContent()
      {
         return $"Application:\n{_assemblyInfo}\n\n{ExceptionMessage}\n\nStack trace:\n{FullStackTrace}";
      }

      private void showException(string message)
      {
         XtraMessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      public string ExceptionMessage
      {
         set { tbException.Text = value; }
         private get { return tbException.Text; }
      }

      public string FullStackTrace
      {
         set { tbFullException.Text = value; }
         private get { return tbFullException.Text; }
      }

      public void Display()
      {
         if (MainView == null)
         {
            ShowDialog();
            return;
         }

         showDialogWithOwner();
      }

      private void showDialogWithOwner()
      {
         try
         {
            ShowDialog((Form) MainView);
         }
         catch (Exception)
         {
            ShowDialog();
         }
      }

      public void Display(Exception exception)
      {
         ExceptionMessage = exception.FullMessage();
         FullStackTrace = exception.FullStackTrace();
         Display();
      }

   
   }
}