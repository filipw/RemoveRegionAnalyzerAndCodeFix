using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RemoveRegionAnalyzerAndCodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveRegionCodeFixProvider)), Shared]
    public class RemoveRegionCodeFixProvider : CodeFixProvider
    {
        private const string title = "Get rid of the damn region";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(RemoveRegionAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root.FindNode(diagnosticSpan, findInsideTrivia: true, getInnermostNodeForTie: true);
            DirectiveTriviaSyntax region = node as RegionDirectiveTriviaSyntax;

            if (region == null)
            {
                region = node as EndRegionDirectiveTriviaSyntax;
            }

            if (region != null)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title,
                        async c =>
                        {
                            Music.Play("Content\\msg.wav");
                            var oldRoot = await context.Document.GetSyntaxRootAsync(c);
                            var newRoot = oldRoot.ReplaceNodes(region.GetRelatedDirectives(), (trivia, syntaxTrivia) => SyntaxFactory.SkippedTokensTrivia());
                            var newDocument = context.Document.WithSyntaxRoot(newRoot.NormalizeWhitespace());
                            return newDocument;
                        }),
                    diagnostic);
            }
        }
    }
}