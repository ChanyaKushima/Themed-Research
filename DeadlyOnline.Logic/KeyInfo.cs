namespace DeadlyOnline.Logic
{
    using System.Runtime.CompilerServices;
    using static System.Windows.Input.Keyboard;
    using static KeyConfig;
    public static class KeyInfo
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsKeyDown_Up() => IsKeyDown(Up1) || IsKeyDown(Up2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsKeyDown_Down() => IsKeyDown(Down1) || IsKeyDown(Down2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsKeyDown_Left() => IsKeyDown(Left1) || IsKeyDown(Left2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsKeyDown_Right() => IsKeyDown(Right1) || IsKeyDown(Right2);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsKeyDown_Select() => IsKeyDown(Select1) || IsKeyDown(Select2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsKeyDown_Cancel() => IsKeyDown(Cancel1) || IsKeyDown(Cancel2);
    }
}
