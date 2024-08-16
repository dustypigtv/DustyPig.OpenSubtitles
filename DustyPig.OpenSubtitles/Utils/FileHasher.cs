using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DustyPig.OpenSubtitles.Utils;

public static class FileHasher
{
    public static string ComputeHash(string filename) => ComputeHash(new FileInfo(filename));

    public static string ComputeHash(FileInfo file)
    {
        long hash = file.Length;
        using var stream = file.OpenRead();

        long i = 0;
        byte[] buffer = new byte[sizeof(long)];
        while (i < 65536 / sizeof(long) && (stream.Read(buffer, 0, sizeof(long)) > 0))
        {
            i++;
            hash += BitConverter.ToInt64(buffer, 0);
        }

        stream.Position = Math.Max(0, file.Length - 65536);
        i = 0;
        while (i < 65536 / sizeof(long) && (stream.Read(buffer, 0, sizeof(long)) > 0))
        {
            i++;
            hash += BitConverter.ToInt64(buffer, 0);
        }

        
        byte[] hashBytes = BitConverter.GetBytes(hash);
        Array.Reverse(hashBytes);
        
        return string.Join(string.Empty, hashBytes.Select(_ => _.ToString("x2", CultureInfo.InvariantCulture)));
    }


    /*
    // Original code from: https://trac.opensubtitles.org/projects/opensubtitles/wiki/HashSourceCodes


    private static byte[] ComputeMovieHash(string filename)
    {
        byte[] result;
        using (Stream input = File.OpenRead(filename))
        {
            result = ComputeMovieHash(input);
        }
        return result;
    }

    private static byte[] ComputeMovieHash(Stream input)
    {
        long lhash, streamsize;
        streamsize = input.Length;
        lhash = streamsize;

        long i = 0;
        byte[] buffer = new byte[sizeof(long)];
        while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0))
        {
            i++;
            lhash += BitConverter.ToInt64(buffer, 0);
        }

        input.Position = Math.Max(0, streamsize - 65536);
        i = 0;
        while (i < 65536 / sizeof(long) && (input.Read(buffer, 0, sizeof(long)) > 0))
        {
            i++;
            lhash += BitConverter.ToInt64(buffer, 0);
        }
        input.Close();
        byte[] result = BitConverter.GetBytes(lhash);
        Array.Reverse(result);
        return result;
    }

    private static string ToHexadecimal(byte[] bytes)
    {
        StringBuilder hexBuilder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            hexBuilder.Append(bytes[i].ToString("x2"));
        }
        return hexBuilder.ToString();
    }
    */
}
