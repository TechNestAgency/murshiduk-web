using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using MurshadikCP.Models.DB;
using System.IO;
using System.Text;
using System.Web;

[assembly: OwinStartupAttribute(typeof(MurshadikCP.Startup))]
namespace MurshadikCP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ///before this
            ConfigureAuth(app);
            //// now after adding api then we are checking this technique
            ///
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "http://murshadik.com",
                        ValidAudience = "http://murshadik.com",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_secret_key_1231321"))
                    }
                });

            var pathToKey = Path.Combine(HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["firebaseJson"]));
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(pathToKey)
            });

        }
    }
}
