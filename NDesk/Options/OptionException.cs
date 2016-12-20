namespace NDesk.Options
{
    using System;

    public class OptionException : Exception
    {
        private string option;

        public OptionException()
        {
        }

        public OptionException(string message, string optionName) : base(message)
        {
            this.option = optionName;
        }

        public OptionException(string message, string optionName, Exception innerException) : base(message, innerException)
        {
            this.option = optionName;
        }

        public string OptionName
        {
            get
            {
                return this.option;
            }
        }
    }
}

