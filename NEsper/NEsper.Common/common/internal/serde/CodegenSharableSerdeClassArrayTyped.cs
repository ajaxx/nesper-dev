///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.common.@internal.context.module;
using com.espertech.esper.compat.collections;

using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionBuilder;

namespace com.espertech.esper.common.@internal.serde
{
    public class CodegenSharableSerdeClassArrayTyped : CodegenFieldSharable
    {
        private readonly CodegenSharableSerdeName name;
        private readonly Type[] valueTypes;

        public CodegenSharableSerdeClassArrayTyped(
            CodegenSharableSerdeName name,
            Type[] valueTypes)
        {
            this.name = name;
            this.valueTypes = valueTypes;
        }

        public Type Type()
        {
            return typeof(DataInputOutputSerdeWCollation<object>);
        }

        public CodegenExpression InitCtorScoped()
        {
            return ExprDotMethodChain(EPStatementInitServicesConstants.REF)
                .Get(EPStatementInitServicesConstants.DATAINPUTOUTPUTSERDEPROVIDER)
                .Add(name.MethodName, new Type[] { typeof(object) }, Constant(valueTypes));
        }

        public override bool Equals(object o)
        {
            if (this == o) {
                return true;
            }

            if (o == null || GetType() != o.GetType()) {
                return false;
            }

            var that = (CodegenSharableSerdeClassArrayTyped) o;

            if (name != that.name) {
                return false;
            }

            // Probably incorrect - comparing Object[] arrays with Arrays.equals
            return CompatExtensions.AreEqual(valueTypes, that.valueTypes);
        }

        public override int GetHashCode()
        {
            var result = name.GetHashCode();
            result = 31 * result + CompatExtensions.Hash(valueTypes);
            return result;
        }

        public class CodegenSharableSerdeName
        {
            public static readonly CodegenSharableSerdeName OBJECTARRAYMAYNULLNULL =
                new CodegenSharableSerdeName("ObjectArrayMayNullNull");

            private CodegenSharableSerdeName(string methodName)
            {
                MethodName = methodName;
            }

            public string MethodName { get; }
        }
    }
} // end of namespace