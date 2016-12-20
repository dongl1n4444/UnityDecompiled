using System;
using System.Collections.Generic;

internal class OverwriteFilesInfo
{
    private Dictionary<string, bool> doOverwrite;
    private Dictionary<string, string> hashes;
    private bool keepAll;
    private bool overwriteAll = false;
    private List<string> userModified;

    public OverwriteFilesInfo()
    {
        this.KeepAll = false;
    }

    public Dictionary<string, bool> DoOverwrite
    {
        get
        {
            return ((this.doOverwrite == null) ? (this.doOverwrite = new Dictionary<string, bool>()) : this.doOverwrite);
        }
    }

    public Dictionary<string, string> Hashes
    {
        get
        {
            return ((this.hashes == null) ? (this.hashes = new Dictionary<string, string>()) : this.hashes);
        }
    }

    public bool KeepAll
    {
        get
        {
            return this.keepAll;
        }
        set
        {
            this.keepAll = value;
            if (this.keepAll)
            {
                this.DoOverwrite.Clear();
                this.UserModified.Clear();
            }
        }
    }

    public bool OverwriteAll
    {
        get
        {
            return this.overwriteAll;
        }
        set
        {
            this.overwriteAll = value;
            if (this.overwriteAll)
            {
                this.DoOverwrite.Clear();
                this.UserModified.Clear();
            }
        }
    }

    public List<string> UserModified
    {
        get
        {
            return ((this.userModified == null) ? (this.userModified = new List<string>()) : this.userModified);
        }
    }
}

