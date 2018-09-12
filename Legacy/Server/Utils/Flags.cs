using System;

/// <summary>
/// codes we get from user
/// </summary>
public enum ProcessCodes : ushort
{
    Login,
    Register,
    Fail
}
/// <summary>
/// codes we send back to user
/// </summary>
public enum ErrorCodes : ushort
{
    Success,
    Exists,
    InvalidLogin,
    Banned,
    TokenUsed,
    AlreadyLogged,
    Error
}
