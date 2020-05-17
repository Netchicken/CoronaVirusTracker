namespace VirusTracker.Models
{
    public class LogoutTrackerModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}

/*
public const string SessionKeyName = "_Name";
public const string SessionKeyAge = "_Age";
const string SessionKeyTime = "_Time";

public string SessionInfo_Name { get; private set; }
public string SessionInfo_Age { get; private set; }
public string SessionInfo_CurrentTime { get; private set; }
public string SessionInfo_SessionTime { get; private set; }
public string SessionInfo_MiddlewareValue { get; private set; }

public void OnGet()
{
    // Requires: using Microsoft.AspNetCore.Http;
    if (string.IsNullOrEmpty(Microsoft.AspNetCore.Http.HttpContext.Session.GetString(SessionKeyName)))
    {
        HttpContext.Session.SetString(SessionKeyName, "The Doctor");
        HttpContext.Session.SetInt32(SessionKeyAge, 773);
    }

    var name = HttpContext.Session.GetString(SessionKeyName);
    var age = HttpContext.Session.GetInt32(SessionKeyAge);






}*/
