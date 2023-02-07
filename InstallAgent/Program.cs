using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace InstallAgent
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Init();
        }

        static void Init()
        {
            var keyName = "dlagent";
            var app = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DownloadAgent.exe");
            // 判断是否存在
            var hcr = Registry.ClassesRoot;
            //var hcr = Registry.CurrentUser.OpenSubKey("Software\\Classes");
            using (var key = Open(hcr, keyName))
            {
                key.SetValue(null, "URL:dlagent");
                key.SetValue("URL Protocol", app);
                using (var dicon = Open(key, "DefaultIcon"))
                {
                    dicon.SetValue(null, $"{app},1");
                }

                using (var shell = Open(key, "shell"))
                {
                    using (var open = Open(shell, "open"))
                    {
                        using (var cmd = Open(open, "command"))
                        {
                            cmd.SetValue(null, $"\"{app}\" \"%1\"");
                        }
                    }
                }
            }

            hcr.Close();

        }

        static RegistryKey Open(RegistryKey root, string name)
        {
            if (root.GetSubKeyNames().Any(x => x == name))
            {
                return root.OpenSubKey(name);
            }
            return root.CreateSubKey(name);
        }

        static void UpdateAccess(RegistryKey key)
        {
            string user = Environment.UserDomainName + "\\" + Environment.UserName;
            var rule = new RegistryAccessRule(user, RegistryRights.ReadKey | RegistryRights.WriteKey
                                                          | RegistryRights.Delete
                                                          | RegistryRights.ChangePermissions, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow);
            var rs = new RegistrySecurity();
            rs.AddAccessRule(new RegistryAccessRule(user, RegistryRights.ReadKey | RegistryRights.WriteKey
                                                          | RegistryRights.Delete
                                                          | RegistryRights.ChangePermissions, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow));

            var r = key.GetAccessControl();
            r.AddAccessRule(rule);
        }

        static void Set(RegistryKey key, string name, string value)
        {
            var v = key.GetValue(name);
            if (v == null)
            {
                key.SetValue(name, value);
            }
            else
            {
                key.DeleteValue(name);
                key.SetValue(name, value);
            }
        }
    }
}
