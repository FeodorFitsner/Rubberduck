﻿using Antlr4.Runtime.Misc;
using Rubberduck.Inspections.Abstract;
using Rubberduck.Inspections.Results;
using Rubberduck.Parsing.Common;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Resources.Inspections;
using Rubberduck.Parsing.VBA;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.Inspections.Concrete
{
    [Experimental]
    internal class EmptyForEachBlockInspection : ParseTreeInspectionBase
    {
        public EmptyForEachBlockInspection(RubberduckParserState state)
            : base(state) { }

        protected override IEnumerable<IInspectionResult> DoGetInspectionResults()
        {
            return Listener.Contexts
                .Where(result => !IsIgnoringInspectionResultFor(result.ModuleName, result.Context.Start.Line))
                .Select(result => new QualifiedContextInspectionResult(this,
                                                        InspectionResults.EmptyForEachBlockInspection,
                                                        result));
        }

        public override IInspectionListener Listener { get; } =
            new EmptyForEachBlockListener();

        public class EmptyForEachBlockListener : EmptyBlockInspectionListenerBase
        {
            public override void EnterForEachStmt([NotNull] VBAParser.ForEachStmtContext context)
            {
                InspectBlockForExecutableStatements(context.block(), context);
            }
        }
    }
}
