namespace UnityEditor.Animations
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Experimental.Director;
    using UnityEngineInternal;

    /// <summary>
    /// <para>The Animator Controller controls animation through layers with state machines, controlled by parameters.</para>
    /// </summary>
    public sealed class AnimatorController : RuntimeAnimatorController
    {
        private const string kControllerExtension = "controller";
        internal static AnimatorController lastActiveController = null;
        internal static int lastActiveLayerIndex = 0;
        internal Action OnAnimatorControllerDirty;
        internal PushUndoIfNeeded undoHandler = new PushUndoIfNeeded(true);

        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        public AnimatorController()
        {
            Internal_Create(this);
        }

        public T AddEffectiveStateMachineBehaviour<T>(AnimatorState state, int layerIndex) where T: StateMachineBehaviour
        {
            return (this.AddEffectiveStateMachineBehaviour(typeof(T), state, layerIndex) as T);
        }

        /// <summary>
        /// <para>Adds a state machine behaviour class of type stateMachineBehaviourType to the AnimatorState for layer layerIndex. This function should be used when you are dealing with synchronized layer and would like to add a state machine behaviour on a synchronized layer. C# Users can use a generic version.</para>
        /// </summary>
        /// <param name="stateMachineBehaviourType"></param>
        /// <param name="state"></param>
        /// <param name="layerIndex"></param>
        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public StateMachineBehaviour AddEffectiveStateMachineBehaviour(Type stateMachineBehaviourType, AnimatorState state, int layerIndex)
        {
            return (StateMachineBehaviour) this.Internal_AddStateMachineBehaviourWithType(stateMachineBehaviourType, state, layerIndex);
        }

        /// <summary>
        /// <para>Utility function to add a layer to the controller.</para>
        /// </summary>
        /// <param name="name">The name of the Layer.</param>
        /// <param name="layer">The layer to add.</param>
        public void AddLayer(string name)
        {
            AnimatorControllerLayer layer = new AnimatorControllerLayer {
                name = this.MakeUniqueLayerName(name),
                stateMachine = new AnimatorStateMachine()
            };
            layer.stateMachine.name = layer.name;
            layer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                AssetDatabase.AddObjectToAsset(layer.stateMachine, AssetDatabase.GetAssetPath(this));
            }
            this.AddLayer(layer);
        }

        /// <summary>
        /// <para>Utility function to add a layer to the controller.</para>
        /// </summary>
        /// <param name="name">The name of the Layer.</param>
        /// <param name="layer">The layer to add.</param>
        public void AddLayer(AnimatorControllerLayer layer)
        {
            this.undoHandler.DoUndo(this, "Layer added");
            AnimatorControllerLayer[] layers = this.layers;
            ArrayUtility.Add<AnimatorControllerLayer>(ref layers, layer);
            this.layers = layers;
        }

        /// <summary>
        /// <para>Utility function that creates a new state  with the motion in it.</para>
        /// </summary>
        /// <param name="motion">The Motion that will be in the AnimatorState.</param>
        /// <param name="layerIndex">The layer where the Motion will be added.</param>
        public AnimatorState AddMotion(Motion motion)
        {
            return this.AddMotion(motion, 0);
        }

        /// <summary>
        /// <para>Utility function that creates a new state  with the motion in it.</para>
        /// </summary>
        /// <param name="motion">The Motion that will be in the AnimatorState.</param>
        /// <param name="layerIndex">The layer where the Motion will be added.</param>
        public AnimatorState AddMotion(Motion motion, int layerIndex)
        {
            AnimatorState state = this.layers[layerIndex].stateMachine.AddState(motion.name);
            state.motion = motion;
            return state;
        }

        /// <summary>
        /// <para>Utility function to add a parameter to the controller.</para>
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        /// <param name="paramater">The parameter to add.</param>
        public void AddParameter(AnimatorControllerParameter paramater)
        {
            this.undoHandler.DoUndo(this, "Parameter added");
            AnimatorControllerParameter[] parameters = this.parameters;
            ArrayUtility.Add<AnimatorControllerParameter>(ref parameters, paramater);
            this.parameters = parameters;
        }

        /// <summary>
        /// <para>Utility function to add a parameter to the controller.</para>
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        /// <param name="paramater">The parameter to add.</param>
        public void AddParameter(string name, AnimatorControllerParameterType type)
        {
            AnimatorControllerParameter paramater = new AnimatorControllerParameter {
                name = this.MakeUniqueParameterName(name),
                type = type
            };
            this.AddParameter(paramater);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void AddStateEffectiveBehaviour(AnimatorState state, int layerIndex, int instanceID);
        public static AnimationClip AllocateAnimatorClip(string name)
        {
            AnimationClip clip = AnimationWindowUtility.AllocateAndSetupClip(true);
            clip.name = name;
            return clip;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool CanAddStateMachineBehaviours();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern Object[] CollectObjectsUsingParameter(string parameterName);
        internal static T[] ConvertStateMachineBehaviour<T>(ScriptableObject[] rawObjects) where T: StateMachineBehaviour
        {
            if (rawObjects == null)
            {
                return null;
            }
            T[] localArray2 = new T[rawObjects.Length];
            for (int i = 0; i < localArray2.Length; i++)
            {
                localArray2[i] = (T) rawObjects[i];
            }
            return localArray2;
        }

        /// <summary>
        /// <para>Creates an AnimatorController at the given path.</para>
        /// </summary>
        /// <param name="path">The path where the AnimatorController asset will be created.</param>
        /// <returns>
        /// <para>The created AnimationController or null if an error occured.</para>
        /// </returns>
        public static AnimatorController CreateAnimatorControllerAtPath(string path)
        {
            AnimatorController asset = new AnimatorController {
                name = Path.GetFileName(path)
            };
            AssetDatabase.CreateAsset(asset, path);
            asset.pushUndo = false;
            asset.AddLayer("Base Layer");
            asset.pushUndo = true;
            return asset;
        }

        /// <summary>
        /// <para>Creates an AnimatorController at the given path, and automatically create an AnimatorLayer  with an AnimatorStateMachine that will add a State with the AnimationClip in it.</para>
        /// </summary>
        /// <param name="path">The path where the AnimatorController will be created.</param>
        /// <param name="clip">The default clip that will be played by the AnimatorController.</param>
        public static AnimatorController CreateAnimatorControllerAtPathWithClip(string path, AnimationClip clip)
        {
            AnimatorController controller = CreateAnimatorControllerAtPath(path);
            controller.AddMotion(clip);
            return controller;
        }

        internal static AnimatorController CreateAnimatorControllerForClip(AnimationClip clip, GameObject animatedObject)
        {
            string assetPath = AssetDatabase.GetAssetPath(clip);
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }
            assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(FileUtil.DeleteLastPathNameComponent(assetPath), animatedObject.name + ".controller"));
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }
            return CreateAnimatorControllerAtPathWithClip(assetPath, clip);
        }

        public AnimatorState CreateBlendTreeInController(string name, out BlendTree tree)
        {
            return this.CreateBlendTreeInController(name, out tree, 0);
        }

        public AnimatorState CreateBlendTreeInController(string name, out BlendTree tree, int layerIndex)
        {
            tree = new BlendTree();
            tree.name = name;
            string defaultBlendTreeParameter = this.GetDefaultBlendTreeParameter();
            tree.blendParameterY = defaultBlendTreeParameter;
            tree.blendParameter = defaultBlendTreeParameter;
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                AssetDatabase.AddObjectToAsset(tree, AssetDatabase.GetAssetPath(this));
            }
            AnimatorState state = this.layers[layerIndex].stateMachine.AddState(tree.name);
            state.motion = tree;
            return state;
        }

        /// <summary>
        /// <para>This function will create a StateMachineBehaviour instance based on the class define in this script.</para>
        /// </summary>
        /// <param name="script">MonoScript class to instantiate.</param>
        /// <returns>
        /// <para>Returns instance id of created object, returns 0 if something is not valid.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int CreateStateMachineBehaviour(MonoScript script);
        internal static AnimatorControllerPlayable FindAnimatorControllerPlayable(Animator animator, AnimatorController controller)
        {
            AnimatorControllerPlayable ret = new AnimatorControllerPlayable();
            FindAnimatorControllerPlayableInternal(ref ret, animator, controller);
            return ret;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void FindAnimatorControllerPlayableInternal(ref AnimatorControllerPlayable ret, Animator animator, AnimatorController controller);
        internal AnimatorStateMachine FindEffectiveRootStateMachine(int layerIndex)
        {
            AnimatorControllerLayer layer = this.layers[layerIndex];
            while (layer.syncedLayerIndex != -1)
            {
                layer = this.layers[layer.syncedLayerIndex];
            }
            return layer.stateMachine;
        }

        /// <summary>
        /// <para>Use this function to retrieve the owner of this behaviour.</para>
        /// </summary>
        /// <param name="behaviour">The State Machine Behaviour to get context for.</param>
        /// <returns>
        /// <para>Returns the State Machine Behaviour edition context.</para>
        /// </returns>
        public static StateMachineBehaviourContext[] FindStateMachineBehaviourContext(StateMachineBehaviour behaviour)
        {
            return Internal_FindStateMachineBehaviourContext(behaviour);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern MonoScript GetBehaviourMonoScript(AnimatorState state, int layerIndex, int behaviourIndex);
        public T[] GetBehaviours<T>() where T: StateMachineBehaviour
        {
            return ConvertStateMachineBehaviour<T>(this.GetBehaviours(typeof(T)));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern ScriptableObject[] GetBehaviours(Type type);
        internal string GetDefaultBlendTreeParameter()
        {
            for (int i = 0; i < this.parameters.Length; i++)
            {
                if (this.parameters[i].type == AnimatorControllerParameterType.Float)
                {
                    return this.parameters[i].name;
                }
            }
            this.AddParameter("Blend", AnimatorControllerParameterType.Float);
            return "Blend";
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern AnimatorController GetEffectiveAnimatorController(Animator animator);
        /// <summary>
        /// <para>Gets the effective state machine behaviour list for the AnimatorState. Behaviours are either stored in the AnimatorStateMachine or in the AnimatorLayer's ovverrides. Use this function to get Behaviour list that is effectively used.</para>
        /// </summary>
        /// <param name="state">The AnimatorState which we want the Behaviour list.</param>
        /// <param name="layerIndex">The layer that is queried.</param>
        public StateMachineBehaviour[] GetStateEffectiveBehaviours(AnimatorState state, int layerIndex)
        {
            return this.Internal_GetEffectiveBehaviours(state, layerIndex);
        }

        /// <summary>
        /// <para>Gets the effective Motion for the AnimatorState. The Motion is either stored in the AnimatorStateMachine or in the AnimatorLayer's ovverrides. Use this function to get the Motion that is effectively used.</para>
        /// </summary>
        /// <param name="state">The AnimatorState which we want the Motion.</param>
        /// <param name="layerIndex">The layer that is queried.</param>
        public Motion GetStateEffectiveMotion(AnimatorState state)
        {
            return this.GetStateEffectiveMotion(state, 0);
        }

        /// <summary>
        /// <para>Gets the effective Motion for the AnimatorState. The Motion is either stored in the AnimatorStateMachine or in the AnimatorLayer's ovverrides. Use this function to get the Motion that is effectively used.</para>
        /// </summary>
        /// <param name="state">The AnimatorState which we want the Motion.</param>
        /// <param name="layerIndex">The layer that is queried.</param>
        public Motion GetStateEffectiveMotion(AnimatorState state, int layerIndex)
        {
            if (this.layers[layerIndex].syncedLayerIndex == -1)
            {
                return state.motion;
            }
            return this.layers[layerIndex].GetOverrideMotion(state);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern int IndexOfParameter(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern ScriptableObject Internal_AddStateMachineBehaviourWithType(Type stateMachineBehaviourType, AnimatorState state, int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Internal_Create(AnimatorController mono);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern StateMachineBehaviourContext[] Internal_FindStateMachineBehaviourContext(ScriptableObject scriptableObject);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern StateMachineBehaviour[] Internal_GetEffectiveBehaviours(AnimatorState state, int layerIndex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void Internal_SetEffectiveBehaviours(AnimatorState state, int layerIndex, StateMachineBehaviour[] behaviours);
        /// <summary>
        /// <para>Creates a unique name for the layers.</para>
        /// </summary>
        /// <param name="name">The desired name of the AnimatorLayer.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string MakeUniqueLayerName(string name);
        /// <summary>
        /// <para>Creates a unique name for the parameter.</para>
        /// </summary>
        /// <param name="name">The desired name of the AnimatorParameter.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern string MakeUniqueParameterName(string name);
        internal static void OnInvalidateAnimatorController(AnimatorController controller)
        {
            if (controller.OnAnimatorControllerDirty != null)
            {
                controller.OnAnimatorControllerDirty.Invoke();
            }
        }

        /// <summary>
        /// <para>Utility function to remove a layer from the controller.</para>
        /// </summary>
        /// <param name="index">The index of the AnimatorLayer.</param>
        public void RemoveLayer(int index)
        {
            this.undoHandler.DoUndo(this, "Layer removed");
            AnimatorControllerLayer[] layers = this.layers;
            this.RemoveLayerInternal(index, ref layers);
            this.layers = layers;
        }

        private void RemoveLayerInternal(int index, ref AnimatorControllerLayer[] layerVector)
        {
            if ((layerVector[index].syncedLayerIndex == -1) && (layerVector[index].stateMachine != null))
            {
                this.undoHandler.DoUndo(layerVector[index].stateMachine, "Layer removed");
                layerVector[index].stateMachine.Clear();
                if (MecanimUtilities.AreSameAsset(this, layerVector[index].stateMachine))
                {
                    Undo.DestroyObjectImmediate(layerVector[index].stateMachine);
                }
            }
            ArrayUtility.Remove<AnimatorControllerLayer>(ref layerVector, layerVector[index]);
        }

        internal void RemoveLayers(List<int> layerIndexes)
        {
            this.undoHandler.DoUndo(this, "Layers removed");
            AnimatorControllerLayer[] layers = this.layers;
            foreach (int num in layerIndexes)
            {
                this.RemoveLayerInternal(num, ref layers);
            }
            this.layers = layers;
        }

        /// <summary>
        /// <para>Utility function to remove a parameter from the controller.</para>
        /// </summary>
        /// <param name="index">The index of the AnimatorParameter.</param>
        public void RemoveParameter(int index)
        {
            this.undoHandler.DoUndo(this, "Parameter removed");
            AnimatorControllerParameter[] parameters = this.parameters;
            ArrayUtility.Remove<AnimatorControllerParameter>(ref parameters, parameters[index]);
            this.parameters = parameters;
        }

        public void RemoveParameter(AnimatorControllerParameter parameter)
        {
            this.undoHandler.DoUndo(this, "Parameter removed");
            AnimatorControllerParameter[] parameters = this.parameters;
            ArrayUtility.Remove<AnimatorControllerParameter>(ref parameters, parameter);
            this.parameters = parameters;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void RemoveStateEffectiveBehaviour(AnimatorState state, int layerIndex, int behaviourIndex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern void RenameParameter(string prevName, string newName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetAnimatorController(Animator behavior, AnimatorController controller);
        public void SetStateEffectiveBehaviours(AnimatorState state, int layerIndex, StateMachineBehaviour[] behaviours)
        {
            if (this.layers[layerIndex].syncedLayerIndex == -1)
            {
                this.undoHandler.DoUndo(state, "Set Behaviours");
                state.behaviours = behaviours;
            }
            else
            {
                this.undoHandler.DoUndo(this, "Set Behaviours");
                this.Internal_SetEffectiveBehaviours(state, layerIndex, behaviours);
            }
        }

        /// <summary>
        /// <para>Sets the effective Motion for the AnimatorState. The Motion is either stored in the AnimatorStateMachine or in the AnimatorLayer's ovverrides. Use this function to set the Motion that is effectively used.</para>
        /// </summary>
        /// <param name="state">The AnimatorState which we want to set the Motion.</param>
        /// <param name="motion">The Motion that will be set.</param>
        /// <param name="layerIndex">The layer to set the Motion.</param>
        public void SetStateEffectiveMotion(AnimatorState state, Motion motion)
        {
            this.SetStateEffectiveMotion(state, motion, 0);
        }

        /// <summary>
        /// <para>Sets the effective Motion for the AnimatorState. The Motion is either stored in the AnimatorStateMachine or in the AnimatorLayer's ovverrides. Use this function to set the Motion that is effectively used.</para>
        /// </summary>
        /// <param name="state">The AnimatorState which we want to set the Motion.</param>
        /// <param name="motion">The Motion that will be set.</param>
        /// <param name="layerIndex">The layer to set the Motion.</param>
        public void SetStateEffectiveMotion(AnimatorState state, Motion motion, int layerIndex)
        {
            if (this.layers[layerIndex].syncedLayerIndex == -1)
            {
                this.undoHandler.DoUndo(state, "Set Motion");
                state.motion = motion;
            }
            else
            {
                this.undoHandler.DoUndo(this, "Set Motion");
                AnimatorControllerLayer[] layers = this.layers;
                layers[layerIndex].SetOverrideMotion(state, motion);
                this.layers = layers;
            }
        }

        internal bool isAssetBundled { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        [Obsolete("layerCount is obsolete. Use layers.Length instead.", true)]
        private int layerCount
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// <para>The layers in the controller.</para>
        /// </summary>
        public AnimatorControllerLayer[] layers { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("parameterCount is obsolete. Use parameters.Length instead.", true)]
        private int parameterCount
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// <para>Parameters are used to communicate between scripting and the controller. They are used to drive transitions and blendtrees for example.</para>
        /// </summary>
        public AnimatorControllerParameter[] parameters { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal bool pushUndo
        {
            set
            {
                this.undoHandler.pushUndo = value;
            }
        }
    }
}

