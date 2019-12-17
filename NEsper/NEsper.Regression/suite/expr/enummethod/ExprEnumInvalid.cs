///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.common.@internal.util;
using com.espertech.esper.compat;
using com.espertech.esper.regressionlib.framework;
using com.espertech.esper.regressionlib.support.bean;

namespace com.espertech.esper.regressionlib.suite.expr.enummethod
{
    public class ExprEnumInvalid : RegressionExecution
    {
        public void Run(RegressionEnvironment env)
        {
            string epl;

            // no parameter while one is expected
            epl = "select Contained.take() from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'Contained.take()': Parameters mismatch for enumeration method 'take', the method requires an (non-lambda) expression providing count [select Contained.take() from SupportBean_ST0_Container]");

            // primitive array property
            epl = "select ArrayProperty.where(x->x.BoolPrimitive) from SupportBeanComplexProps";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'ArrayProperty.where()': Error validating enumeration method 'where' parameter 0: Failed to validate declared expression body expression 'x.BoolPrimitive': Failed to resolve property 'x.BoolPrimitive' to a stream or nested property in a stream [select ArrayProperty.where(x->x.BoolPrimitive) from SupportBeanComplexProps]");

            // property not there
            epl = "select Contained.where(x->x.dummy = 1) from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'Contained.where()': Error validating enumeration method 'where' parameter 0: Failed to validate declared expression body expression 'x.dummy=1': Failed to resolve property 'x.dummy' to a stream or nested property in a stream [select Contained.where(x->x.dummy = 1) from SupportBean_ST0_Container]");
            epl = "select * from SupportBean(products.where(p -> code = '1'))";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate filter expression 'products.where()': Failed to resolve 'products.where' to a property, single-row function, aggregation function, script, stream or class name ");

            // test not an enumeration method
            epl = "select Contained.notAMethod(x->x.BoolPrimitive) from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'Contained.notAMethod()': Could not find event property, enumeration method or instance method named 'notAMethod' in collection of events of type '");

            // invalid lambda expression for non-lambda func
            epl = "select MakeTest(x->1) from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'MakeTest()': Unexpected lambda-expression encountered as parameter to UDF or static method 'MakeTest' [select MakeTest(x->1) from SupportBean_ST0_Container]");

            // invalid lambda expression for non-lambda func
            epl = "select SupportBean_ST0_Container.MakeTest(x->1) from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'SupportBean_ST0_Container.MakeTest()': Unexpected lambda-expression encountered as parameter to UDF or static method 'MakeTest' [select SupportBean_ST0_Container.MakeTest(x->1) from SupportBean_ST0_Container]");

            // invalid incompatible params
            epl = "select Contained.take('a') from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'Contained.take('a')': Failed to resolve enumeration method, date-time method or mapped property 'Contained.take('a')': Error validating enumeration method 'take', expected a number-type result for expression parameter 0 but received System.String [select Contained.take('a') from SupportBean_ST0_Container]");

            // invalid incompatible params
            epl = "select Contained.take(x -> x.P00) from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'Contained.take()': Parameters mismatch for enumeration method 'take', the method requires an (non-lambda) expression providing count, but receives a lambda expression [select Contained.take(x -> x.P00) from SupportBean_ST0_Container]");

            // invalid too many lambda parameter
            epl = "select Contained.where((x,y,z) -> true) from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'Contained.where()': Parameters mismatch for enumeration method 'where', the method requires a lambda expression providing predicate, but receives a 3-parameter lambda expression [select Contained.where((x,y,z) -> true) from SupportBean_ST0_Container]");

            // invalid no parameter
            epl = "select Contained.where() from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'Contained.where()': Parameters mismatch for enumeration method 'where', the method has multiple footprints accepting a lambda expression providing predicate, or a 2-parameter lambda expression providing (predicate, index), but receives no parameters [select Contained.where() from SupportBean_ST0_Container]");

            // invalid no parameter
            epl = "select window(IntPrimitive).takeLast() from SupportBean#length(2)";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'window(IntPrimitive).takeLast()': Parameters mismatch for enumeration method 'takeLast', the method requires an (non-lambda) expression providing count [select window(IntPrimitive).takeLast() from SupportBean#length(2)]");

            // invalid wrong parameter
            epl = "select Contained.where(x=>true,y->true) from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'Contained.where(,)': Parameters mismatch for enumeration method 'where', the method has multiple footprints accepting a lambda expression providing predicate, or a 2-parameter lambda expression providing (predicate, index), but receives a lambda expression and a lambda expression [select Contained.where(x=>true,y->true) from SupportBean_ST0_Container]");

            // invalid wrong parameter
            epl = "select Contained.where(1) from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'Contained.where(1)': Parameters mismatch for enumeration method 'where', the method requires a lambda expression providing predicate, but receives an (non-lambda) expression [select Contained.where(1) from SupportBean_ST0_Container]");

            // invalid too many parameter
            epl = "select Contained.where(1,2) from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'Contained.where(1,2)': Parameters mismatch for enumeration method 'where', the method has multiple footprints accepting a lambda expression providing predicate, or a 2-parameter lambda expression providing (predicate, index), but receives an (non-lambda) expression and an (non-lambda) expression [select Contained.where(1,2) from SupportBean_ST0_Container]");

            // subselect multiple columns
            epl =
                "select (select TheString, IntPrimitive from SupportBean#lastevent).where(x->x.BoolPrimitive) from SupportBean_ST0";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'TheString.where()': Error validating enumeration method 'where' parameter 0: Failed to validate declared expression body expression 'x.BoolPrimitive': Failed to resolve property 'x.BoolPrimitive' to a stream or nested property in a stream [select (select TheString, IntPrimitive from SupportBean#lastevent).where(x->x.BoolPrimitive) from SupportBean_ST0]");

            // subselect individual column
            epl = "select (select IntPrimitive from SupportBean#lastevent).where(x->x.BoolPrimitive) from SupportBean_ST0";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'IntPrimitive.where()': Error validating enumeration method 'where' parameter 0: Failed to validate declared expression body expression 'x.BoolPrimitive': Failed to resolve property 'x.BoolPrimitive' to a stream or nested property in a stream [select (select IntPrimitive from SupportBean#lastevent).where(x->x.BoolPrimitive) from SupportBean_ST0]");

            // aggregation
            epl = "select avg(IntPrimitive).where(x->x.BoolPrimitive) from SupportBean_ST0";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Incorrect syntax near '(' ('avg' is a reserved keyword) at line 1 column 10");

            // invalid incompatible params
            epl = "select Contained.allOf(x -> 1) from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'Contained.allOf()': Error validating enumeration method 'allOf', expected a boolean-type result for expression parameter 0 but received System.Int32 [select Contained.allOf(x -> 1) from SupportBean_ST0_Container]");

            // invalid incompatible params
            epl = "select Contained.allOf(x -> 1) from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'Contained.allOf()': Error validating enumeration method 'allOf', expected a boolean-type result for expression parameter 0 but received System.Int32 [select Contained.allOf(x -> 1) from SupportBean_ST0_Container]");

            // invalid incompatible params
            epl = "select Contained.aggregate(0, (result, item) -> result || ',') from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                $"Failed to validate select-clause expression 'Contained.aggregate(0,)': Error validating enumeration method 'aggregate' parameter 1: Failed to validate declared expression body expression 'result||\",\"': Implicit conversion from datatype '{typeof(int?).CleanName()}' to System.String is not allowed [select Contained.aggregate(0, (result, item) -> result || ',') from SupportBean_ST0_Container]");

            // invalid incompatible params
            epl = "select Contained.average(x -> x.Id) from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'Contained.average()': Error validating enumeration method 'average', expected a number-type result for expression parameter 0 but received System.String [select Contained.average(x -> x.Id) from SupportBean_ST0_Container]");

            // not a property
            epl = "select Contained.firstof().dummy from SupportBean_ST0_Container";
            SupportMessageAssertUtil.TryInvalidCompile(
                env,
                epl,
                "Failed to validate select-clause expression 'Contained.firstof().dummy()': Failed to resolve method 'dummy': Could not find enumeration method, date-time method or instance method named 'dummy' in class '" +
                typeof(SupportBean_ST0).CleanName() +
                "' taking no parameters [select Contained.firstof().dummy from SupportBean_ST0_Container]");
        }
    }
} // end of namespace