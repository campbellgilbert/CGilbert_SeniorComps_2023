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
            Routing.RegisterRoute(nameof(TextEditor), typeof(TextEditor));
            Routing.RegisterRoute(nameof(CopilotPage), typeof(CopilotPage));
            Routing.RegisterRoute(nameof(FollowupPopup), typeof(FollowupPopup));
        }
    }
}