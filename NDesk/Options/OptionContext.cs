namespace NDesk.Options
{
    using System;

    public class OptionContext
    {
        private OptionValueCollection c;
        private int index;
        private string name;
        private NDesk.Options.Option option;
        private NDesk.Options.OptionSet set;

        public OptionContext(NDesk.Options.OptionSet set)
        {
            this.set = set;
            this.c = new OptionValueCollection(this);
        }

        public NDesk.Options.Option Option
        {
            get
            {
                return this.option;
            }
            set
            {
                this.option = value;
            }
        }

        public int OptionIndex
        {
            get
            {
                return this.index;
            }
            set
            {
                this.index = value;
            }
        }

        public string OptionName
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public NDesk.Options.OptionSet OptionSet
        {
            get
            {
                return this.set;
            }
        }

        public OptionValueCollection OptionValues
        {
            get
            {
                return this.c;
            }
        }
    }
}

