using System.ComponentModel;

namespace TFiN.OAuthLab.Areas.OAuth.Models
{
    public enum ResponseType
    {
        [Description("Autorisatie code")]
        Code = 0,
        [Description("Access token")]
        Token = 1,
    }
}