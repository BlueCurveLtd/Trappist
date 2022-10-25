namespace Trappist.Wpf.Bedrock.Controls.Pages.Helpers
{
    public static class MasterDetailInfo
    {
        private static readonly MasterDetail masterDetail;

        static MasterDetailInfo() => masterDetail = (MasterDetail)BedrockApplication.Current.MainWindow;

        public static string? Title
        {
            get => masterDetail.TopActionBarTitle;
            set => masterDetail.TopActionBarTitle = value ?? string.Empty;
        }

        public static void ClearTitle() => Title = null;

        public static void FullScreen() => masterDetail.FullScreen();

        public static void Minimize() => masterDetail.Minimize();
    }
}
