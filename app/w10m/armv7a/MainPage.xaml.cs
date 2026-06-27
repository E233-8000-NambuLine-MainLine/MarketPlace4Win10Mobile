private void StartStore_Click(object sender, RoutedEventArgs e)
{
    // トークンがあればLocalSettingsに保存（なければ空のまま）
    var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
    settings.Values["GitHubToken"] = TokenInput.Text;

    // TODO: ここでメインのストア画面（Pivot画面など）に遷移させる
    // Frame.Navigate(typeof(StoreMainPage));
}
