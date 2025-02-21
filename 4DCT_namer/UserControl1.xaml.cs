using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

using ViewModels;

[assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class Script : UserControl
    {
        private ViewModel InstViewModel = new ViewModel();

        public Script()
        {
            InitializeComponent();
        }

        public void Execute(ScriptContext context, System.Windows.Window window)
        {
////            window.Height = 200;
//            window.Height = 250;
//            window.Width = 600;
//            window.Content = this;
//            window.Background = Brushes.WhiteSmoke;
//            window.SizeChanged += (sender, args) =>
//            {
//                //                this.Height = window.ActualHeight;
//                //                this.Width = window.ActualWidth;
//                this.Height = window.ActualHeight * 0.95;
//                this.Width = window.ActualWidth * 0.98;
//            };

            window.Content = this;
            window.SizeChanged += (sender, args) =>
            {
                this.Height = window.ActualHeight * 0.92;
                this.Width = window.ActualWidth * 0.95;
            };
            window.Height = 250;
            window.Width = 600;

            InstViewModel = this.DataContext as ViewModel;
            InstViewModel.SetScriptContext(context);

        }
    }
}
