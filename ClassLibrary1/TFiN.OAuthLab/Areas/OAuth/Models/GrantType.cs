using System.ComponentModel;

namespace TFiN.OAuthLab.Areas.OAuth.Models
{
    public enum GrantType
    {
        [Description("Autorisatie code")]
        AuthorizationCode = 0,
        [Description("Refresh token")]
        RefreshToken = 1,
    }
}