using System.IO;

namespace Hackathon_TestCaseGenerator.Services
{
    public class DocumentationService
    {
        private readonly string _docPath;

        public DocumentationService(string docPath)
        {
            _docPath = docPath;
        }

        public string GetDocumentationContent()
        {
            if (!File.Exists(_docPath))
                throw new FileNotFoundException("Documentation file not found.", _docPath);

            return File.ReadAllText(_docPath);
        }
    }
}
