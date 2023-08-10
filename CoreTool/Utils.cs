using System;
using System.Text;
using System.Threading.Tasks;
using MicrosoftAuth;
using MicrosoftAuth.Models.Token;
using MSAuth.Popup;

namespace CoreTool
{
    internal class Utils
    {
        public static async Task<string> GetMicrosoftToken(string scope = "service::dcat.update.microsoft.com::MBI_SSL")
        {
            string token = OAuthPopup.GetAuthToken();
            MicrosoftAccount account = MicrosoftAccount.FromOAuthResponse(token);

            BaseToken requestedToken = await account.RequestToken("{28520974-CE92-4F36-A219-3F255AF7E61E}", new SecureScope($"scope={scope}", "TOKEN_BROKER"));

            Console.WriteLine($"Microsoft: Received token for scope {scope}.");

            return Convert.ToBase64String(Encoding.Unicode.GetBytes(requestedToken.Token));
        }
    }
}