namespace AlphaWork
{
    public /*abstract*/ class ProcedureBase : GameFramework.Procedure.ProcedureBase
    {
        public virtual/*abstract*/ bool UseNativeDialog
        {
            get;
        }

        public virtual void Go()
        {

        }
    }
}
