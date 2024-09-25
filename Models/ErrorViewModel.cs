namespace YouWeMovie.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    //fasd

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}