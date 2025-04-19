namespace OrderManager.Shared;

public class ValidationResult
{
    public bool IsValid { get; private set; } = true;
    public bool IsNotValid => !IsValid;
    public IList<Error> Errors { get; } = [];

    public void AddError(Error error)
    {
        Errors.Add(error);
        IsValid = false;
    }
    
    public void AddError(string code, string description)
    {
        AddError(new Error(code, description, ErrorType.Problem));
    }
}