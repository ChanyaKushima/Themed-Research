using System;
using System.Collections.Generic;
using System.Text;

namespace DeadlyOnline.Logic
{
    [Serializable]
    public class BehaviorInfo
    {
        private Behavior Behavior => BehaviorList.GetBehavior(_behaviorId);
        public string Name { get; }

        private int _behaviorId;
        private int _interval;

        private int BehaviorId
        {
            set
            {
                if (value < 0 || value >= BehaviorList.behaviorList.Count)
                {
                    ThrowHelper.ThrowArgumentOutOfRengeException_value();
                }
                _behaviorId = value;
            }
        }

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

        public BehaviorInfo(int behaviorId, string name, int gageUsage)
        {
            BehaviorId = behaviorId;
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
