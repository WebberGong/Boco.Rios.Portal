namespace Castle.DynamicProxy.Builder.CodeBuilder.Utils
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Reflection.Emit;

    public sealed class LdcOpCodesDictionary : DictionaryBase
    {
        private static readonly LdcOpCodesDictionary _dict = new LdcOpCodesDictionary();
        private static readonly OpCode _emptyOpCode = new OpCode();

        private LdcOpCodesDictionary()
        {
            base.Dictionary[typeof(bool)] = OpCodes.Ldc_I4;
            base.Dictionary[typeof(sbyte)] = OpCodes.Ldc_I4;
            base.Dictionary[typeof(short)] = OpCodes.Ldc_I4;
            base.Dictionary[typeof(int)] = OpCodes.Ldc_I4;
            base.Dictionary[typeof(long)] = OpCodes.Ldc_I8;
            base.Dictionary[typeof(float)] = OpCodes.Ldc_R4;
            base.Dictionary[typeof(double)] = OpCodes.Ldc_R8;
            base.Dictionary[typeof(byte)] = OpCodes.Ldc_I4_0;
            base.Dictionary[typeof(ushort)] = OpCodes.Ldc_I4_0;
            base.Dictionary[typeof(uint)] = OpCodes.Ldc_I4_0;
        }

        public static OpCode EmptyOpCode
        {
            get
            {
                return _emptyOpCode;
            }
        }

        public static LdcOpCodesDictionary Instance
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

