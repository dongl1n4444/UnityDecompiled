namespace Unity.PackageManager.Ivy
{
    using System;

    public class IvyRepository
    {
        public string Name;
        public string Pattern;
        public Uri Url;

        public IvyRepository()
        {
            this.Pattern = "packages/";
        }

        public IvyRepository(string name, Uri url)
        {
            this.Pattern = "packages/";
            this.Name = name;
            this.Url = url;
        }

        public Uri BuildFullUrl(string file)
        {
            if (this.Url == null)
            {
                return new Uri(new Uri("http://replace.me"), this.Pattern + file);
            }
            return new Uri(this.Url, this.Pattern + file);
        }

        public IvyRepository Clone()
        {
            return Cloner.CloneObject<IvyRepository>(this);
        }
    }
}

