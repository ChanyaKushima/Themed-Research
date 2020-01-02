using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DeadlyOnline.Logic
{
    [Serializable]
    public class BehaviorInfo: ISerializable
    {
        private readonly Behavior _behavior;

        public string Name { get; }

        private readonly int _behaviorId;

        public int SpdCountUsage { get; }
        
        public BehaviorInfo(int behaviorId)
        {
            if (behaviorId < 0 || behaviorId >= BehaviorList.Count)
            {
                ThrowHelper.ThrowArgumentOutOfRengeException_value();
            }
            _behaviorId = behaviorId;

            (_behavior, Name, SpdCountUsage) = BehaviorList.GetCore(behaviorId);
        }

        public void InvokeBehavior(CharaBaseData self, CharaBaseData target)
        {
            if (SpdCountUsage > self.SpdCount)
            {
                ThrowHelper.ThrowInvalidOperationException();
            }

            Behavior behavior = _behavior;
            if (behavior != null)
            {
                behavior(self, target);
                self.SpdCount -= SpdCountUsage;
            }
        }

        public BehaviorInfo(SerializationInfo info, StreamingContext context)
        {
            int id = (int)info.GetValue("id", typeof(int));

            _behaviorId = id;

            (_behavior, Name, SpdCountUsage) = BehaviorList.GetCore(id);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("id", _behaviorId);
        }
    }

    public delegate void Behavior(CharaBaseData self, CharaBaseData target);
}
