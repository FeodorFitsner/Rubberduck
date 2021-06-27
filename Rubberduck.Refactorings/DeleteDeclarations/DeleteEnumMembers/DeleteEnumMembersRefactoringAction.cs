﻿using Rubberduck.Parsing.Rewriter;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;
using Rubberduck.Refactorings.DeleteDeclarations.Abstract;
using System;
using System.Linq;

namespace Rubberduck.Refactorings.DeleteDeclarations
{
    public class DeleteEnumMembersRefactoringAction : DeleteElementRefactoringActionBase<DeleteEnumMembersModel>
    {
        public DeleteEnumMembersRefactoringAction(IDeclarationFinderProvider declarationFinderProvider, 
            IDeclarationDeletionTargetFactory targetFactory, 
            IDeclarationDeletionGroupsGeneratorFactory deletionGroupsGeneratorFactory,
            IRewritingManager rewritingManager)
            : base(declarationFinderProvider, targetFactory, deletionGroupsGeneratorFactory, rewritingManager)
        {}

        public override void Refactor(DeleteEnumMembersModel model, IRewriteSession rewriteSession)
        {
            if (model.Targets.Any(t => t.DeclarationType != DeclarationType.EnumerationMember))
            {
                throw new InvalidOperationException("Only DeclarationType.EnumerationMember can be refactored by this class");
            }

            DeleteDeclarations(model, rewriteSession);
        }
    }
}
