using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

[Serializable]
public class CrmSetting
{
    public string Key { get; set; } = "626F9A17E540DF3DB4FA99166A0E03BCC9D944130CE29CCF50C68AE21D1ADCC4";
    public string Iv { get; set; } = "7B8E47125CA0160D7FDEDF82FA08C286";
    public string User { get; set; }
    public string Role { get; set; }
    public string ReqKey { get; set; } = "7495bfa034924527bfd516ca0e881f4b";
    public string PartnerId { get; set; } = "626F9A17E540DF3DB4FA99166A0E03BCC9D944130CE29CCF50C68AE21D1ADCC4";

    [NotMapped] public byte[] KeyToBytes => Encoding.UTF8.GetBytes(Key);
    [NotMapped] public byte[] IvToBytes => Encoding.UTF8.GetBytes(Iv);
    [NotMapped] public string UserFromBase64 => Encoding.UTF8.GetString(Convert.FromBase64String(User));
    [NotMapped] public string RoleFromBase64 => Encoding.UTF8.GetString(Convert.FromBase64String(Role));
}