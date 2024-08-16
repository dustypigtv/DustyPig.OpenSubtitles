using System;
using System.IO;
using System.Text.Json;

namespace DustyPig.OpenSubtitles.Utils;


public class SessionInfo
{
    static JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    public string Token { get; set; }
    
    public DateTime ExpiresUTC { get; set; }

    public string BaseUrl { get; set; }

    public string ToJson() => JsonSerializer.Serialize(this, _jsonSerializerOptions);

    public void Save(FileInfo file)
    {
        file.Directory.Create();
        File.WriteAllText(file.FullName, ToJson());
    }

    public void Save(string filename) => Save(new FileInfo(filename));


    public static SessionInfo FromJson(string json) => JsonSerializer.Deserialize<SessionInfo>(json);

    public static SessionInfo FromFile(FileInfo file) => FromJson(File.ReadAllText(file.FullName));

    public static SessionInfo FromFile(string filename) => FromJson(File.ReadAllText(filename));
}
