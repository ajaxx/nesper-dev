///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.common.client;
using com.espertech.esper.common.@internal.bytecodemodel.@base;
using com.espertech.esper.common.@internal.bytecodemodel.model.expression;
using com.espertech.esper.common.@internal.epl.enummethod.dot;
using com.espertech.esper.common.@internal.epl.enummethod.eval.singlelambdaopt3form.@base;
using com.espertech.esper.common.@internal.epl.expression.codegen;
using com.espertech.esper.common.@internal.epl.expression.core;
using com.espertech.esper.common.@internal.@event.arr;
using com.espertech.esper.compat.collections;

using static com.espertech.esper.common.@internal.bytecodemodel.model.expression.CodegenExpressionBuilder;

namespace com.espertech.esper.common.@internal.epl.enummethod.eval.singlelambdaopt3form.countof
{
	public class EnumCountOfEventPlus : ThreeFormEventPlus {
	    public EnumCountOfEventPlus(ExprDotEvalParamLambda lambda, ObjectArrayEventType indexEventType, int numParameters)
			: base(lambda, indexEventType, numParameters)
	    {
	    }

	    public override EnumEval EnumEvaluator {
		    get {
			    ExprEvaluator inner = InnerExpression.ExprEvaluator;

			    return new ProxyEnumEval(
				    (
					    eventsLambda,
					    enumcoll,
					    isNewData,
					    context) => {
					    if (enumcoll.IsEmpty()) {
						    return 0;
					    }

					    ICollection<EventBean> beans = (ICollection<EventBean>) enumcoll;
					    ObjectArrayEventBean indexEvent = new ObjectArrayEventBean(new object[2], FieldEventType);
					    eventsLambda[StreamNumLambda + 1] = indexEvent;
					    object[] props = indexEvent.Properties;
					    props[1] = enumcoll.Count;
					    int rowcount = 0;
					    int count = -1;

					    foreach (EventBean next in beans) {
						    count++;
						    props[0] = count;
						    eventsLambda[StreamNumLambda] = next;

						    object pass = inner.Evaluate(eventsLambda, isNewData, context);
						    if (pass == null || false.Equals(pass)) {
							    continue;
						    }

						    rowcount++;
					    }

					    return rowcount;
				    });
		    }
	    }

	    public override Type ReturnType() {
	        return typeof(int);
	    }

	    public override CodegenExpression ReturnIfEmptyOptional() {
	        return Constant(0);
	    }

	    public override void InitBlock(CodegenBlock block, CodegenMethod methodNode, ExprForgeCodegenSymbol scope, CodegenClassScope codegenClassScope) {
	        block.DeclareVar<int>("rowcount", Constant(0));
	    }

	    public override void ForEachBlock(CodegenBlock block, CodegenMethod methodNode, ExprForgeCodegenSymbol scope, CodegenClassScope codegenClassScope) {
	        CodegenLegoBooleanExpression.CodegenContinueIfNotNullAndNotPass(block, InnerExpression.EvaluationType, InnerExpression.EvaluateCodegen(typeof(bool?), methodNode, scope, codegenClassScope));
	        block.IncrementRef("rowcount");
	    }

	    public override void ReturnResult(CodegenBlock block) {
	        block.MethodReturn(Ref("rowcount"));
	    }
	}
} // end of namespace
