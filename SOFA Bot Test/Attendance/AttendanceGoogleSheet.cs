using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;


namespace FOFA_Bot.Attendance
{
    internal class AttendanceGoogleSheet
    {
        internal async static Task HandleUnsignedUsers(List<string> userNames)
        {
            Logger.LogInformation($"{userNames.Count} members didn't signed up");
            string LastSheetRowPath = "..\\..\\..\\Attendance\\LastSheetRow.txt";
            int? currentSheetRow = ReadAndUpdateSheetRowFile(LastSheetRowPath);
            if (currentSheetRow == null)
                return;
            else
                await SaveNamesToBotSignupsSheet(userNames, currentSheetRow);
            return;
        }
        private static int? ReadAndUpdateSheetRowFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Logger.LogError("Cannot find LastSheetRow.txt");
                return null;
            }
            else
            {
                Logger.LogInformation("Found LastSheetRow.txt");
                string lastSheetRowString = File.ReadAllText(filePath);
                try
                {
                    int lastSheetRow = Int32.Parse(lastSheetRowString);
                    Logger.LogInformation($"with the last row being {lastSheetRow}");
                    File.WriteAllText(filePath, $"{lastSheetRow + 1}");
                    return lastSheetRow + 1;
                }
                catch
                {
                    Logger.LogError("Cannot parse LastSheetRow.txt to int");
                    return null;
                }
            }
        }

        private async static Task SaveNamesToBotSignupsSheet(List<string> userNames, int? currentRow)
        {
            SheetsService? service = await GetSheetService();
            string sheetId = "1ZC__GnEiITdrm8Wc6k-GBlkDG3Xpt2nzEnm2n6HZRb8";
            string newRange = await GetRange(currentRow, userNames.Count + 1);
            IList<IList<Object>> objNeRecords = await GenerateData(userNames);
            await UpdatGoogleSheet(objNeRecords, sheetId, newRange, service);
            Logger.LogInformation($"Finished updating sheet");
            return;
        }
        private async static Task<SheetsService?> GetSheetService()
        {
            Logger.LogInformation($"Getting sheet service");
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
        private async static Task<string> GetRange(int? currentRow, int numberOfCollumns)
        {
            char lastCollumnChar = (char)('A' - 1 + numberOfCollumns);
            return ($"A{currentRow}:{lastCollumnChar}{currentRow}");
        }
        private async static Task<IList<IList<Object>>> GenerateData(List<string> userNames)
        {
            Logger.LogInformation($"Generating data for the sheet");
            List<IList<Object>> fullObject = new List<IList<Object>>();
            IList<Object> objectLine = [];
            objectLine.Add(DateTime.Now.ToString("dd/MM/yyyy"));
            foreach (string userName in userNames)
                objectLine.Add(userName);
            fullObject.Add(objectLine);
            return fullObject;
        }
        private async static Task UpdatGoogleSheet(IList<IList<Object>> values, string spreadsheetId, string range, SheetsService service)
        {
            Logger.LogInformation($"Updating google sheet");
            SpreadsheetsResource.ValuesResource.AppendRequest request = service.Spreadsheets.Values.Append(new ValueRange() { Values = values }, spreadsheetId, range);
            request.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            AppendValuesResponse? response = request.Execute();
        }
    }
}