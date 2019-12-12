using System;
using System.Collections.Generic;
using System.Text;

namespace DeadlyOnline.Logic
{
    public class BehaviorInfo
    {
        public Behavior Behavior { protected get; set; }
        private int _interval;

        public int GageUsage
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

        public void InvokeBehavior(CharaBaseData self, CharaBaseData target)
        {
            if (_interval < self.SpdCount)
            {
                ThrowHelper.ThrowInvalidOperationException();
            }

            Behavior behavior = Behavior;
            if (behavior != null)
            {
                behavior(self, target);
                self.SpdCount -= GageUsage;
            }
        }
    }

    public delegate void Behavior(CharaBaseData self, CharaBaseData target);
}
