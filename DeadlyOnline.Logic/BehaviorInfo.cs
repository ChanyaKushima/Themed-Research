using System;
using System.Collections.Generic;
using System.Text;

namespace DeadlyOnline.Logic
{
    public class BehaviorInfo
    {
        private Behavior Behavior { get; set; }
        public string Name { get; }
        private int _interval;

        public int SpdCountUsage
        {
            get { return _interval; }
            set
            {
                if (value < 0)
                {
                    ThrowHelper.ThrowArgumentException();
                }
                _interval = value;
            }
        }

        public BehaviorInfo(Behavior behavior, string name, int gageUsage)
        {
            Behavior = behavior;
            Name = name;
            SpdCountUsage = gageUsage;
        }

        public void InvokeBehavior(CharaBaseData self, CharaBaseData target)
        {
            if (_interval > self.SpdCount)
            {
                ThrowHelper.ThrowInvalidOperationException();
            }

            Behavior behavior = Behavior;
            if (behavior != null)
            {
                behavior(self, target);
                self.SpdCount -= SpdCountUsage;
            }
        }
    }

    public delegate void Behavior(CharaBaseData self, CharaBaseData target);
}
