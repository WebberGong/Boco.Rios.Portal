namespace Castle.DynamicProxy.Builder.CodeBuilder.Utils
{
    using System;
    using System.Reflection.Emit;

    internal abstract class OpCodeUtil
    {
        protected OpCodeUtil()
        {
        }

        public static void EmitLoadIndirectOpCodeForType(ILGenerator gen, Type type)
        {
            if (type.IsEnum)
            {
                EmitLoadIndirectOpCodeForType(gen, GetUnderlyingTypeOfEnum(type));
            }
            else
            {
                if (type.IsByRef)
                {
                    throw new NotSupportedException("Cannot load ByRef values");
                }
                if (type.IsPrimitive)
                {
                    OpCode objA = LdindOpCodesDictionary.Instance[type];
                    if (object.ReferenceEquals(objA, LdindOpCodesDictionary.EmptyOpCode))
                    {
                        throw new ArgumentException("Type " + type + " could not be converted to a OpCode");
                    }
                    gen.Emit(objA);
                }
                else if (type.IsValueType)
                {
                    gen.Emit(OpCodes.Ldobj, type);
                }
                else
                {
                    gen.Emit(OpCodes.Ldind_Ref);
                }
            }
        }

        public static void EmitLoadOpCodeForConstantValue(ILGenerator gen, object value)
        {
            if (value is string)
            {
                gen.Emit(OpCodes.Ldstr, value.ToString());
            }
            else if (value is int)
            {
                OpCode opcode = LdcOpCodesDictionary.Instance[value.GetType()];
                gen.Emit(opcode, (int) value);
            }
            else
            {
                if (!(value is bool))
                {
                    throw new NotSupportedException();
                }
                OpCode code2 = LdcOpCodesDictionary.Instance[value.GetType()];
                gen.Emit(code2, Convert.ToInt32(value));
            }
        }

        public static void EmitLoadOpCodeForDefaultValueOfType(ILGenerator gen, Type type)
        {
            if (type.IsPrimitive)
            {
                gen.Emit(LdcOpCodesDictionary.Instance[type], 0);
            }
            else
            {
                gen.Emit(OpCodes.Ldnull);
            }
        }

        public static void EmitStoreIndirectOpCodeForType(ILGenerator gen, Type type)
        {
            if (type.IsEnum)
            {
                EmitStoreIndirectOpCodeForType(gen, GetUnderlyingTypeOfEnum(type));
            }
            else
            {
                if (type.IsByRef)
                {
                    throw new NotSupportedException("Cannot store ByRef values");
                }
                if (type.IsPrimitive)
                {
                    OpCode objA = StindOpCodesDictionary.Instance[type];
                    if (object.ReferenceEquals(objA, StindOpCodesDictionary.EmptyOpCode))
                    {
                        throw new ArgumentException("Type " + type + " could not be converted to a OpCode");
                    }
                    gen.Emit(objA);
                }
                else if (type.IsValueType)
                {
                    gen.Emit(OpCodes.Stobj, type);
                }
                else
                {
                    gen.Emit(OpCodes.Stind_Ref);
                }
            }
        }

        private static Type GetUnderlyingTypeOfEnum(Type enumType)
        {
            Enum enum2 = (Enum) Activator.CreateInstance(enumType);
            switch (enum2.GetTypeCode())
            {
                case TypeCode.SByte:
                    return typeof(sbyte);

                case TypeCode.Byte:
                    return typeof(byte);

                case TypeCode.Int16:
                    return typeof(short);

                case TypeCode.Int32:
                    return typeof(int);

                case TypeCode.Int64:
                    return typeof(long);
            }
            throw new NotSupportedException();
        }
    }
}

