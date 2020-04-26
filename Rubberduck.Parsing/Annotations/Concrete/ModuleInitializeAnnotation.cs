﻿namespace Rubberduck.Parsing.Annotations
{
    /// <summary>
    /// @ModuleInitialize annotation, marks a procedure that the unit testing engine executes before running the first test of a module.
    /// </summary>
    /// <parameter>
    /// This annotation takes no argument.
    /// </parameter>
    /// <example>
    /// <module name="TestModule1" type="Standard Module">
    /// <![CDATA[
    /// Option Explicit
    /// '@TestModule
    /// 
    /// Private Assert As Rubberduck.AssertClass
    /// 
    /// '@ModuleInitialize
    /// Private Sub ModuleInitialize()
    ///     Set Assert = New Rubberduck.AssertClass
    /// End Sub
    /// ]]>
    /// </module>
    /// </example>
    public sealed class ModuleInitializeAnnotation : AnnotationBase, ITestAnnotation
    {
        public ModuleInitializeAnnotation()
            : base("ModuleInitialize", AnnotationTarget.Member)
        {}
    }
}
