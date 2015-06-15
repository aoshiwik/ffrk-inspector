
using Fiddler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFRKScan
{
    public interface IUserInterfacePolicy
    {
        void Initialize(string name, IEnumerable<IFeature> featureTabs);
    }

    public class StandAloneUIPolicy : IUserInterfacePolicy
    {
        Form form;

        public void Initialize(string name, IEnumerable<IFeature> featureTabs)
        {
            TabPage mTabPage = FiddlerUIPolicy.CreateTabPage(name, featureTabs);

            var tabControls = new TabControl();
            tabControls.Controls.Add(mTabPage);
            tabControls.Dock = DockStyle.Fill;

            form = new Form() { Width = 1024, Height = 800 };
            form.Controls.Add(tabControls);
            form.Show();
        }

        public void Run()
        {
            Application.EnableVisualStyles();
            Application.Run(form);
        }
    }


    public class NoUIPolicy : IUserInterfacePolicy
    {
        public void Initialize(string name, IEnumerable<IFeature> featureTabs)
        {
        }
    }

    public class FiddlerUIPolicy : IUserInterfacePolicy
    {
        public void Initialize(string name, IEnumerable<IFeature> featureTabs)
        {
            TabPage mTabPage = CreateTabPage(name, featureTabs);

            FiddlerApplication.UI.tabsViews.TabPages.Add(mTabPage);
        }

        public static TabPage CreateTabPage(string name, IEnumerable<IFeature> featureTabs)
        {
            TabPage mTabPage;
            mTabPage = new TabPage(name);

            var innerTabs = new TabControl();
            mTabPage.Controls.Add(innerTabs);

            foreach (var createFeatureTab in featureTabs)
            {
                var featureControl = createFeatureTab.CreateControl();

                if (featureControl != null)
                {
                    var featureTab = new TabPage(createFeatureTab.Name);
                    var content = featureControl;
                    content.Dock = DockStyle.Fill;
                    featureTab.Controls.Add(content);

                    featureTab.Dock = DockStyle.Fill;
                    innerTabs.Controls.Add(featureTab);
                }
            }

            innerTabs.Dock = DockStyle.Fill;
            mTabPage.Controls.Add(innerTabs);
            return mTabPage;
        }
    }
}
