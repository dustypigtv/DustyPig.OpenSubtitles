﻿namespace DustyPig.OpenSubtitles.Models;

public class Credentials
{
    public Credentials() { }

    public Credentials(string username, string password) 
    { 
        Username = username;
        Password = password;
    }

    public string Username { get; set; }

    public string Password { get; set; }
}
