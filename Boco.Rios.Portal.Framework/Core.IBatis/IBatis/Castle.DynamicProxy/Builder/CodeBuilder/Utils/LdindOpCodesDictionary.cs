namespace Castle.DynamicProxy.Builder.CodeBuilder.Utils
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Reflection.Emit;

    public sealed class LdindOpCodesDictionary : DictionaryBase
    {
        private static readonly LdindOpCodesDictionary _dict = new LdindOpCodesDictionary();
        private static readonly OpCode _emptyOpCode = new OpCode();

        private LdindOpCodesDictionary()
        {
            base.Dictionary[typeof(bool)] = OpCodes.Ldind_I1;
            base.Dictionary[typeof(sbyte)] = OpCodes.Ldind_I1;
            base.Dictionary[typeof(short)] = OpCodes.Ldind_I2;
            base.Dictionary[typeof(int)] = OpCodes.Ldind_I4;
            base.Dictionary[typeof(long)] = OpCodes.Ldind_I8;
            base.Dictionary[typeof(float)] = OpCodes.Ldind_R4;
            base.Dictionary[typeof(double)] = OpCodes.Ldind_R8;
            base.Dictionary[typeof(byte)] = OpCodes.Ldind_U1;
            base.Dictionary[typeof(ushort)] = OpCodes.Ldind_U2;
            base.Dictionary[typeof(uint)] = OpCodes.Ldind_U4;
        }

        public static OpCode EmptyOpCode
        {
            get
            {
                return _emptyOpCode;
            }
        }

        public static LdindOpCodesDictionary Instance
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

