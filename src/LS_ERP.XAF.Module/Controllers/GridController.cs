using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;
using System.Drawing;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class GridController : ViewController<ListView>
    {
        public GridController()
        {
            InitializeComponent();
        }
        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            GridListEditor listEditor = View.Editor as GridListEditor;
            if (listEditor != null)
            {
                GridView gridView = listEditor.GridView;

                // Light gray background for odd rows  
                gridView.OptionsView.EnableAppearanceOddRow = true;
                gridView.Appearance.OddRow.BackColor = Color.FromArgb(239, 239, 239);

                //// Different light gray background for even rows  
                //gridView.OptionsView.EnableAppearanceEvenRow = true;
                //gridView.Appearance.EvenRow.BackColor = Color.FromArgb(246, 246, 246);

                // Light blue background for focused row  
                gridView.Appearance.FocusedRow.BackColor = Color.FromArgb(186, 217, 240);
                gridView.Appearance.FocusedCell.BackColor = Color.FromArgb(209, 230, 245);

            }
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
