using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;


namespace FOFA_Bot.Attendance
{
    internal class AttendanceGoogleSheet
    {
        internal static async Task HandleUnsignedUsers(List<string> userNames)
        {
            SheetsService? service = await GetSheetService();
            string sheetId = BotInfo.GetAttendanceSheetId();
            string newRange = await GetRange(userNames.Count + 1);
            IList<IList<Object>> objNeRecords = await GenerateData(userNames);
            if (service != null)
                await UpdateGoogleSheet(objNeRecords, sheetId, newRange, service);
            Logger.LogInformation($"Finished updating attendance sheet");
            return;
        }
        private static async Task<SheetsService?> GetSheetService()
        {
            Logger.LogInformation($"Getting attendance sheet service");
            string clientId = BotInfo.GetSheetClientId();
            string clientSecret = BotInfo.GetSheetClientSecret();
            string[] scopes = [SheetsService.Scope.Spreadsheets];
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
            SheetsService? service = new(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "FOFA Bot"
            });
            return service;
        }
        private static async Task<string> GetRange(int numberOfCollumns)
        {
            char lastCollumnChar = (char)('A' - 1 + numberOfCollumns);
            return ($"A{2}:{lastCollumnChar}2");
        }
        private static async Task<IList<IList<Object>>> GenerateData(List<string> userNames)
        {
            Logger.LogInformation($"Generating data for the attendance sheet");
            List<IList<Object>> fullObject = [];
            IList<Object> objectLine = [];
            objectLine.Add(DateTime.Now.ToString("dd/MM/yyyy"));
            foreach (string userName in userNames)
                objectLine.Add(userName);
            fullObject.Add(objectLine);
            return fullObject;
        }
        private static async Task UpdateGoogleSheet(IList<IList<Object>> values, string spreadsheetId, string range, SheetsService service)
        {
            Logger.LogInformation($"Updating attendance google sheet");
            var request = service.Spreadsheets.Values.Append(new ValueRange() { Values = values }, spreadsheetId, range);
            request.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            request.Execute();
        }

    }
}