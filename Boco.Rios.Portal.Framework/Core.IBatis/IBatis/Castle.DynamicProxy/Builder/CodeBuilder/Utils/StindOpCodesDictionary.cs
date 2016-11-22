namespace Castle.DynamicProxy.Builder.CodeBuilder.Utils
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Reflection.Emit;

    public sealed class StindOpCodesDictionary : DictionaryBase
    {
        private static readonly StindOpCodesDictionary _dict = new StindOpCodesDictionary();
        private static readonly OpCode _emptyOpCode = new OpCode();

        private StindOpCodesDictionary()
        {
            base.Dictionary[typeof(bool)] = OpCodes.Stind_I1;
            base.Dictionary[typeof(sbyte)] = OpCodes.Stind_I1;
            base.Dictionary[typeof(short)] = OpCodes.Stind_I2;
            base.Dictionary[typeof(int)] = OpCodes.Stind_I4;
            base.Dictionary[typeof(long)] = OpCodes.Stind_I8;
            base.Dictionary[typeof(float)] = OpCodes.Stind_R4;
            base.Dictionary[typeof(double)] = OpCodes.Stind_R8;
            base.Dictionary[typeof(byte)] = OpCodes.Stind_I1;
            base.Dictionary[typeof(ushort)] = OpCodes.Stind_I2;
            base.Dictionary[typeof(uint)] = OpCodes.Stind_I4;
        }

        public static OpCode EmptyOpCode
        {
            get
            {
                return _emptyOpCode;
            }
        }

        public static StindOpCodesDictionary Instance
        {
            get
            {
                return _dict;
            }
        }

        public OpCode this[Type type]
        {
            get
            {
                if (base.Dictionary.Contains(type))
                {
                    return (OpCode) base.Dictionary[type];
                }
                return EmptyOpCode;
            }
        }
    }
}

