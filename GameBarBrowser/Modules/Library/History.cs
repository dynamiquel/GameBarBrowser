using GameBarBrowser.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameBarBrowser.Library
{
    public class History
    {
        public event Action HistoryModified;

        private List<Artifact> artifacts = new List<Artifact>();

        public Task<List<Artifact>> Query(string query)
        {
            // Queries name first, then URL.
            var matchedElements = new List<Artifact>();

            matchedElements.AddRange(artifacts.Where(b => b.Name.Contains(query)));

            var remainingArtifacts = artifacts.Except(matchedElements);
            matchedElements.AddRange(remainingArtifacts.Where(b => b.URI.Contains(query)));

            return Task.FromResult(matchedElements);
        }

        public Task<List<Artifact>> GetArtifacts()
        {
            return GetArtifacts(string.Empty);
        }

        public Task<List<Artifact>> GetArtifacts(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Task.FromResult(artifacts);

            return Query(query);
        }

        public void AddRange(IEnumerable<Artifact> artifactsToAdd)
        {
            if (artifacts == null)
                artifacts = new List<Artifact>();

            if (!UserSettings.RecordHistory)
                return;

            foreach (var artifact in artifactsToAdd)
            {
                if (UserSettings.IgnoreDuplicatedHistory && artifacts.Any(a => a.URI == artifact.URI))
                {
                    var previouslyAttended = artifacts.First(a => a.URI == artifact.URI);
                    previouslyAttended.TimesVisited++;
                    previouslyAttended.LastVisited = artifact.FirstVisited;
                }
                else
                    artifacts.Add(artifact);

                System.Diagnostics.Debug.WriteLine("Artifact added");
            }

            HistoryModified?.Invoke();
        }

        public void Add(Artifact artifact)
        {
            AddRange(new Artifact[] { artifact });
        }

        public void RemoveAll(DateTime olderThan)
        {
            // Clearing is quicker than removing with Linq.
            if (olderThan == DateTime.MinValue)
                artifacts.Clear();
            else
                artifacts.RemoveAll(a => a.LastVisited >= olderThan);

            HistoryModified?.Invoke();
        }

        public void Remove(Artifact artifact)
        {
            if (artifacts.Contains(artifact))
                artifacts.Remove(artifact);

            HistoryModified?.Invoke();
        }

        public void Clear(bool dontInvokeHistoryModified = false)
        {
            if (artifacts != null)
                artifacts.Clear();

            if (!dontInvokeHistoryModified)
                HistoryModified?.Invoke();
        }
    }
}
