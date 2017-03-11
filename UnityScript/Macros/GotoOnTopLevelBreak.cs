namespace UnityScript.Macros
{
    using Boo.Lang.Compiler.Ast;
    using System;

    [Serializable]
    public class GotoOnTopLevelBreak : DepthFirstTransformer
    {
        protected LabelStatement _label;
        protected int _level;

        public GotoOnTopLevelBreak(LabelStatement label)
        {
            this._label = label;
        }

        public override void OnBreakStatement(BreakStatement node)
        {
            if (this._level <= 0)
            {
                this.ReplaceCurrentNode(SwitchMacroModule.NewGoto(this._label));
            }
        }

        public override void OnForStatement(ForStatement node)
        {
            this.OnLoopBody(node.Block);
        }

        public void OnLoopBody(Block block)
        {
            this._level++;
            block.Accept(this);
            this._level--;
        }

        public override void OnWhileStatement(WhileStatement node)
        {
            this.OnLoopBody(node.Block);
        }
    }
}

