using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RemoveRegionAnalyzerAndCodeFix
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveRegionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "RemoveRegionAnalyzer";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, ImmutableArray.Create(SyntaxKind.RegionDirectiveTrivia, SyntaxKind.EndRegionDirectiveTrivia));
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation(), context.Node.GetText().ToString().Replace(Environment.NewLine, string.Empty));
            context.ReportDiagnostic(diagnostic);
            Task.Delay(2000).GetAwaiter().GetResult();
            Music.Play("Content\\msg.wav");
        }
    }
}
