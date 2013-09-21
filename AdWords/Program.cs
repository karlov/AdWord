using Google.Api.Ads.AdWords.Lib;
using Google.Api.Ads.AdWords.v201306;
using Google.Api.Ads.Common.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Google_Adwords
{
    class Program
    {
        static void Main(string[] args)
        {
            //Авторизация
            AdWordsUser user = new AdWordsUser();
            AdWordsAppConfig config = (user.Config as AdWordsAppConfig);
            if (config.AuthorizationMethod == AdWordsAuthorizationMethod.OAuth2)
            {
                if (config.OAuth2Mode == OAuth2Flow.APPLICATION &&
                string.IsNullOrEmpty(config.OAuth2RefreshToken))
                {
                    DoAuth2Authorization(user);
                }
            }
            else
            {
                throw new Exception("Authorization mode is not OAuth.");
            }

            Console.Write("Enter the customer id: ");
            string customerId = Console.ReadLine();
            config.ClientCustomerId = customerId;
            //Конец
        }
        public void GetAuthToken()
        {
            string URL = "https://www.google.com/accounts/ClientLogin";
            string email = "apis@location3.com";
            string password = "L3MP@ssw0rd";
            string source = "adwords";

            string httpBody =
                string.Format(
                    "accountType=HOSTED_OR_GOOGLE&Email={0}&Passwd={1}&service=adwords&source={2}",
                    email, password, source);
            var request = WebRequest.Create(URL) as HttpWebRequest;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(httpBody);
            }

            using (HttpWebResponse httpWebResponse = request.GetResponse() as HttpWebResponse)
            {
                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream stream = httpWebResponse.GetResponseStream())
                    {
                        StreamReader readStream = new StreamReader(stream, Encoding.UTF8);
                        Console.Out.WriteLine(readStream.ReadToEnd());
                    }
                }
            }
        }

        //Ф-я авторизации
        private static void DoAuth2Authorization(AdWordsUser user)
        {
            // Since we are using a console application, set the callback url to null.
            user.Config.OAuth2RedirectUri = null;
            AdsOAuthProviderForApplications oAuth2Provider =
            (user.OAuthProvider as AdsOAuthProviderForApplications);
            // Get the authorization url.
            string authorizationUrl = oAuth2Provider.GetAuthorizationUrl();
            Console.WriteLine("Open a fresh web browser and navigate to \n\n{0}\n\n. You will be " +
            "prompted to login and then authorize this application to make calls to the " +
            "AdWords API. Once approved, you will be presented with an authorization code.",
            authorizationUrl);

            // Accept the OAuth2 authorization code from the user.
            Console.Write("Enter the authorization code :");
            string authorizationCode = Console.ReadLine();

            // Fetch the access and refresh tokens.
            oAuth2Provider.FetchAccessAndRefreshTokens(authorizationCode);
        }
    }
}
