using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace SOFA_Bot_Test.Nades
{
    internal class NadeGoogleSheet
    {
        internal static async Task SaveNadePollResultsToSheet(List<List<string>> grenadeChoicesNames)
        {
            SheetsService? service = await GetSheetService();
            if (service == null)
            {
                Logger.LogError("Service for nade sheet is null");
                return;
            }
            string sheetId = BotInfo.GetNadeSheetId();
            string newSheetName = await DuplicateSheetTab(service, sheetId);
            //TODO LOW generate data change values to checkboxes
            IList<IList<Object>> objNeRecords = await GenerateData(grenadeChoicesNames);
            string newRange = $"{newSheetName}!A3";
            await UpdateGoogleSheet(objNeRecords, sheetId, newRange, service);
            Logger.LogInformation($"Finished updating nade sheet");
            return;
        }
        private static async Task<SheetsService?> GetSheetService()
        {
            Logger.LogInformation($"Getting nade sheet service");
            string clientId = BotInfo.GetSheetClientId();
            string clientSecret = BotInfo.GetSheetClientSecret();
            string[] scopes = { SheetsService.Scope.Spreadsheets };
            UserCredential? credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                scopes,
                "FOFA Bot",
                CancellationToken.None,
                new FileDataStore("GoogleToken"))
                .Result;
            SheetsService? service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "FOFA Bot"
            });
            return service;
        }
        private static async Task<IList<IList<Object>>> GenerateData(List<List<string>> grenadeChoicesNames)
        {
            Logger.LogInformation($"Generating data for the nade sheet");
            List<IList<Object>> fullObject = new();
            int i = 1;
            List<string> pollAllNames = await GetListOfPollUsers(grenadeChoicesNames);
            foreach (string name in pollAllNames)
            {
                IList<Object> objectLine = [];
                objectLine.Add(i);
                i++;
                objectLine.Add($"=INDIRECT(CONCAT(\"Data!B\";MATCH(\"{name}\";Data!$A$2:$A$1000;0)+1))");
                foreach (List<string> choice in grenadeChoicesNames)
                {
                    if (choice.Contains(name))
                        objectLine.Add(true);
                    else
                        objectLine.Add(false);
                }
                objectLine.Add(false);
                fullObject.Add(objectLine);
            }
            return fullObject;
        }
        private static async Task<List<string>> GetListOfPollUsers(List<List<string>> grenadeChoicesNames)
        {
            List<string> pollAllNames = new();
            foreach (List<string> choice in grenadeChoicesNames)
            {
                foreach (string choiceName in choice)
                {
                    if (!pollAllNames.Contains(choiceName))
                        pollAllNames.Add(choiceName);
                }
            }
            Logger.LogInformation($"Found {pollAllNames.Count} users that voited in the poll");
            return pollAllNames;
        }
        private static async Task<string> DuplicateSheetTab(SheetsService service, string sheetId)
        {
            Logger.LogInformation($"Starting duplicating sheet tab for nades");
            string newSheetName = $"{DateTime.Now.ToString("d")}";
            BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
            batchUpdateSpreadsheetRequest.Requests =
            [
                new Request()
                {
                    DuplicateSheet = new DuplicateSheetRequest()
                    {
                        InsertSheetIndex = 2,
                        NewSheetName = newSheetName,
                        SourceSheetId = 0
                    }
                }
            ];
            var request = service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, sheetId);
            try
            {
                BatchUpdateSpreadsheetResponse response = request.Execute();
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.ToString());
            }
            Logger.LogInformation($"Finished duplicating sheet for nades");
            return newSheetName;
        }
        private static async Task UpdateGoogleSheet(IList<IList<Object>> values, string spreadsheetId, string range, SheetsService service)
        {
            Logger.LogInformation($"Updating nades google sheet");
            var request = service.Spreadsheets.Values.Append(new ValueRange() { Values = values }, spreadsheetId, range);
            request.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            AppendValuesResponse? response = request.Execute();
        }
    }
}
