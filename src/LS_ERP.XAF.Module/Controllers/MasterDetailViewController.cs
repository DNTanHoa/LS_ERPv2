using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using System;

namespace LS_ERP.XAF.Module.Controllers
{
    public class MasterDetailViewController : ViewController
    {
        private void PushFrameToNestedController(Frame frame)
        {
            foreach (Controller c in frame.Controllers)
            {
                if (c is NestedListViewFrameController)
                {
                    ((NestedListViewFrameController)c).AssignMasterFrame(Frame);
                }
            }
        }
        private void lpe_FrameChanged(object sender, EventArgs e)
        {
            PushFrameToNestedController(((ListPropertyEditor)sender).Frame);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            foreach (ListPropertyEditor lpe in ((DetailView)View).GetItems<ListPropertyEditor>())
            {
                if (lpe.Frame != null)
                {
                    PushFrameToNestedController(lpe.Frame);
                }
                else
                {
                    lpe.FrameChanged += lpe_FrameChanged;
                }
            }
        }
        protected override void OnDeactivated()
        {
            foreach (ListPropertyEditor lpe in ((DetailView)View).GetItems<ListPropertyEditor>())
            {
                lpe.FrameChanged -= new EventHandler<EventArgs>(lpe_FrameChanged);
            }
            base.OnDeactivated();
        }
        public MasterDetailViewController()
        {
            TargetViewType = ViewType.DetailView;
        }
    }
}
