﻿namespace com.espertech.esper.compiler.@internal.util
{
    public partial class RoslynCompiler
    {
        public interface Source
        {
            string Name { get; }
            string Code { get; }
        }
    }
}