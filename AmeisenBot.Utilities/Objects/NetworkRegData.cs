namespace AmeisenBotUtilities
{
    public class NetworkRegData
    {
        public SendableMe Me { get; set; }
        public string Base64Image { get; set; }

        public NetworkRegData(string base64Image, SendableMe me)
        {
            Me = me;
            Base64Image = base64Image;
        }
    }
}