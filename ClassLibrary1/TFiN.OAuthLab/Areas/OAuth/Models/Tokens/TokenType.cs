using System.ComponentModel;

namespace TFiN.OAuthLab.Areas.OAuth.Models
{
    public enum TokenType
    {
        [Description("Bearer token")]
        Bearer = 1,
        [Description("Access token")]
        Refresh = 1,
    }
}