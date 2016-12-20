namespace Unity.UNetWeaver
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    internal class WeaverLists
    {
        public TypeDefinition generateContainerClass;
        public List<MethodDefinition> generatedReadFunctions = new List<MethodDefinition>();
        public List<MethodDefinition> generatedWriteFunctions = new List<MethodDefinition>();
        public List<FieldDefinition> netIdFields = new List<FieldDefinition>();
        public Dictionary<string, int> numSyncVars = new Dictionary<string, int>();
        public Dictionary<string, MethodReference> readByReferenceFuncs;
        public Dictionary<string, MethodReference> readFuncs;
        public List<EventDefinition> replacedEvents = new List<EventDefinition>();
        public List<FieldDefinition> replacedFields = new List<FieldDefinition>();
        public List<MethodDefinition> replacedMethods = new List<MethodDefinition>();
        public List<MethodDefinition> replacementEvents = new List<MethodDefinition>();
        public HashSet<string> replacementMethodNames = new HashSet<string>();
        public List<MethodDefinition> replacementMethods = new List<MethodDefinition>();
        public List<MethodDefinition> replacementProperties = new List<MethodDefinition>();
        public Dictionary<string, MethodReference> writeFuncs;
    }
}

