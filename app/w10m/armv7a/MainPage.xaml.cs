using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http; // HTTP通信に必須！
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MarketPlace4Win10Mobile
{
    // ==========================================
    // 1. メイン画面のロジック（コントローラー）
    // ==========================================
    public sealed partial class MainPage : Page
    {
        private AppInfo myApp;

        public MainPage()
        {
            this.InitializeComponent();

            // インスタンス化してDataContextに接着（HTML/JSのバインドと同じ）
            myApp = new AppInfo();
            this.DataContext = myApp;

            // アプリ起動と同時に、裏で自動的にGitHub APIを叩きに行く（非同期）
            FetchGitHubData();
        }

        // C#名物「async/await」による超強力な非同期HTTPリクエスト
        private async void FetchGitHubData()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // GitHub APIを叩く時はUser-Agentヘッダーが絶対に必要（ルール）
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("MarketPlaceApp");

                    // テスト用として、適当なGitHubのAPI（例としてリポジトリ情報）をGETしてみる
                    // 本番はここにあなたのリポジトリのURLが入ります
                    string url = "https://github.com";
                    
                    // awaitを置くだけで、通信が終わるまで「画面をフリーズさせずに」裏で待ってくれる！
                    string jsonResult = await client.GetStringAsync(url);

                    // TODO: 本来はここでJSONをパース（解析）して代入します
                    // 今回はGETが成功した証拠として、AppNameにデータをぶち込む！
                    myApp.AppName = "GitHubからGET成功！";
                    myApp.App.Desc = "データの中身: " + jsonResult.Substring(0, 50) + "...";
                }
            }
            catch (Exception)
            {
                myApp.AppName = "通信エラー(Process Killed)";
            }
        }

        // ボタンクリックイベントはしっかりクラスの中に配置！
        private void StartStore_Click(object sender, RoutedEventArgs e)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values["GitHubToken"] = TokenInput.Text;

            // Frame.Navigate(typeof(StoreMainPage));
        }
    }

    // ==========================================
    // 2. データモデル（画面と連動する変数たち）
    // ==========================================
    public class Developer
    {
        public string Name { get; set; } = "GT-R50_MA";
    }

    public class AppDetail : INotifyPropertyChanged
    {
        private string _desc = "大江戸コントローラーを聴きながら爆速で動く最強のメディアアプリです。";
        public string Desc
        {
            get { return _desc; }
            set { if (_desc != value) { _desc = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class AppInfo : INotifyPropertyChanged
    {
        private string _appName = "読み込み中...";
        public string AppName
        {
            get { return _appName; }
            set { if (_appName != value) { _appName = value; OnPropertyChanged(); } }
        }

        public Developer Develop { get; set; } = new Developer();
        public AppDetail App { get; set; } = new AppDetail();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
