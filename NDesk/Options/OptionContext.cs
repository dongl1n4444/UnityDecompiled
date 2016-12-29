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
            get => 
                this.option;
            set
            {
                this.option = value;
            }
        }

        public int OptionIndex
        {
            get => 
                this.index;
            set
            {
                this.index = value;
            }
        }

        public string OptionName
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        public NDesk.Options.OptionSet OptionSet =>
            this.set;

        public OptionValueCollection OptionValues =>
            this.c;
    }
}

