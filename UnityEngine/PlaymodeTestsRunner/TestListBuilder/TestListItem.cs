namespace UnityEngine.PlaymodeTestsRunner.TestListBuilder
{
    using System;
    using System.Collections.Generic;

    internal class TestListItem : TestListElement
    {
        public TestListItem(string id, string name, string fullName) : base(id, name, fullName)
        {
        }

        public override IEnumerable<TestListElement> GetFlattenedHierarchy()
        {
            return new TestListItem[] { this };
        }
    }
}

