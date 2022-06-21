using System.Collections.Specialized;
using System.Text.RegularExpressions;
using CricketBootstrap.Environment;

namespace CricketBootstrap.Server;

public class DynamicDocument {
    private static readonly Regex SearchRegex = new("(?<=<chirp>).*?(?=</chirp>)");
    private static readonly Regex ReplacementRegex = new("(?=<chirp>).*?(?<=</chirp>)");
    private readonly string _document;
    private readonly MatchCollection _searchMatches;
    private readonly MatchCollection _replacementMatches;
    private readonly EnvironmentManager _environmentManager;

    public DynamicDocument(string document, EnvironmentManager environmentManager) {
        _document = document;
        _environmentManager = environmentManager;
        _searchMatches = SearchRegex.Matches(document);
        _replacementMatches = ReplacementRegex.Matches(document);
    }

    public string Parse(NameValueCollection nameValueCollection) {
        _environmentManager.AddGetRequestValues(nameValueCollection);
        string dynamicDocument = (string) _document.Clone();
        for (int i = 0; i < _searchMatches.Count; i++) {
            dynamicDocument = dynamicDocument.Replace(_replacementMatches[i].Value, Execute(_searchMatches[i].Value));
        }
        _environmentManager.RemoveGetRequestValues(nameValueCollection);
        return dynamicDocument;
    }

    private string Execute(string command) {
        var arguments = command.Split(' ');
        if (arguments.Length != 2) {
            return string.Empty;
        }
        return (string) _environmentManager.EnvironmentsInterpreter[arguments[0]].CallFunction(arguments[1]);
    }
}