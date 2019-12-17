///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2019 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;
using com.espertech.esper.compat.function;

namespace com.espertech.esper.common.@internal.bytecodemodel.core
{
    public class CodegenNamedMethods
    {
        private IDictionary<string, CodegenMethod> _methods;

        public IDictionary<string, CodegenMethod> Methods =>
            _methods ?? Collections.GetEmptyMap<string, CodegenMethod>();

        public CodegenMethod AddMethod(
            Type returnType,
            string methodName,
            IList<CodegenNamedParam> @params,
            Type generator,
            CodegenClassScope classScope,
            Consumer<CodegenMethod> code)
        {
            return AddMethodWithSymbols(
                returnType,
                methodName,
                @params,
                generator,
                classScope,
                code,
                CodegenSymbolProviderEmpty.INSTANCE);
        }

        public CodegenMethod AddMethodWithSymbols(
            Type returnType,
            string methodName,
            IList<CodegenNamedParam> @params,
            Type generator,
            CodegenClassScope classScope,
            Consumer<CodegenMethod> code,
            CodegenSymbolProvider symbolProvider)
        {
            if (_methods == null) {
                _methods = new Dictionary<string, CodegenMethod>();
            }

            var existing = _methods.Get(methodName);
            if (existing != null) {
                if (ListExtensions.AreEqual(@params, existing.LocalParams)) {
                    return existing;
                }

                throw new IllegalStateException("Method by name '" + methodName + "' already registered");
            }

            var method = CodegenMethod
                .MakeMethod(returnType, generator, symbolProvider, classScope)
                .AddParam(@params);
            method.Block.DebugStack();
            _methods.Put(methodName, method);
            code.Invoke(method);
            return method;
        }

        public CodegenMethod GetMethod(string name)
        {
            var method = _methods.Get(name);
            if (method == null) {
                throw new IllegalStateException("Method by name '" + name + "' not found");
            }

            return method;
        }
    }
} // end of namespace