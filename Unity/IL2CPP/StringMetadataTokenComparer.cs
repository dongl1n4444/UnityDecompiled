namespace Unity.IL2CPP
{
    using System;
    using System.Collections.Generic;

    public class StringMetadataTokenComparer : EqualityComparer<StringMetadataToken>
    {
        public static bool AreEqual(StringMetadataToken x, StringMetadataToken y) => 
            (object.ReferenceEquals(x, y) || (x.Literal == y.Literal));

        public override bool Equals(StringMetadataToken x, StringMetadataToken y) => 
            AreEqual(x, y);

        public override int GetHashCode(StringMetadataToken obj) => 
            obj.Literal.GetHashCode();
    }
}

