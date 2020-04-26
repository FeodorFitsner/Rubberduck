﻿namespace Rubberduck.Parsing.Annotations
{
    /// <summary>
    /// @TestCleanup annotation, marks a procedure that the unit testing engine executes once after running each of the tests in a module.
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
    /// '...
    /// Private SUT As Class1
    /// 
    /// '@TestCleanup
    /// Private Sub TestCleanup()
    ///     Set SUT = Nothing
    /// End Sub
    /// ]]>
    /// </module>
    /// </example>
    public sealed class TestCleanupAnnotation : AnnotationBase, ITestAnnotation
    {
        public TestCleanupAnnotation()
            : base("TestCleanup", AnnotationTarget.Member)
        {}
    }
}
