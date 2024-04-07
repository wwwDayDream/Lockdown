using System;
using System.IO;
using System.Reflection;

namespace Lockdown;

#if DEBUG
internal static class FileLog {
    private const string LogFileName = "NetworkInstantiates.log";
    public static string LogFilePath =>
        Path.Combine(
            Directory.GetParent(Assembly.GetExecutingAssembly().Location.TrimEnd(Path.DirectorySeparatorChar))?.FullName ?? Environment.CurrentDirectory,
            LogFileName);

    public static void Log(object o)
    {
        File.AppendAllText(LogFilePath, o + Environment.NewLine);
    }
}
#endif