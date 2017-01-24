namespace Engines
{
    /**
     * The LateInit() method is run by AddEngine in the Context, guaranteeing that the Engine
     * has passed through the IEngine initialization before this method call.
     */
    public interface ILateInitEngine
    {
        void LateInit ();
    }
}
