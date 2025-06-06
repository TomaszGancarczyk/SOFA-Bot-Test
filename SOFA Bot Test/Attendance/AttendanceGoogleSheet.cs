using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;


namespace SOFA_Bot_Test.Attendance
{
    internal class AttendanceGoogleSheet
    {
        internal async static Task HandleUnsignedUsers(List<string> userNames)
        {
            Logger.LogInformation($"{userNames.Count} members didn't signed up");
            string LastSheetRowPath = "LastSheetRow.txt";
            int? currentSheetRow = ReadAndUpdateSheetRowFile(LastSheetRowPath);
            if (currentSheetRow == null)
                return;
            else
                await SaveNamesToBotSignupsSheet(userNames, currentSheetRow);


            return;
            //date - user - user - user
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
                    File.WriteAllText(filePath, lastSheetRow + 1.ToString());
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
            string sheetId = "1ZC__GnEiITdrm8Wc6k-GBlkDG3Xpt2nzEnm2n6HZRb8/edit?gid=0#gid=0";
            //string newRange = GetRange(service, sheetId);
            //IList<IList<Object>> objNeRecords = GenerateData();
            //UpdatGoogleSheetinBatch(objNeRecords, SheetId, newRange, service);
            return;
        }
        private async static Task<SheetsService?> GetSheetService()
        {
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
    }
}