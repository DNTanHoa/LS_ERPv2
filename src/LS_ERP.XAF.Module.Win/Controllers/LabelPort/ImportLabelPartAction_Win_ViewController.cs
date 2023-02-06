﻿using DevExpress.ExpressApp.Actions;
using LS_ERP.XAF.Module.Controllers;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class ImportLabelPartAction_Win_ViewController : ImportLabelPortAction_ViewController
    {
        public override void BrowserImportLabelPort_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserImportLabelPort_Execute(sender, e);
            var importParam = View.CurrentObject as ImportLabelPortParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            importParam.FilePath = fileDialog.FileName;
            View.Refresh();
        }
    }
}
