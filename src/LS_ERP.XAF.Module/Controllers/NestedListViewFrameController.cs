using DevExpress.ExpressApp;

namespace LS_ERP.XAF.Module.Controllers
{
    public class NestedListViewFrameController : ViewController
    {
        private Frame masterFrame;
        public NestedListViewFrameController()
        {
            TargetViewType = ViewType.ListView;
            TargetViewNesting = Nesting.Nested;
        }
        public void AssignMasterFrame(Frame parentFrame)
        {
            masterFrame = parentFrame;
        }
    }
}
