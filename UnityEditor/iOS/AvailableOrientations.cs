namespace UnityEditor.iOS
{
    using System;

    internal class AvailableOrientations
    {
        public bool landscapeLeft;
        public bool landscapeRight;
        public bool portrait;
        public bool portraitUpsideDown;

        public AvailableOrientations()
        {
            this.portrait = this.portraitUpsideDown = this.landscapeLeft = this.landscapeRight = false;
        }
    }
}

