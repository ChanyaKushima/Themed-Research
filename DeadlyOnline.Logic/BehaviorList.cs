using System;
using System.Collections.Generic;
using System.Text;

namespace DeadlyOnline.Logic
{
    public static class BehaviorList
    {
        public static Behavior GetBehavior(int index) => behaviorList[index].Behavior;

        public static string GetName(int index)=> behaviorList[index].Name;
        public static int GetInterval(int index)=> behaviorList[index].Interval;

        public static (Behavior behavior, string name, int interval) GetCore(int index) => behaviorList[index];

        public static int Count => behaviorList.Count;

        private static readonly List<BehaviorCore> behaviorList = new List<BehaviorCore>()
        {
            new BehaviorCore(NomalAttack,"通常攻撃",1),
        };
        private static void NomalAttack(CharaBaseData self, CharaBaseData target)
        {
            target.Damage(self.AttackPower);
        }

        private readonly struct BehaviorCore
        {
            public readonly Behavior Behavior;
            public readonly string Name;
            public readonly int Interval;

            public BehaviorCore(Behavior behavior, string name, int interval) =>
                (Behavior, Name, Interval) = (behavior, name, interval);

            public static implicit operator (Behavior,string,int)(BehaviorCore core)=>
                (core.Behavior,core.Name,core.Interval);
        }
    }
}
