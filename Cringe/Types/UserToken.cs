namespace Cringe.Types
{
    public class UserToken
    {
        public string Token { get; set; }

        public int PlayerId { get; set; }

        public string Username { get; set; }
        public override string ToString()
        {
            return $"{PlayerId} | {Username}";
        }
    }
}
