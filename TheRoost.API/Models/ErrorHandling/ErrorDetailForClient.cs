using System.Text.Json;

namespace TheRoost.API.Models.ErrorHandling
{
    public class ErrorDetailForClient
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public Guid ErrorID { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
