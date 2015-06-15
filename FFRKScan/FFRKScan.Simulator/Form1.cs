using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFRKScan.Simulator
{
    public partial class Form1 : Form, IUserInterfacePolicy
    {
        FFRKScanAutoTamper proxy;
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            proxy = new FFRKScanAutoTamper(this);
            proxy.OnLoad();
        }

        public void Initialize(string name, IEnumerable<IFeature> featureTabs)
        {
            TabPage mTabPage = FiddlerUIPolicy.CreateTabPage(name, featureTabs);

            this.tabControl1.Controls.Add(mTabPage);
        }
    }
}
