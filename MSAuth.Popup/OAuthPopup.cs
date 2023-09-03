﻿using CefSharp;
using CefSharp.WinForms;

namespace MSAuth.Popup
{
    public partial class OAuthPopup : Form
    {
        private static readonly string authPopupUrl = "https://login.live.com/ppsecure/InlineConnect.srf?id=80604&platform=android2.1.0510.1018&client_id=android-app://com.mojang.minecraftearth.H62DKCBHJP6WXXIV7RBFOGOL4NAK4E6Y";

        private static string OAuthResponseToken { get; set; } = "";

        public OAuthPopup()
        {
            CefSettings settings = new CefSettings();
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            settings.LogSeverity = LogSeverity.Disable;

            Cef.Initialize(settings);
            InitializeWebview();

            usedBrowser!.LoadUrl(authPopupUrl);
        }

        #region CefSharp Webview Code

        private ChromiumWebBrowser usedBrowser;
        private void InitializeWebview()
        {
            usedBrowser = new ChromiumWebBrowser();
            SuspendLayout();

            usedBrowser.ActivateBrowserOnCreation = false;
            usedBrowser.Dock = DockStyle.Fill;
            usedBrowser.Location = new Point(0, 0);
            usedBrowser.Name = "Sign in Microsoft Account";
            usedBrowser.Size = new Size(420, 550);
            usedBrowser.TabIndex = 0;
            usedBrowser.Text = "msAuthPopupWebview";
            usedBrowser.LoadingStateChanged += BrowserOnLoadingStateChanged;

            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(420, 550);
            Controls.Add(usedBrowser);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Sign in Microsoft Account";
            this.Icon = MicrosoftAuth.Popup.Properties.Resources.MS;
            ShowIcon = true;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sign in Microsoft Account";

            ResumeLayout(false);
        }

        public new void Click(int x, int y)
        {
            usedBrowser.GetBrowser().GetHost().SendMouseClickEvent(x, y, MouseButtonType.Left, false, 1, CefEventFlags.None);
            Thread.Sleep(15);
            usedBrowser.GetBrowser().GetHost().SendMouseClickEvent(x, y, MouseButtonType.Left, true, 1, CefEventFlags.None);
        }

        private Task AutoLogin()
        {
            while (true)
            {
                var task_1 = Task.Run(async delegate
                {
                    try
                    {
                        //auto login in
                        await Task.Delay(5000);
                        //fill email
                        usedBrowser.ExecuteScriptAsync("document.getElementById('i0116').value = '';");
                        await Task.Delay(500);
                        //click next button
                        usedBrowser.ExecuteScriptAsync("document.getElementById('i0116').click();");
                        usedBrowser.ExecuteScriptAsync("document.getElementById('idSIButton9').click();");
                        Click(0, 0);
                        await Task.Delay(5000);
                        //fill password
                        usedBrowser.ExecuteScriptAsync("document.getElementById('i0118').value = '';");
                        //await Task.Delay(500);
                        usedBrowser.ExecuteScriptAsync("document.getElementById('i0118').click();");
                        usedBrowser.ExecuteScriptAsync("document.getElementById('idSIButton9').click();");
                        Click(0, 0);
                        await Task.Delay(5000);
                    }
                    catch
                    {
                        // ignored
                    }
                });
                break;
            }

            return Task.CompletedTask;
        }

        private void BrowserOnLoadingStateChanged(object? sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading) return;

            string javascriptCode = @"
                document.getElementById('lightboxBackgroundContainer').style.display = 'none';
                document.getElementsByClassName('background-logo-holder')[0].style.display = 'none';
                document.getElementById('footer').style.display = 'none';
                document.getElementById('lightbox').style.border = 'none';
                document.getElementById('lightbox').style.boxShadow = 'none';
                document.getElementById('lightbox').style.marginBottom = '0';
            ";
            usedBrowser.ExecuteScriptAsync(javascriptCode);

            _ = AutoLogin();

            var cookieVisitor = new CookieVisitor(cookies =>
            {
                var hasFinishedAuth = cookies.First(cookie => cookie.name == "Page").value.Contains("finalNext");

                if (hasFinishedAuth)
                {
                    // Mismatched names because on auth finish PPInlineAuth is set,
                    // but Property is populated
                    var propertyCookie = cookies.First(pred => pred.name == "Property");
                    OAuthResponseToken = propertyCookie.value;
                    DialogResult = DialogResult.OK;
                    BeginInvoke(Close);
                }
            });

            Cef.GetGlobalCookieManager().VisitAllCookies(cookieVisitor);
        }

        public static string GetAuthToken()
        {
            var popup = new OAuthPopup();
            popup.ShowDialog();

            return OAuthResponseToken;
        }

        #endregion

        private void OAuthPopup_Load(object sender, EventArgs e)
        {

        }
    }
}
