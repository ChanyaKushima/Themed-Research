using System;
using System.Collections.Generic;
using System.Text;

namespace DeadlyOnline.Logic
{
    public static class BehaviorList
    {
        public static Behavior GetBehavior(int index) => 
            behaviorList[index];

        internal static readonly List<Behavior> behaviorList = new List<Behavior>()
        {
            NomalAttack
        };
        private static void NomalAttack(CharaBaseData self,CharaBaseData target)
        {
            target.Damage(self.AttackPower);
        }
    }
}
