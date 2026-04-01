#pragma warning disable RCS1110
//using System.Globalization;
using System.IO;

namespace Mod.CustomCampaigns.Extensions
{
    public static class StringExtensions
    {
        public static string UniformPathSeparators(this string source)
        {
            return source.Replace(Path.DirectorySeparatorChar, '/')
                         .Replace(Path.AltDirectorySeparatorChar, '/')
                         .Replace('\\', '/');
        }

        public static string UniformPathSeparatorsTrimmed(this string source)
        {
            return source.UniformPathSeparators().TrimEnd('/');
        }
    }
}
