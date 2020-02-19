﻿using System;
using System.Linq;
using Antlr4.Runtime;
using Rubberduck.Parsing;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Rewriter;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;
using Rubberduck.Parsing.VBA.Parsing;
using Rubberduck.Refactorings.AddInterfaceImplementations;
using Rubberduck.VBEditor.ComManagement;
using Rubberduck.VBEditor.SafeComWrappers;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using Rubberduck.VBEditor.Utility;

namespace Rubberduck.Refactorings.ExtractInterface
{
    public class ExtractInterfaceRefactoringAction : RefactoringActionWithSuspension<ExtractInterfaceModel>
    {
        private readonly ICodeOnlyRefactoringAction<AddInterfaceImplementationsModel> _addImplementationsRefactoringAction;
        private readonly IParseTreeProvider _parseTreeProvider;
        private readonly IProjectsProvider _projectsProvider;
        private readonly IAddComponentService _addComponentService;

        public ExtractInterfaceRefactoringAction(
            AddInterfaceImplementationsRefactoringAction addImplementationsRefactoringAction,
            IParseTreeProvider parseTreeProvider,
            IParseManager parseManager, 
            IRewritingManager rewritingManager,
            IProjectsProvider projectsProvider,
            IAddComponentService addComponentService) 
            : base(parseManager, rewritingManager)
        {
            _addImplementationsRefactoringAction = addImplementationsRefactoringAction;
            _parseTreeProvider = parseTreeProvider;
            _projectsProvider = projectsProvider;
            _addComponentService = addComponentService;
        }

        protected override bool RequiresSuspension(ExtractInterfaceModel model)
        {
            return true;
        }

        protected override void Refactor(ExtractInterfaceModel model, IRewriteSession rewriteSession)
        {
            AddInterface(model, rewriteSession);
        }

        private void AddInterface(ExtractInterfaceModel model, IRewriteSession rewriteSession)
        {
            var targetProject = _projectsProvider.Project(model.TargetDeclaration.ProjectId);
            if (targetProject == null)
            {
                return; //The target project is not available.
            }

            AddInterfaceClass(model);
            AddImplementsStatement(model, rewriteSession);
            AddInterfaceMembersToClass(model, rewriteSession);
        }

        private void AddInterfaceClass(ExtractInterfaceModel model)
        {
            var targetProjectId = model.TargetDeclaration.ProjectId;
            var interfaceCode = InterfaceModuleBody(model);
            var interfaceName = model.InterfaceName;

            _addComponentService.AddComponent(targetProjectId, ComponentType.ClassModule, interfaceCode, componentName: interfaceName);
        }

        private static string InterfaceModuleBody(ExtractInterfaceModel model)
        {
            var interfaceMembers = string.Join(Environment.NewLine, model.SelectedMembers.Select(m => m.Body));
            var optionExplicit = $"{Tokens.Option} {Tokens.Explicit}{Environment.NewLine}";
            var interfaceAnnotation = new InterfaceAnnotation();
            var interfaceAnnotationText = $"'@{interfaceAnnotation.Name}{Environment.NewLine}";

            return $"{optionExplicit}{Environment.NewLine}{interfaceAnnotationText}{Environment.NewLine}{interfaceMembers}";
        }

        private void AddImplementsStatement(ExtractInterfaceModel model, IRewriteSession rewriteSession)
        {
            var rewriter = rewriteSession.CheckOutModuleRewriter(model.TargetDeclaration.QualifiedModuleName);

            var implementsStatement = $"Implements {model.InterfaceName}";

            var (insertionIndex, isImplementsStatement) = InsertionIndex(model);

            if (insertionIndex == -1)
            {
                rewriter.InsertBefore(0, $"{implementsStatement}{Environment.NewLine}{Environment.NewLine}");
            }
            else
            {
                rewriter.InsertAfter(insertionIndex, $"{Environment.NewLine}{(isImplementsStatement ? string.Empty : Environment.NewLine)}{implementsStatement}");
            }
        }

        private (int index, bool isImplementsStatement) InsertionIndex(ExtractInterfaceModel model)
        {
            var tree = (ParserRuleContext)_parseTreeProvider.GetParseTree(model.TargetDeclaration.QualifiedModuleName, CodeKind.CodePaneCode);

            var moduleDeclarations = tree.GetDescendent<VBAParser.ModuleDeclarationsContext>();
            if (moduleDeclarations == null)
            {
                return (-1, false);
            }

            var lastImplementsStatement = moduleDeclarations
                .GetDescendents<VBAParser.ImplementsStmtContext>()
                .LastOrDefault();
            if (lastImplementsStatement != null)
            {
                return (lastImplementsStatement.Stop.TokenIndex, true);
            }

            var lastOptionStatement = moduleDeclarations
                .GetDescendents<VBAParser.ModuleOptionContext>()
                .LastOrDefault();
            if (lastOptionStatement != null)
            {
                return (lastOptionStatement.Stop.TokenIndex, false);
            }

            return (-1, false);
        }

        private void AddInterfaceMembersToClass(ExtractInterfaceModel model, IRewriteSession rewriteSession)
        {
            var targetModule = model.TargetDeclaration.QualifiedModuleName;
            var interfaceName = model.InterfaceName;
            var membersToImplement = model.SelectedMembers.Select(m => m.Member).ToList();

            var addMembersModel = new AddInterfaceImplementationsModel(targetModule, interfaceName, membersToImplement);
            _addImplementationsRefactoringAction.Refactor(addMembersModel, rewriteSession);
        }

        private static readonly DeclarationType[] ModuleTypes =
        {
            DeclarationType.ClassModule,
            DeclarationType.Document,
            DeclarationType.UserForm
        };
    }
}