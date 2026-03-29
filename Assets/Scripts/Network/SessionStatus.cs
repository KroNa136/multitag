using UnityEngine;

#region Host

public enum StartSessionStatus
{
    None,
    Created,
    Failed
}

public enum KickPlayerStatus
{
    None,
    Kicked,
    IsNotHost,
    Failed
}

public enum StopSessionStatus
{
    None,
    Stopped,
    Failed
}

#endregion Host

#region Client

public enum FindSessionsStatus
{
    None,
    Found,
    Failed
}

public enum JoinSessionStatus
{
    None,
    Joined,
    NotFound,
    SessionIsFull,
    Failed
}

public enum LeaveSessionStatus
{
    None,
    Left,
    Failed
}

#endregion Client
