using IWshRuntimeLibrary;
using System;
using System.IO;
namespace FakeDOORSInstaller
{
    class Installer
    {

        static void Main(string[] args)
        {
            Directory.CreateDirectory(@"C:\Program Files\FakeDOORS");
            System.IO.File.Copy("FakeDOORS.exe", @"C:\Program Files\FakeDOORS\FakeDOORS.exe", true);

            ShortcutCreator.CreateShortcut();
        }
    }
}
