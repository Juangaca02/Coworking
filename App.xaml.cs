namespace TFG
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Registrar Syncfusion
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF5cXmBCe0x0R3xbf1x1ZFZMYVpbRnNPIiBoS35Rc0VnW31fdXdWRWZbWEF0VEBU");
            MainPage = new AppShell();
            //MainPage = new NavigationPage(new MainPage());
        }
    }
}