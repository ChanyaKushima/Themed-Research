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

        private readonly int _behaviorID;

        public int SpdCountUsage { get; }
        
        public BehaviorInfo(int behaviorID)
        {
            if (behaviorID < 0 || behaviorID >= BehaviorList.Count)
            {
                ThrowHelper.ThrowArgumentOutOfRengeException_value();
            }
            _behaviorID = behaviorID;

            (_behavior, Name, SpdCountUsage) = BehaviorList.GetCore(behaviorID);
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

            _behaviorID = id;

            (_behavior, Name, SpdCountUsage) = BehaviorList.GetCore(id);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("id", _behaviorID);
        }
    }

    public delegate void Behavior(CharaBaseData self, CharaBaseData target);
}
