namespace Castle.DynamicProxy.Builder.CodeBuilder.Utils
{
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public abstract class ArgumentsUtil
    {
        protected ArgumentsUtil()
        {
        }

        public static Expression[] ConvertArgumentReferenceToExpression(ArgumentReference[] args)
        {
            Expression[] expressionArray = new Expression[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                expressionArray[i] = args[i].ToExpression();
            }
            return expressionArray;
        }

        public static ArgumentReference[] ConvertToArgumentReference(ParameterInfo[] args)
        {
            ArgumentReference[] referenceArray = new ArgumentReference[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                referenceArray[i] = new ArgumentReference(args[i].ParameterType);
            }
            return referenceArray;
        }

        public static ArgumentReference[] ConvertToArgumentReference(Type[] args)
        {
            ArgumentReference[] referenceArray = new ArgumentReference[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                referenceArray[i] = new ArgumentReference(args[i]);
            }
            return referenceArray;
        }

        public static void EmitLoadOwnerAndReference(Reference reference, ILGenerator il)
        {
            if (reference != null)
            {
                EmitLoadOwnerAndReference(reference.OwnerReference, il);
                reference.LoadReference(il);
            }
        }

        public static Type[] InitializeAndConvert(ArgumentReference[] args)
        {
            Type[] typeArray = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                args[i].Position = i + 1;
                typeArray[i] = args[i].Type;
            }
            return typeArray;
        }

        public static void InitializeArgumentsByPosition(ArgumentReference[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                args[i].Position = i + 1;
            }
        }
    }
}

