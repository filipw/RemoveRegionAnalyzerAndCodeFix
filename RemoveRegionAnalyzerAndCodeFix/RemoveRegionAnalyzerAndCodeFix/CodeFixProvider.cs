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
            var node = root.FindNode(context.Span, true, true);
            var region = node as DirectiveTriviaSyntax;

            if (region != null)
            {
                context.RegisterCodeFix(CodeAction.Create(title, c =>
                {
                    Music.Play("Content\\msg.wav");
                    var newRoot = root.RemoveNodes(region.GetRelatedDirectives(), SyntaxRemoveOptions.AddElasticMarker);
                    var newDocument = context.Document.WithSyntaxRoot(newRoot);
                    return Task.FromResult(newDocument);
                } ), context.Diagnostics.First());
            }
        }
    }
}
