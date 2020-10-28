﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class StaticFieldDeclarationOrchestrator : NodeSpecificOrchestrator<FieldDeclarationSyntax, BaseFieldDeclarationSyntax>
    {
        protected override bool CanHandle(FieldDeclarationSyntax t)
        {
            return t.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword);
        }

        protected override BaseFieldDeclarationSyntax OrchestrateChildrenMutation(FieldDeclarationSyntax node, MutationContext context)
        {
            using var newContext = context.EnterStatic();
            // we need to signal we are in a static field
            return base.OrchestrateChildrenMutation(node, newContext);
        }

        public StaticFieldDeclarationOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}