using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameBarBrowser.Library
{
    public class History
    {
        List<Artifact> artifacts;

        public Task<List<Artifact>> Query(string query)
        {
            // Queries name first, then URL.
            var matchedElements = new List<Artifact>();

            matchedElements.AddRange(artifacts.Where(b => b.Name.Contains(query)));

            var remainingArtifacts = artifacts.Except(matchedElements);
            matchedElements.AddRange(remainingArtifacts.Where(b => b.URL.Contains(query)));

            return Task.FromResult(matchedElements);
        }

        public void AddRange(List<Artifact> artifactsToAdd)
        {
            if (artifacts == null)
                artifacts = new List<Artifact>();

            artifacts.AddRange(artifacts);
        }

        public void Clear()
        {
            if (artifacts != null)
                artifacts.Clear();
        }
    }
}
