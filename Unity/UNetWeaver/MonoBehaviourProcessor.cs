namespace Unity.UNetWeaver
{
    using Mono.Cecil;
    using System;

    internal class MonoBehaviourProcessor
    {
        private TypeDefinition m_td;

        public MonoBehaviourProcessor(TypeDefinition td)
        {
            this.m_td = td;
        }

        public void Process()
        {
            this.ProcessSyncVars();
            this.ProcessMethods();
        }

        private void ProcessMethods()
        {
            foreach (MethodDefinition definition in this.m_td.Methods)
            {
                foreach (CustomAttribute attribute in definition.CustomAttributes)
                {
                    if (attribute.AttributeType.FullName == Weaver.CommandType.FullName)
                    {
                        Log.Error("Script " + this.m_td.FullName + " uses [Command] " + definition.Name + " but is not a NetworkBehaviour.");
                        Weaver.fail = true;
                    }
                    if (attribute.AttributeType.FullName == Weaver.ClientRpcType.FullName)
                    {
                        Log.Error("Script " + this.m_td.FullName + " uses [ClientRpc] " + definition.Name + " but is not a NetworkBehaviour.");
                        Weaver.fail = true;
                    }
                    if (attribute.AttributeType.FullName == Weaver.TargetRpcType.FullName)
                    {
                        Log.Error("Script " + this.m_td.FullName + " uses [TargetRpc] " + definition.Name + " but is not a NetworkBehaviour.");
                        Weaver.fail = true;
                    }
                    switch (attribute.Constructor.DeclaringType.ToString())
                    {
                        case "UnityEngine.Networking.ServerAttribute":
                            Log.Error("Script " + this.m_td.FullName + " uses the attribute [Server] on the method " + definition.Name + " but is not a NetworkBehaviour.");
                            Weaver.fail = true;
                            break;

                        case "UnityEngine.Networking.ServerCallbackAttribute":
                            Log.Error("Script " + this.m_td.FullName + " uses the attribute [ServerCallback] on the method " + definition.Name + " but is not a NetworkBehaviour.");
                            Weaver.fail = true;
                            break;

                        case "UnityEngine.Networking.ClientAttribute":
                            Log.Error("Script " + this.m_td.FullName + " uses the attribute [Client] on the method " + definition.Name + " but is not a NetworkBehaviour.");
                            Weaver.fail = true;
                            break;

                        case "UnityEngine.Networking.ClientCallbackAttribute":
                            Log.Error("Script " + this.m_td.FullName + " uses the attribute [ClientCallback] on the method " + definition.Name + " but is not a NetworkBehaviour.");
                            Weaver.fail = true;
                            break;
                    }
                }
            }
        }

        private void ProcessSyncVars()
        {
            foreach (FieldDefinition definition in this.m_td.Fields)
            {
                foreach (CustomAttribute attribute in definition.CustomAttributes)
                {
                    if (attribute.AttributeType.FullName == Weaver.SyncVarType.FullName)
                    {
                        Log.Error("Script " + this.m_td.FullName + " uses [SyncVar] " + definition.Name + " but is not a NetworkBehaviour.");
                        Weaver.fail = true;
                    }
                }
            }
        }
    }
}

