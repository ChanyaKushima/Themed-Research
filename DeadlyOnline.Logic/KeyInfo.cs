namespace DeadlyOnline.Logic
{
    using System.Runtime.CompilerServices;
    using static System.Windows.Input.Keyboard;
    using static KeyConfig;
    public static class KeyInfo
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool UpKeyIsDown() => IsKeyDown(Up1) || IsKeyDown(Up2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DownKeyIsDown() => IsKeyDown(Down1) || IsKeyDown(Down2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LeftKeyIsDown() => IsKeyDown(Left1) || IsKeyDown(Left2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RightKeyIsDown() => IsKeyDown(Right1) || IsKeyDown(Right2);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SelectKeyIsDown() => IsKeyDown(Select1) || IsKeyDown(Select2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CancelKeyIsDown() => IsKeyDown(Cancel1) || IsKeyDown(Cancel2);
    }
}
