using System;

/// <summary>
/// codes we get from user
/// </summary>
public enum ProcessCodes : ushort
{
    Login,
    Register,
    Message,
    Ban,
    Kick,
    Reboot,
    Version,
    Logger,
    Command,
    UpdateUsers,
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
    Version_Success,
    Error
}
