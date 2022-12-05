using SimpleJSON;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace L11.Sync.POEditor.Editor
{
    internal class ImportResult
    {
        public bool Success;
        public string Json;
    }

    internal class POEditorImporter
    {
        public async Task<ImportResult> ImportAsync(POEditorSyncSettings settings, POEditorImportEntry entry)
        {
            var progress = Progress.Start($"Importing {entry.Language} localization", options: Progress.Options.Sticky);

            Progress.SetDescription(progress, "Getting download URL");

            var downloadUrl = await FetchDownloadURLAsync(settings, entry);
            if (downloadUrl == null)
            {
                Progress.SetDescription(progress, "Failed to fetch download URL");
                Progress.Finish(progress, Progress.Status.Failed);
                return new ImportResult() { Success = false };
            }

            Progress.SetDescription(progress, "Downloading");
            Progress.Report(progress, 0.1f);

            var request = UnityWebRequest.Get(downloadUrl);
            request.SendWebRequest();

            while (request.result == UnityWebRequest.Result.InProgress)
            {
                Progress.Report(progress, Mathf.Lerp(0.1f, 1f, request.downloadProgress));
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                Progress.SetDescription(progress, "Success");
                Progress.Finish(progress, Progress.Status.Succeeded);

                return new ImportResult()
                {
                    Success = true,
                    Json = request.downloadHandler.text
                };
            }

            Progress.SetDescription(progress, request.result.ToString());
            Progress.Finish(progress, Progress.Status.Failed);

            return new ImportResult() { Success = false };
        }

        private async Task<string> FetchDownloadURLAsync(POEditorSyncSettings settings, POEditorImportEntry entry)
        {
            var fetchTask = new TaskCompletionSource<string>();

            var form = new WWWForm();
            form.AddField("api_token", settings.POEditorApiKey);
            form.AddField("id", settings.POEditorProjectId);
            form.AddField("language", entry.Language);
            form.AddField("type", "json");

            var request = UnityWebRequest.Post("https://api.poeditor.com/v2/projects/export", form);
            request.SendWebRequest().completed += (op) =>
            {
                var response = JSON.Parse(request.downloadHandler.text);
                if (response["response"]["code"].AsInt == 200)
                {
                    fetchTask.SetResult(response["result"]["url"]);
                }
                else
                {
                    Debug.LogError($"Failed to get download url for localization file: {response["response"]["message"]}");
                    fetchTask.SetResult(null);
                }
            };

            return await fetchTask.Task;
        }
    }
}
