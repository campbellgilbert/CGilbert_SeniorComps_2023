namespace ravendesk
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //navigationpage has to be the ROOT page!
            MainPage = new AppShell();

            //register new pages
            //File Select
            Routing.RegisterRoute(nameof(FileSelectPage), typeof(FileSelectPage));

            //FileSelectPage = new NavigationPage(new FileSelectPage);

        }
    }
}