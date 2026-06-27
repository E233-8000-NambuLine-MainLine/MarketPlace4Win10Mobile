using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http;
using Windows.Data.Json; // 【追加】JSONパースに必要な魔法のインクルード！
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

            // インスタンス化してDataContextに接着
            myApp = new AppInfo();
            this.DataContext = myApp;

            // アプリ起動時に非同期でGitHub APIを叩きに行く
            FetchGitHubData();
        }

        // GitHub APIからデータをGETしてJSONをパースする関数
        private async void FetchGitHubData()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // GitHub APIの動作に必須のヘッダーを設定
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("MarketPlaceApp");

                    // 【修正】あなたの本物のリポジトリ情報を返すGitHub APIのURL
                    string url = "https://github.com";
                    
                    // 1. HTTPリクエストで生のJSON文字列をGET
                    string jsonResult = await client.GetStringAsync(url);

                    // 2. 【JSON魔法】文字列をオブジェクトにパース（解析）！
                    JsonObject jsonObject = JsonObject.Parse(jsonResult);

                    // 3. JSONのキーを指定して、必要なデータを直接引っこ抜く！
                    string repoName = jsonObject.GetNamedString("name");
                    string repoDesc = jsonObject.GetNamedString("description");
                    string ownerName = jsonObject.GetNamedObject("owner").GetNamedString("login");

                    // 4. 【変数の置き換えを実行！】
                    // 代入した瞬間に、XAML側の画面の文字も全自動でパッ書き換わります！
                    myApp.AppName = repoName;       // 「MarketPlace4Win10Mobile」
                    myApp.Develop.Name = ownerName; // 「E233-8000-NambuLine」
                    myApp.App.Desc = repoDesc;       // 「GitHubからレポジトリを登録し…」
                }
            }
            catch (Exception)
            {
                myApp.AppName = "通信エラー(Process Killed)";
            }
        }

        // ボタンクリックイベント
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
    public class Developer : INotifyPropertyChanged
    {
        private string _name = "GT-R50_MA";
        
        public string Name
        {
            get { return _name; }
            set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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

        // 階層化データオブジェクトの定義
        public Developer Develop { get; set; } = new Developer();
        public AppDetail App { get; set; } = new AppDetail();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
